using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using MongoDB.Bson;
using ZreperujTo.Web.Data;
using ZreperujTo.Web.Helpers;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.CategoryModels;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.DbModels;
using ZreperujTo.Web.Models.FailModels;
using ZreperujTo.Web.Models.FileInfoModels;
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Fails")]
    public class FailsApiController : Controller
    {
        private readonly IZreperujToService _serviceCore;

        public FailsApiController(ZreperujToDbClient serviceCore)
        {
            _serviceCore = serviceCore;
        }
        
        // GET: api/Fails/Browse/{categoryId?}/{subcategoryId?}
        [HttpGet("Browse")]
        [HttpGet("Browse/{categoryId}")]
        [HttpGet("Browse/{categoryId}/{subcategoryId}")]
        [ProducesResponseType(typeof(IEnumerable<FailMetaModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(
            string categoryId,
            string subcategoryId,
            [FromQuery] string city,
            [FromQuery] string district,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? validThrough,
            [FromQuery] List<SpecialRequirement> requirements,
            [FromQuery] string sortOrder,
            [FromQuery] int pageLimit = 10,
            [FromQuery] int pageNumber = 1
            )
        {
            ObjectId categoryObjectId, subcategoryObjectId;
            ObjectId.TryParse(categoryId, out categoryObjectId);
            ObjectId.TryParse(subcategoryId, out subcategoryObjectId);
            
            var result = await _serviceCore.GetFailsMetaAsync(categoryObjectId, subcategoryObjectId, city, district, minPrice, maxPrice,
                validThrough, requirements, sortOrder, pageLimit, pageNumber);
            return Ok(result);
        }

        // GET: api/Fails/Details/5
        [HttpGet("Details/{id}")]
        [ProducesResponseType(typeof(FailReadModel),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            ObjectId failId;
            FailDbModel failDbModel;
            if (!ObjectId.TryParse(id, out failId))
            {
                ModelState.AddModelError("failId", "'failId' is incorrect");
                return BadRequest(ModelState);
            }

            FailReadModel read = null;
            try
            {
                read = await _serviceCore.GetFailReadModelAsync(failId);
            }
            catch (FailDoesNotExistException)
            {
                ModelState.AddModelError("failId", "The fail does not exist");
                return BadRequest(ModelState);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
            return Ok(read);
        }

        // POST: api/Fails/Create
        [HttpPost("Create")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [ProducesResponseType(typeof(FailReadModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]FailWriteModel writeModel)
        {
            if (writeModel == null)
            {
                ModelState.AddModelError("writeModel", "Request BODY cannot be empty");
                return BadRequest(ModelState);
            }

            ObjectId categoryId, subcategoryId;
            CategoryDbModel category;
            SubcategoryDbModel subcategory;
            UserInfoDbModel userInfo;
            string userId = User.GetSubId();
            if (writeModel.PictureIds == null)
                writeModel.PictureIds = new List<string>();

            if (!ObjectId.TryParse(writeModel.CategoryId, out categoryId))
            {
                ModelState.AddModelError("CategoryId", "Invalid or empty 'CategoryId'");
            }
            if (!ObjectId.TryParse(writeModel.SubcategoryId, out subcategoryId))
            {
                ModelState.AddModelError("SubcategoryId", "Invalid or empty 'SubcategoryId'");
            }
            if (writeModel.AuctionValidThrough < DateTime.Now)
            {
                ModelState.AddModelError("AuctionValidThrough", "The DateTime cannot be earlier than now");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            writeModel.Location = writeModel.Location.TrimStrings();
            
            if (writeModel.AuctionValidThrough - DateTime.Now > TimeSpan.FromDays(7.0))
            {
                writeModel.AuctionValidThrough = DateTime.Now.AddDays(7.0);
            }

            var dbModel = new FailDbModel
            {
                Active = true,
                AssignedBidId = ObjectId.Empty,
                AuctionValidThrough = writeModel.AuctionValidThrough,
                CreatedAt = DateTime.Now,
                UserId = userId,
                CategoryId = categoryId,
                SubcategoryId = subcategoryId,
                Requirements = writeModel.Requirements,
                Budget = writeModel.Budget,
                Description = writeModel.Description,
                Highlited = writeModel.Highlited,
                Location = writeModel.Location,
                Title = writeModel.Title
            };


            FailReadModel result = null;
            try
            {
                result = await _serviceCore.InsertFailDbModelAsync(dbModel, writeModel.PictureIds);
            }
            catch (InvalidCategoryException)
            {
                ModelState.AddModelError("CategoryId", "The category does not exist");
            }
            catch (InvalidSubcategoryException)
            {
                ModelState.AddModelError("SubcategoryId", "The subcategory does not exist");
            }
            catch (UserDoesNotExistException)
            {
                ModelState.AddModelError("UserId", "User does not exist");
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(result);
        }


        // POST: api/Fails/Details/5/MakeBid
        [HttpPost("Details/{id}/Bids/MakeBid")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> MakeBid(string id, [FromBody]BidWriteModel bid)
        {
            ObjectId failId;
            if (bid == null)
            {
                ModelState.AddModelError("bid", "Request body cannot be empty");
            }
            if (!ObjectId.TryParse(id, out failId))
            {
                ModelState.AddModelError("id", "Invalid 'id' of fail");
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                string userId = User.GetSubId();
                var dbModel = new BidDbModel
                {
                    Active = true,
                    Assigned = false,
                    Budget = bid.Budget,
                    Description = bid.Description,
                    FailId = failId,
                    UserId = userId
                };
                await _serviceCore.AddBidAsync(dbModel);
            }
            catch (FailDoesNotExistException)
            {
                ModelState.AddModelError("id", "The fail does not exist");
            }
            catch (FailAlreadyAssignedException)
            {
                ModelState.AddModelError("id", "The fail is already assigned");
            }
            catch (InactiveFailException)
            {
                ModelState.AddModelError("id", "The fail is inactive");
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpGet("Details/{id}/Bids")]
        [ProducesResponseType(typeof(List<BidReadModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBids(string id)
        {
            ObjectId failId;
            if (!ObjectId.TryParse(id, out failId))
            {
                ModelState.AddModelError("id", "Invalid fail id");
                return BadRequest(ModelState);
            }

            List<BidReadModel> result;
            try
            {
                result = await _serviceCore.GetBidReadModelsAsync(failId);
            }
            catch (FailDoesNotExistException)
            {
                ModelState.AddModelError("id", "Fail does not exist");
                return BadRequest(ModelState);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
            return Ok(result);
        }

        [HttpPost("Details/{failId}/Bids/{bidId}/Accept")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> AcceptBid(string failId, string bidId)
        {
            ObjectId failObjectId, bidObjectId;
            if (!ObjectId.TryParse(failId, out failObjectId))
                ModelState.AddModelError("failId", "Invalid 'failId' parameter value");
            if (!ObjectId.TryParse(bidId, out bidObjectId))
                ModelState.AddModelError("bidId", "Invalid 'bidId' parameter value");

            string userId = User.GetSubId();

            try
            {
                await _serviceCore.AcceptBid(bidObjectId, failObjectId, userId);
            }
            catch (FailDoesNotExistException)
            {
                ModelState.AddModelError("failId", "The fail does not exist");
            }
            catch (BidDoesNotExistException)
            {
                ModelState.AddModelError("bidId", "The bid does not exist");
            }
            catch (UserNotOwnerException)
            {
                ModelState.AddModelError("userId", "The user is not an owner of the fail");
                return StatusCode((int) HttpStatusCode.Forbidden, ModelState);
            }
            catch (Exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}
