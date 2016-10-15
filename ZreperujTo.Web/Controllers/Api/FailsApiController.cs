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
using ZreperujTo.Web.Models.CategoryModels;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.DbModels;
using ZreperujTo.Web.Models.FailModels;
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Fails")]
    public class FailsApiController : Controller
    {
        private readonly ZreperujToDbClient _zreperujDb;

        public FailsApiController(ZreperujToDbClient zreperujDb)
        {
            _zreperujDb = zreperujDb;
        }
        
        // GET: api/Fails/Browse/{categoryId?}/{subcategoryId?}
        [HttpGet("Browse")]
        [HttpGet("Browse/{categoryId:int}")]
        [HttpGet("Browse/{categoryId:int}/{subcategoryId:int?}")]
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
            
            var result = await _zreperujDb.GetFailsMetaAsync(categoryObjectId, subcategoryObjectId, city, district, minPrice, maxPrice,
                validThrough, requirements, sortOrder, pageLimit, pageNumber);
            return Ok(result);
        }

        // GET: api/Fails/Details/5
        [HttpGet("Details/{id}")]
        [ProducesResponseType(typeof(FailReadModel),(int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(string id)
        {
            var objId = ObjectId.Parse(id);
            var dbModel = await _zreperujDb.GetFailDbModelAsync(objId);

            var category = (await _zreperujDb.GetCategoriesAsync()).FirstOrDefault(x => x.Id == dbModel.CategoryId);
            var subcategory = (await _zreperujDb.GetSubcategoriesAsync()).FirstOrDefault(x => x.Id == dbModel.SubcategoryId);

            var userInfo = await _zreperujDb.GetUserInfoDbModelAsync(dbModel.UserId);

            var read = new FailReadModel
            {
                Active = dbModel.Active,
                AuctionValidThrough = dbModel.AuctionValidThrough,
                Category = (category != null) ? new CategoryReadModel(category) : null,
                Subcategory = (subcategory != null) ? new SubcategoryReadModel(subcategory) : null,
                // TODO: Bids
                Budget = dbModel.Budget,
                Description = dbModel.Description,
                FailId = dbModel.Id.ToString(),
                Highlited = dbModel.Highlited,
                Location = dbModel.Location,
                Pictures = dbModel.Pictures,
                Requirements = dbModel.Requirements,
                Title = dbModel.Title,
                UserInfo = new UserInfoMetaModel
                {
                    Email = userInfo.Email,
                    Company = userInfo.Company,
                    MobileNumber = userInfo.MobileNumber,
                    Name = userInfo.Name,
                    RatingCount = userInfo.RatingCount,
                    RatingSum = userInfo.RatingSum,
                    UserId = userInfo.UserId
                }
            };

            return Ok(read);
        }
        
        // POST: api/Fails/Create
        [HttpPost("Create")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [ProducesResponseType(typeof(FailReadModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]FailWriteModel writeModel)
        {
            ObjectId categoryId, subcategoryId;
            var categories = await _zreperujDb.GetCategoriesAsync();
            var subcategories = await _zreperujDb.GetSubcategoriesAsync();
            CategoryDbModel category;
            SubcategoryDbModel subcategory;

            if (writeModel == null)
            {
                ModelState.AddModelError("writeModel","Request BODY cannot be empty");
            }
            if (!ObjectId.TryParse(writeModel.CategoryId, out categoryId))
            {
                ModelState.AddModelError("CategoryId", "Invalid or empty 'CategoryId'");
            }
            if (!ObjectId.TryParse(writeModel.SubcategoryId, out subcategoryId))
            {
                ModelState.AddModelError("SubcategoryId", "Invalid or empty 'SubcategoryId'");
            }
            if ((category = categories.FirstOrDefault(x => x.Id == categoryId)) == null)
            {
                ModelState.AddModelError("CategoryId", "This category does not exist");
            }
            if ((subcategory = subcategories.FirstOrDefault(x => x.Id == subcategoryId)) == null)
            {
                ModelState.AddModelError("SubcategoryId", "This subcategory does not exist");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            writeModel.Location = writeModel.Location.TrimStrings();
            
            if (writeModel.AuctionValidThrough == null || writeModel.AuctionValidThrough < DateTime.Now ||
                writeModel.AuctionValidThrough - DateTime.Now > TimeSpan.FromDays(7.0))
            {
                writeModel.AuctionValidThrough = DateTime.Now.AddDays(7.0);
            }

            string userId = User.Claims.FirstOrDefault(
                        x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            


            var dbModel = new FailDbModel
            {
                Active = true,
                AssignedBidId = null,
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
                Pictures = writeModel.Pictures,
                Title = writeModel.Title
            };

            var obj = await _zreperujDb.InsertFailDbModelAsync(dbModel);
            var userInfo = await _zreperujDb.GetUserInfoDbModelAsync(userId);

            var result = new FailReadModel
            {
                Active = obj.Active,
                AuctionValidThrough = obj.AuctionValidThrough,
                Category = (category != null) ? new CategoryReadModel(category) : null,
                Subcategory = (subcategory != null) ? new SubcategoryReadModel(subcategory) : null,
                Budget = obj.Budget,
                Description = obj.Description,
                FailId = obj.Id.ToString(),
                Highlited = obj.Highlited,
                Location = obj.Location,
                Pictures = obj.Pictures,
                Requirements = obj.Requirements,
                Title = obj.Title,
                UserInfo = new UserInfoMetaModel
                {
                    Company = userInfo.Company,
                    Email = userInfo.Email,
                    MobileNumber = userInfo.MobileNumber,
                    Name = userInfo.Name,
                    UserId = userInfo.UserId,
                    RatingSum = userInfo.RatingSum,
                    RatingCount = userInfo.RatingCount
                }
            };
            
            return Ok(result);
        }
        
        //// PUT: api/Fails/
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}
        
        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
