using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
using MongoDB.Driver;
using ZreperujTo.Web.Data;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.CategoryModels;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.DbModels;
using ZreperujTo.Web.Models.FailModels;
using ZreperujTo.Web.Models.FileInfoModels;
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Helpers
{
    public class ZreperujToDbClient : IZreperujToService
    {
        private IMongoClient _mongoClient;
        private IMongoDatabase _mongoDb;
        private ApplicationDbContext _db;
        private CloudStorageAccount _cloudStorageAccount;

        private const string UserInfoCollectionName = "user_info";
        private const string BidsCollectionName = "bids";
        private const string FailsCollectionName = "fails";
        private const string CategoriesCollectionName = "categories";
        private const string SubcategoriesCollectionName = "subcategories";
        private const string PictureInfoCollectionName = "picture_info";
        private const string ContainerName = "zreperujto";

        public ZreperujToDbClient(
            IMongoClient mongoClient,
            IMongoDatabase mongoDb,
            ApplicationDbContext db,
            CloudStorageAccount cloudStorageAccount)
        {
            _mongoClient = mongoClient;
            _mongoDb = mongoDb;
            _db = db;
            _cloudStorageAccount = cloudStorageAccount;
        }

        public async Task<bool> AddUserInfoAsync(UserInfoDbModel model)
        {
            await _mongoDb.GetCollection<UserInfoDbModel>(UserInfoCollectionName).InsertOneAsync(model);
            return true;
        }

        public async Task<UserInfoDbModel> GetUserInfoDbModelAsync(string userId)
        {
            var res = await (await _mongoDb.GetCollection<UserInfoDbModel>(UserInfoCollectionName).FindAsync(x=>x.UserId == userId)).FirstOrDefaultAsync();
            return res;
        }

        public async Task<List<UserInfoDbModel>> GetUserInfoDbModelsAsync(params string[] userIds)
        {
            var collection = _mongoDb.GetCollection<UserInfoDbModel>(UserInfoCollectionName);
            var filter = Builders<UserInfoDbModel>.Filter.In(x => x.UserId, userIds);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<FailReadModel> InsertFailDbModelAsync(FailDbModel model, List<string> pictureIds = null)
        {
            if (pictureIds == null) pictureIds = new List<string>();

            var collection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName);
            var _categories = GetCategoriesAsync();
            var _subcategories = GetSubcategoriesAsync();
            var _profile = GetUserInfoDbModelAsync(model.UserId);
            var _pictures = GetPictureInfoDbModelsAsync(pictureIds);
            await Task.WhenAll(_categories, _subcategories, _profile, _pictures);

            model.Pictures = _pictures.Result.Select(x => new PictureInfoReadModel
            {
                OriginalFileUri = x.OriginalSizeUri,
                ThumbnailFileUri = x.ThumbnailUri
            }).ToList();

            var categoryDBmodel = _categories.Result.FirstOrDefault(x => x.Id == model.CategoryId);
            var subcategoryDBmodel = _subcategories.Result.FirstOrDefault(x => x.Id == model.SubcategoryId);
            if (categoryDBmodel == null)
            {
                throw new InvalidCategoryException();
            }
            if (subcategoryDBmodel == null)
            {
                throw new InvalidSubcategoryException();
            }
            if (_profile.Result == null)
            {
                throw new UserDoesNotExistException();
            }

            await collection.InsertOneAsync(model);

            var filter = Builders<FailDbModel>.Filter.Eq(x => x.UserId, model.UserId);
            filter = filter & Builders<FailDbModel>.Filter.Eq(x => x.CreatedAt, model.CreatedAt);
            var obj = (await (await collection.FindAsync(filter)).ToListAsync()).FirstOrDefault();

            var result = new FailReadModel
            {
                Active = obj.Active,
                AssignedBid = null,
                AuctionValidThrough = obj.AuctionValidThrough,
                Bids = null,
                Budget = model.Budget,
                Category = new CategoryReadModel(categoryDBmodel),
                Subcategory = new SubcategoryReadModel(subcategoryDBmodel),
                Description = model.Description,
                Highlited = model.Highlited,
                Id = obj.Id.ToString(),
                Pictures = obj.Pictures,
                UserInfo = new UserInfoMetaModel
                {
                    Id = _profile.Result.UserId,
                    Company = _profile.Result.Company,
                    Email = _profile.Result.Email,
                    MobileNumber = _profile.Result.MobileNumber,
                    Name = _profile.Result.Name,
                    RatingCount = _profile.Result.RatingCount,
                    RatingSum = _profile.Result.RatingSum
                },
                Location = obj.Location,
                Requirements = obj.Requirements,
                Title = obj.Title
            };
            return result;
        }

        public async Task<List<FailMetaModel>> GetFailsMetaAsync(ObjectId? categoryId,
            ObjectId? subcategoryId,
            string city,
            string district,
            decimal? minPrice,
            decimal? maxPrice,
            DateTime? validThrough,
            List<SpecialRequirement> requirements,
            string sortOrder,
            int pageLimit = 10,
            int pageNumber = 1)
        {
            var failsCollection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName);
            var filter = Builders<FailDbModel>.Filter.Eq(x => x.Active, true);
            filter = filter & Builders<FailDbModel>.Filter.Eq(x => x.AssignedBidId, ObjectId.Empty);

            if (categoryId.HasValue && categoryId.Value != ObjectId.Empty)
                filter = filter & Builders<FailDbModel>.Filter.Eq(x => x.CategoryId, categoryId.Value);

            if (subcategoryId.HasValue && subcategoryId.Value != ObjectId.Empty)
                filter = filter & Builders<FailDbModel>.Filter.Eq(x => x.SubcategoryId, subcategoryId.Value);

            if (minPrice.HasValue && minPrice.Value != 0)
                filter = filter & Builders<FailDbModel>.Filter.Gte(x => x.Budget, minPrice.Value);

            if (maxPrice.HasValue && maxPrice.Value != 0)
                filter = filter & Builders<FailDbModel>.Filter.Lte(x => x.Budget, maxPrice.Value);

            //// TODO: Implement Db filter
            //if (!String.IsNullOrWhiteSpace(city)) { }
            //if (!String.IsNullOrWhiteSpace(district)) { }    
            var _findTask = failsCollection.FindAsync(filter);
            var categories = GetCategoriesAsync();
            var subcategories = GetSubcategoriesAsync();
            await Task.WhenAll(_findTask, categories, subcategories);
            var list = await _findTask.Result.ToListAsync();
            list =
                list.Where(
                    x => ((!String.IsNullOrWhiteSpace(city)) ? x.Location.City.ToLower().Contains(city.ToLower()) : true)
                    && ((!String.IsNullOrWhiteSpace(district)) ? x.Location.District.ToLower().Contains(district.ToLower()) : true))
                    .ToList();

            

            switch (sortOrder)
            {
                case "budget_asc":
                    list =
                        list.OrderBy(x => x.Budget)
                            .ThenBy(x => x.Highlited)
                            .ToList();
                    break;
                case "budget_desc":
                    list =
                        list.OrderByDescending(x => x.Budget)
                            .ThenBy(x => x.Highlited)
                            .ToList();
                    break;
                case "validthrough_asc":
                    list =
                        list.OrderBy(x => x.AuctionValidThrough)
                            .ThenBy(x => x.Highlited)
                            .ToList();
                    break;
                case "validthrough_desc":
                    list =
                        list.OrderByDescending(x => x.AuctionValidThrough)
                            .ThenBy(x => x.Highlited)
                            .ToList();
                    break;
                default:
                    list = list.OrderBy(x => x.Highlited).ToList();
                    break;
            }
            list.Skip((pageNumber - 1)*pageLimit).Take(pageLimit).ToList();

            var result = new List<FailMetaModel>();
            
            
            foreach (var e in list)
            {
                var category = categories.Result.FirstOrDefault(x => x.Id == e.CategoryId);
                var subcategory = subcategories.Result.FirstOrDefault(x => x.Id == e.SubcategoryId);

                result.Add(new FailMetaModel
                {
                    Active = e.Active,
                    Category = (category != null) ? new CategoryReadModel(category) : null,
                    Subcategory = (subcategory != null) ? new SubcategoryReadModel(subcategory) : null,
                    AuctionValidThrough = e.AuctionValidThrough,
                    Budget = e.Budget,
                    Description = e.Description,
                    Id = e.Id.ToString(),
                    Highlited = e.Highlited,
                    Location = new LocationInfo
                    {
                        City = e.Location.City,
                        District = e.Location.District,
                        PostalCode = e.Location.PostalCode
                    },
                    Pictures = e.Pictures,
                    Title = e.Title,
                    Requirements = e.Requirements
                });
            }
            return result;
        }

        public async Task<List<FailMetaModel>> GetUserFailsMetaAsync(string userId)
        {
            var collection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName)
                .FindAsync(x => x.UserId == userId);
            var categories = GetCategoriesAsync();
            var subcategories = GetSubcategoriesAsync();
            await Task.WhenAll(collection, categories, subcategories);

            var result = new List<FailMetaModel>();
            var list = await collection.Result.ToListAsync();

            foreach (var e in list)
            {
                var category = categories.Result.FirstOrDefault(x => x.Id == e.CategoryId);
                var subcategory = subcategories.Result.FirstOrDefault(x => x.Id == e.SubcategoryId);

                result.Add(new FailMetaModel
                {
                    Active = e.Active,
                    Category = (category != null) ? new CategoryReadModel(category) : null,
                    Subcategory = (subcategory != null) ? new SubcategoryReadModel(subcategory) : null,
                    AuctionValidThrough = e.AuctionValidThrough,
                    Budget = e.Budget,
                    Description = e.Description,
                    Id = e.Id.ToString(),
                    Highlited = e.Highlited,
                    Location = new LocationInfo
                    {
                        City = e.Location.City,
                        District = e.Location.District,
                        PostalCode = e.Location.PostalCode
                    },
                    Pictures = e.Pictures,
                    Title = e.Title,
                    Requirements = e.Requirements
                });
            }
            result = result.OrderByDescending(x=>x.AuctionValidThrough).ToList();
            return result;
        }

        public async Task<FailDbModel> GetFailDbModelAsync(ObjectId objId)
        {
            var collection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName);
            var result = await (await collection.FindAsync(x => x.Id == objId)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<FailReadModel> GetFailReadModelAsync(ObjectId objId)
        {
            var collection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName);
            var filter = Builders<FailDbModel>.Filter.Eq(x => x.Id, objId);
            var failDbModel = await (await collection.FindAsync(filter)).FirstOrDefaultAsync();

            if (failDbModel == null)
                throw new FailDoesNotExistException();

            var userInfo = GetUserInfoDbModelAsync(failDbModel.UserId);
            var categories = GetCategoriesAsync();
            var subcategories = GetSubcategoriesAsync();
            var bidsDb = GetBidDbModelsAsync(failDbModel.Id);
            await Task.WhenAll(userInfo, categories, subcategories, bidsDb);

            var categoryDBModel = categories.Result.FirstOrDefault(x => x.Id == failDbModel.CategoryId);
            var subcategoryDBModel = subcategories.Result.FirstOrDefault(x => x.Id == failDbModel.SubcategoryId);
            List<BidReadModel> bids = new List<BidReadModel>();
            var userInfoDbModels = await GetUserInfoDbModelsAsync(bidsDb.Result.Select(x => x.UserId).ToArray());
            foreach (var x in bidsDb.Result)
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
                    FailId = failDbModel.Id.ToString(),
                    UserInfo = new UserInfoReadModel(user)
                });
            }

            var read = new FailReadModel
            {
                Active = failDbModel.Active,
                AuctionValidThrough = failDbModel.AuctionValidThrough,
                Category = (categoryDBModel != null) ? new CategoryReadModel(categoryDBModel) : null,
                Subcategory = (subcategoryDBModel != null) ? new SubcategoryReadModel(subcategoryDBModel) : null,
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
                    Email = userInfo.Result.Email,
                    Company = userInfo.Result.Company,
                    MobileNumber = userInfo.Result.MobileNumber,
                    Name = userInfo.Result.Name,
                    RatingCount = userInfo.Result.RatingCount,
                    RatingSum = userInfo.Result.RatingSum,
                    Id = userInfo.Result.UserId
                },
                AssignedBid = bids.FirstOrDefault(x => x.Id == failDbModel.AssignedBidId.ToString())
            };
            return read;
        }

        public async Task<List<CategoryDbModel>> GetCategoriesAsync()
        {
            var collection = _mongoDb.GetCollection<CategoryDbModel>(CategoriesCollectionName);
            var result = await collection.AsQueryable().ToListAsync();
            return result;
        }
        public async Task<List<SubcategoryDbModel>> GetSubcategoriesAsync()
        {
            var collection = _mongoDb.GetCollection<SubcategoryDbModel>(SubcategoriesCollectionName);
            var result = await collection.AsQueryable().ToListAsync();
            return result;
        }

        public async Task<bool> AddCategoryAsync(CategoryDbModel cat)
        {
            await _mongoDb.GetCollection<CategoryDbModel>(CategoriesCollectionName).InsertOneAsync(cat);
            return true;
        }

        public async Task<bool> AddSubcategoryAsync(SubcategoryDbModel cat)
        {
            await _mongoDb.GetCollection<SubcategoryDbModel>(SubcategoriesCollectionName).InsertOneAsync(cat);
            return true;
        }

        // **** BIDS SECTION **** //

        public async Task<bool> AddBidAsync(BidDbModel bid)
        {
            var fail = await GetFailDbModelAsync(bid.FailId);
            if (fail == null)
                throw new FailDoesNotExistException();
            if (fail.AssignedBidId != ObjectId.Empty)
                throw new FailAlreadyAssignedException();
            if (fail.Active == false)
                throw new InactiveFailException();
            await _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).InsertOneAsync(bid);
            return true;
        }

        public async Task<List<BidDbModel>> GetBidDbModelsAsync()
        {
            var bids = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).AsQueryable().ToList();
            return bids;
        }

        public async Task<List<BidDbModel>> GetBidDbModelsAsync(ObjectId failId)
        {
            var bids = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).AsQueryable().Where(x=>x.FailId == failId).ToList();
            return bids;
        }

        public async Task<List<BidDbModel>> GetBidDbModelsAsync(string userId)
        {
            var bids = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).AsQueryable().Where(x => x.UserId == userId).ToList();
            return bids;
        }

        public async Task<List<BidReadModel>> GetBidReadModelsAsync(ObjectId failId)
        {
            var fail = GetFailDbModelAsync(failId);
            var bids = GetBidDbModelsAsync(failId);
            await Task.WhenAll(fail, bids);
            if (fail.Result == null)
                throw new FailDoesNotExistException();
            var userInfo = await GetUserInfoDbModelsAsync(bids.Result.Select(x => x.UserId).ToArray());
            var result = new List<BidReadModel>();
            foreach (var bid in bids.Result)
            {
                var user = userInfo.FirstOrDefault(x => x.UserId == bid.UserId);
                result.Add(new BidReadModel
                {
                    Active = bid.Active,
                    Assigned = bid.Assigned,
                    Budget = bid.Budget,
                    Description = bid.Description,
                    FailId = bid.FailId.ToString(),
                    Id = bid.Id.ToString(),
                    UserId = bid.UserId,
                    UserInfo = new UserInfoReadModel
                    {
                        Badges = user.Badges,
                        Company = user.Company,
                        Email = user.Email,
                        Id = user.UserId,
                        MobileNumber = user.MobileNumber,
                        Name = user.Name,
                        RatingCount = user.RatingCount,
                        RatingSum = user.RatingSum,
                        Ratings = user.Ratings
                    }
                });
            }
            return result;
        }

        public async Task<BidDbModel> GetBidAsync(ObjectId bidId)
        {
            var bid = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).AsQueryable().Where(x => x.Id == bidId).FirstOrDefault();
            return bid;
        }

        public async Task<bool> AcceptBid(ObjectId bidId, ObjectId failId, string userId)
        {
            var bidsCollection = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName);
            var failsCollection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName);

            var fail = GetFailDbModelAsync(failId);
            var bids = GetBidDbModelsAsync(failId);
            await Task.WhenAll(fail, bids);

            if (fail.Result == null)
                throw new FailDoesNotExistException();
            if (!bids.Result.Any(x=>x.Id == bidId))
                throw new BidDoesNotExistException();
            if (fail.Result.UserId != userId)
                throw new UserNotOwnerException();

            fail.Result.AssignedBidId = bidId;

            var updateBids = bids.Result.Select(x =>
            {
                if (x.Id == bidId)
                {
                    x.Active = true;
                    x.Assigned = true;
                }
                else
                {
                    x.Active = false;
                    x.Assigned = false;
                }
                return bidsCollection.FindOneAndReplaceAsync(y => y.Id == x.Id, x);
            });
            var updateFail = failsCollection.FindOneAndReplaceAsync(x => x.Id == failId, fail.Result);
            await Task.WhenAll(updateBids);
            
            return true;
        }

        public async Task<BlobUploadResult> UploadPictureAsync(byte[] buffer, string name = null)
        {
            // Create the blob client.
            CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);
            name = String.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);

            await blockBlob.UploadFromByteArrayAsync(buffer, 0, (int)buffer.Length);

            var result = new BlobUploadResult
            {
                FileName = blockBlob?.Name,
                Size = blockBlob?.Properties.Length,
                Uri = blockBlob?.Uri.ToString()
            };

            return result;
        }

        public async Task<bool> InsertPictureInfoDbModelAsync(PictureInfoDbModel model)
        {
            var bidsCollection = _mongoDb.GetCollection<PictureInfoDbModel>(PictureInfoCollectionName);
            await bidsCollection.InsertOneAsync(model);
            return true;
        }

        public async Task<List<PictureInfoDbModel>> GetPictureInfoDbModelsAsync(List<string> ids)
        {
            var bidsCollection = _mongoDb.GetCollection<PictureInfoDbModel>(PictureInfoCollectionName);
            var query = Builders<PictureInfoDbModel>.Filter.In(x => x.BaseName, ids);
            return await (await bidsCollection.FindAsync(query)).ToListAsync();
        }

        public async Task<List<CategoryReadModel>> GetCategoryReadModelsAsync()
        {
            var categories = GetCategoriesAsync();
            var subcategories = GetSubcategoriesAsync();
            await Task.WhenAll(categories, subcategories);

            var result = categories.Result.Select(x => new CategoryReadModel(x)).ToList();
            foreach (var sub in subcategories.Result)
            {
                var obj = result.FirstOrDefault(x => x.Id == sub.CategoryId.ToString());
                if (obj != null)
                {
                    if (obj.Subcategories == null)
                    {
                        obj.Subcategories = new List<SubcategoryReadModel>();
                    }
                    obj.Subcategories.Add(new SubcategoryReadModel(sub));
                }
            }
            return result;
        }
    }
}
