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
            ObjectId failId;
            FailDbModel failDbModel;
            if (!ObjectId.TryParse(id, out failId))
            {
                ModelState.AddModelError("failId", "'failId' is incorrect");
                return BadRequest(ModelState);
            }

            CategoryDbModel category;
            SubcategoryDbModel subcategory;
            #region Getting categories, subcategories and fail from database
            {
                var _failDbModelTask = _zreperujDb.GetFailDbModelAsync(failId);
                var _categoryTask = _zreperujDb.GetCategoriesAsync();
                var _subsubcategoryTask = _zreperujDb.GetSubcategoriesAsync();
                await Task.WhenAll(_categoryTask, _subsubcategoryTask, _failDbModelTask);

                failDbModel = _failDbModelTask.Result;
                if (failDbModel == null)
                {
                    ModelState.AddModelError("failId", "The fail does not exist");
                    return BadRequest(ModelState);
                }
                category = _categoryTask.Result.FirstOrDefault(x => x.Id == failDbModel.CategoryId);
                subcategory = _subsubcategoryTask.Result.FirstOrDefault(x => x.Id == failDbModel.SubcategoryId);
            }
            #endregion


            UserInfoDbModel userInfoDbModel;
            List<BidDbModel> bidsDbModel;
            #region Getting fail's owner userInfo and fail's bids
            {
                var _userInfoTask = _zreperujDb.GetUserInfoDbModelAsync(failDbModel.UserId);
                var _bidsDbModelTask = _zreperujDb.GetBidsAsync(failId);
                await Task.WhenAll(_userInfoTask, _bidsDbModelTask);
                userInfoDbModel = _userInfoTask.Result;
                bidsDbModel = _bidsDbModelTask.Result;
            }
            #endregion

            List<BidReadModel> bids = new List<BidReadModel>();
            var userInfoDbModels = await _zreperujDb.GetUserInfoDbModelAsync(bidsDbModel.Select(x => x.UserId).ToArray());
            foreach (var x in bidsDbModel)
            {
                var user = userInfoDbModels.FirstOrDefault(u => u.UserId == x.UserId);
                user.Ratings = null;
                user.Badges = null;
                bids.Add(new BidReadModel
                {
                    Active = x.Active,
                    Assigned = x.Assigned,
                    Budget = x.Budget,
                    Id = x.Id.ToString(),
                    Description = x.Description,
                    UserId = x.UserId,
                    FailId = failId.ToString(),
                    UserInfo = new UserInfoReadModel(user)
                });
            }

            var read = new FailReadModel
            {
                Active = failDbModel.Active,
                AuctionValidThrough = failDbModel.AuctionValidThrough,
                Category = (category != null) ? new CategoryReadModel(category) : null,
                Subcategory = (subcategory != null) ? new SubcategoryReadModel(subcategory) : null,
                Bids = bids,
                Budget = failDbModel.Budget,
                Description = failDbModel.Description,
                Id = failDbModel.Id.ToString(),
                Highlited = failDbModel.Highlited,
                Location = failDbModel.Location,
                Pictures = failDbModel.Pictures,
                Requirements = failDbModel.Requirements,
                Title = failDbModel.Title,
                UserInfo = new UserInfoMetaModel
                {
                    Email = userInfoDbModel.Email,
                    Company = userInfoDbModel.Company,
                    MobileNumber = userInfoDbModel.MobileNumber,
                    Name = userInfoDbModel.Name,
                    RatingCount = userInfoDbModel.RatingCount,
                    RatingSum = userInfoDbModel.RatingSum,
                    Id = userInfoDbModel.UserId
                },
                AssignedBid = bids.FirstOrDefault(x=>x.Id == failDbModel.AssignedBidId.ToString())
            };

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

            List<CategoryDbModel> categories;
            List<SubcategoryDbModel> subcategories;
            List<PictureInfoDbModel> pictureDbModels;
            {
                var _categoriesTask = _zreperujDb.GetCategoriesAsync();
                var _subcategoriesTask = _zreperujDb.GetSubcategoriesAsync();
                var _userInfoTask = _zreperujDb.GetUserInfoDbModelAsync(userId);
                var _picturesTask = _zreperujDb.GetPictureInfoDbModelsAsync(writeModel.PictureIds);
                await Task.WhenAll(_categoriesTask, _subcategoriesTask, _userInfoTask, _picturesTask);
                categories = _categoriesTask.Result;
                subcategories = _subcategoriesTask.Result;
                userInfo = _userInfoTask.Result;
                pictureDbModels = _picturesTask.Result;
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
                Pictures = pictureDbModels.Select(x=>new PictureInfoReadModel
                {
                    OriginalFileUri = x.OriginalSizeUri,
                    ThumbnailFileUri = x.ThumbnailUri
                }).ToList(),
                Title = writeModel.Title
            };

            var obj = await _zreperujDb.InsertFailDbModelAsync(dbModel);
            

            var result = new FailReadModel
            {
                Active = obj.Active,
                AuctionValidThrough = obj.AuctionValidThrough,
                Category = (category != null) ? new CategoryReadModel(category) : null,
                Subcategory = (subcategory != null) ? new SubcategoryReadModel(subcategory) : null,
                Budget = obj.Budget,
                Description = obj.Description,
                Id = obj.Id.ToString(),
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
                    Id = userInfo.UserId,
                    RatingSum = userInfo.RatingSum,
                    RatingCount = userInfo.RatingCount
                }
            };
            
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
            {
                return BadRequest(ModelState);
            }

            var fail = await _zreperujDb.GetFailDbModelAsync(failId);
            if (fail.AssignedBidId != null && fail.AssignedBidId != ObjectId.Empty)
            {
                ModelState.AddModelError("id", "Fail is already assigned");
            }
            if (fail.Active == false)
            {
                ModelState.AddModelError("id", "Fail is canceled/closed");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = User.Claims.FirstOrDefault(
                        x => x.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            var dbModel = new BidDbModel
            {
                Active = true,
                Assigned = false,
                Budget = bid.Budget,
                Description = bid.Description,
                FailId = fail.Id,
                UserId = userId
            };
            await _zreperujDb.AddBidAsync(dbModel);
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpGet("Details/{id}/Bids")]
        [ProducesResponseType(typeof(List<BidReadModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetBids(string id)
        {
            var failId = ObjectId.Parse(id);
            var bidsDb = await _zreperujDb.GetBidsAsync(failId);
            List<BidReadModel> bids = new List<BidReadModel>();
            Parallel.ForEach(bidsDb, x =>
            {
                var user = _zreperujDb.GetUserInfoDbModelAsync(x.UserId).Result;
                user.Ratings = null;
                user.Badges = null;
                bids.Add(new BidReadModel
                {
                    Active = x.Active,
                    Assigned = x.Assigned,
                    Budget = x.Budget,
                    Id = x.Id.ToString(),
                    Description = x.Description,
                    UserId = x.UserId,
                    FailId = failId.ToString(),
                    UserInfo = new UserInfoReadModel(user)
                });
            });
            return Ok(bids);
        }

        [HttpPost("Details/{failId}/Bids/{bidId}/Accept")]
        [Authorize(ActiveAuthenticationSchemes = "Bearer")]
        [ProducesResponseType(typeof(FailReadModel), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> AcceptBid(string failId, string bidId)
        {
            var failObjectId = ObjectId.Parse(failId);
            var bidObjectId = ObjectId.Parse(bidId);

            var fail = await _zreperujDb.GetFailDbModelAsync(failObjectId);
            var bid = await _zreperujDb.GetBidAsync(bidObjectId);
            string userId = User.GetSubId();

            if (fail != null && bid != null && fail.UserId == userId)
            {
                await _zreperujDb.AcceptBid(bidObjectId, failObjectId);
                return Ok();
            }
            else
            {
                if (fail == null)
                {
                    ModelState.AddModelError("failId", "The fail does not exist");
                }
                if (bid == null)
                {
                    ModelState.AddModelError("bidId", "The bid does not exist");
                }
                if (fail.UserId != userId)
                {
                    ModelState.AddModelError("error", "You are not the owner of this fail");
                }
                return BadRequest(ModelState);
            }
        }
    }
}
