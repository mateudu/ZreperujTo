﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.DbModels;
using ZreperujTo.Web.Models.FailModels;
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Data
{
    public class ZreperujToDbClient
    {
        private IMongoClient _mongoClient;
        private IMongoDatabase _mongoDb;
        private ApplicationDbContext _db;

        private const string UserInfoCollectionName = "user_info";
        private const string BidsCollectionName = "bids";
        private const string FailsCollectionName = "fails";
        private const string CategoriesCollectionName = "categories";

        public ZreperujToDbClient(
            IMongoClient mongoClient,
            IMongoDatabase mongoDb,
            ApplicationDbContext db)
        {
            _mongoClient = mongoClient;
            _mongoDb = mongoDb;
            _db = db;
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

        public async Task<List<FailMetaModel>> GetFailsMetaAsync(int? categoryId,
            int? subcategoryId,
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
                .FindAsync(x => x.Active && x.AssignedBidId == null);
            var list = await collection.ToListAsync();
            list =
                list.Where(x => (categoryId.HasValue && categoryId.Value != 0) ? x.CategoryId == categoryId.Value : true
                      && (subcategoryId.HasValue && subcategoryId.Value != 0) ? x.SubcategoryId == subcategoryId.Value : true
                          && (!String.IsNullOrWhiteSpace(city)) ? x.Location.City.ToLower().Contains(city.ToLower()) : true
                              && (!String.IsNullOrWhiteSpace(district)) ? x.Location.District.ToLower().Contains(district.ToLower()) : true
                                  && (minPrice.HasValue && minPrice.Value != 0) ? x.Budget.MinimalPrice >= minPrice.Value : true
                                      && (maxPrice.HasValue && maxPrice.Value != 0) ? x.Budget.MaximalPrice >= maxPrice.Value : true
                                      ).ToList();
            switch (sortOrder)
            {
                case "maxprice_asc":
                    list =
                        list.OrderBy(x => x.Budget.MaximalPrice)
                            .ThenBy(x => x.Highlited)
                            .ToList();
                    break;
                case "maxprice_desc":
                    list =
                        list.OrderByDescending(x => x.Budget.MaximalPrice)
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
            // TODO: Add category 
            foreach (var e in list)
            {
                result.Add(new FailMetaModel
                {
                    Active = e.Active,
                    //Category = 
                    AuctionValidThrough = e.AuctionValidThrough,
                    Budget = e.Budget,
                    Description = e.Description,
                    FailId = e.Id.ToString(),
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

        public async Task<FailDbModel> GetFailDbModelAsync(ObjectId objId)
        {
            var collection = _mongoDb.GetCollection<FailDbModel>(FailsCollectionName);
            var result = await (await collection.FindAsync(x => x.Id == objId)).FirstOrDefaultAsync();
            return result;
        }
    }
}