using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.CategoryModels;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.DbModels;
using ZreperujTo.Web.Models.FailModels;
using ZreperujTo.Web.Models.FileInfoModels;

namespace ZreperujTo.Web.Helpers
{
    public interface IZreperujToService
    {
        Task<bool> AddUserInfoAsync(UserInfoDbModel model);
        Task<UserInfoDbModel> GetUserInfoDbModelAsync(string userId);
        Task<List<UserInfoDbModel>> GetUserInfoDbModelsAsync(params string[] userIds);

        Task<FailReadModel> InsertFailDbModelAsync(FailDbModel model, List<string> pictureIds = null);
        Task<List<FailMetaModel>> GetFailsMetaAsync(
            ObjectId? categoryId, 
            ObjectId? subcategoryId,
            string city,
            string district,
            decimal? minPrice,
            decimal? maxPrice,
            DateTime? validThrough,
            List<SpecialRequirement> requirements,
            string sortOrder,
            int pageLimit = 10,
            int pageNumber = 1);

        Task<List<FailMetaModel>> GetUserFailsMetaAsync(string userId);
        Task<FailReadModel> GetFailReadModelAsync(ObjectId objId);
        Task<FailDbModel> GetFailDbModelAsync(ObjectId objId);

        Task<List<CategoryDbModel>> GetCategoriesAsync();
        Task<List<SubcategoryDbModel>> GetSubcategoriesAsync();
        Task<bool> AddCategoryAsync(CategoryDbModel cat);
        Task<bool> AddSubcategoryAsync(SubcategoryDbModel cat);
        Task<List<CategoryReadModel>> GetCategoryReadModelsAsync();

        Task<bool> AddBidAsync(BidDbModel bid);
        Task<List<BidDbModel>> GetBidDbModelsAsync();
        Task<List<BidDbModel>> GetBidDbModelsAsync(ObjectId failId);
        Task<List<BidDbModel>> GetBidDbModelsAsync(string userId);
        Task<List<BidReadModel>> GetBidReadModelsAsync(ObjectId failId);
        Task<BidDbModel> GetBidAsync(ObjectId bidId);
        Task<bool> AcceptBid(ObjectId bidId, ObjectId failId, string userId);

        Task<BlobUploadResult> UploadPictureAsync(byte[] buffer, string name = null);
        Task<bool> InsertPictureInfoDbModelAsync(PictureInfoDbModel model);
        Task<List<PictureInfoDbModel>> GetPictureInfoDbModelsAsync(List<string> ids);
    }
    public class InvalidCategoryException : Exception { }
    public class InvalidSubcategoryException : Exception { }
    public class UserDoesNotExistException : Exception { }
    public class FailDoesNotExistException : Exception { }
    public class FailAlreadyAssignedException : Exception { }
    public class InactiveFailException : Exception { }
    public class BidDoesNotExistException : Exception { }
    public class UserNotOwnerException : Exception { }
}
