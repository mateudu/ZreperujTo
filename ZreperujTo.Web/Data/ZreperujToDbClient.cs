using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ZreperujTo.Web.Models.DbModels;
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

        public async Task<UserInfoDbModel> GetUserInfoDbModel(string userId)
        {
             var res = await (await _mongoDb.GetCollection<UserInfoDbModel>(UserInfoCollectionName).FindAsync(x=>x.UserId == userId)).FirstOrDefaultAsync();
            return res;
        }
    }
}
