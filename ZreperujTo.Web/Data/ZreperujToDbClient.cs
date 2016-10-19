using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MongoDB.Bson;
using MongoDB.Driver;
using ZreperujTo.Web.Controllers.Api;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.CategoryModels;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.DbModels;
using ZreperujTo.Web.Models.FailModels;
using ZreperujTo.Web.Models.FileInfoModels;
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Data
{
    public class ZreperujToDbClient
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

        public async Task<List<UserInfoDbModel>> GetUserInfoDbModelAsync(params string[] userIds)
        {
            var collection = _mongoDb.GetCollection<UserInfoDbModel>(UserInfoCollectionName);
            var filter = Builders<UserInfoDbModel>.Filter.In(x => x.UserId, userIds);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<FailDbModel> InsertFailDbModelAsync(FailDbModel model)
        {
            var collection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName);
            await collection.InsertOneAsync(model);
            var result =
                await
                    (await collection.FindAsync(x => x.UserId == model.UserId && x.CreatedAt == model.CreatedAt))
                        .FirstOrDefaultAsync();
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
            var collection = await _mongoDb.GetCollection<FailDbModel>(FailsCollectionName)
                .FindAsync(x => x.Active && (x.AssignedBidId == ObjectId.Empty));
            var list = await collection.ToListAsync();
            list =
                list.Where(x => (categoryId.HasValue && categoryId.Value != ObjectId.Empty) ? x.CategoryId == categoryId.Value : true
                      && (subcategoryId.HasValue && subcategoryId.Value != ObjectId.Empty) ? x.SubcategoryId == subcategoryId.Value : true
                          && (!String.IsNullOrWhiteSpace(city)) ? x.Location.City.ToLower().Contains(city.ToLower()) : true
                              && (!String.IsNullOrWhiteSpace(district)) ? x.Location.District.ToLower().Contains(district.ToLower()) : true
                                  && (minPrice.HasValue && minPrice.Value != 0) ? x.Budget >= minPrice.Value : true
                                      && (maxPrice.HasValue && maxPrice.Value != 0) ? x.Budget >= maxPrice.Value : true
                                      ).ToList();
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
            var categories = await GetCategoriesAsync();
            var subcategories = await GetSubcategoriesAsync();
            
            foreach (var e in list)
            {
                var category = categories.FirstOrDefault(x => x.Id == e.CategoryId);
                var subcategory = subcategories.FirstOrDefault(x => x.Id == e.SubcategoryId);

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

        public async Task<List<FailMetaModel>> GetFailsMetaAsync(string userId)
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

        public async Task<bool> AddBidAsync(BidDbModel bid)
        {
            await _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).InsertOneAsync(bid);
            return true;
        }

        public async Task<List<BidDbModel>> GetBidsAsync()
        {
            var bids = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).AsQueryable().ToList();
            return bids;
        }

        public async Task<List<BidDbModel>> GetBidsAsync(ObjectId failId)
        {
            var bids = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).AsQueryable().Where(x=>x.FailId == failId).ToList();
            return bids;
        }

        public async Task<List<BidDbModel>> GetBidsAsync(string userId)
        {
            var bids = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).AsQueryable().Where(x => x.UserId == userId).ToList();
            return bids;
        }

        public async Task<BidDbModel> GetBidAsync(ObjectId bidId)
        {
            var bid = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName).AsQueryable().Where(x => x.Id == bidId).FirstOrDefault();
            return bid;
        }

        public async Task<bool> AcceptBid(ObjectId bidId, ObjectId failId)
        {
            var bidsCollection = _mongoDb.GetCollection<BidDbModel>(BidsCollectionName);
            var failsCollection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName);

            var fail = GetFailDbModelAsync(failId);
            var bids = GetBidsAsync(failId);
            await Task.WhenAll(fail, bids);

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

        public async Task<BlobUploadResult> UploadToBlobAsync(byte[] buffer, string name = null)
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
    }
}
