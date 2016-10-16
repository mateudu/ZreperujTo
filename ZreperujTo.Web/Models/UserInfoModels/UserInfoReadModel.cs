using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.DbModels;

namespace ZreperujTo.Web.Models.UserInfoModels
{
    public class UserInfoReadModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        //public int Points { get; set; }
        public bool Company { get; set; }
        public int RatingCount { get; set; }
        public int RatingSum { get; set; }
        public List<Rating> Ratings { get; set; }
        public List<Badge> Badges { get; set; }

        public UserInfoReadModel(UserInfoDbModel dbModel)
        {

            Badges = dbModel.Badges;
            Company = dbModel.Company;
            Email = dbModel.Email;
            MobileNumber = dbModel.MobileNumber;
            Name = dbModel.Name;
            Ratings = dbModel.Ratings;
            RatingCount = dbModel.RatingCount;
            RatingSum = dbModel.RatingSum;
            Id = dbModel.UserId;
        }

        public UserInfoReadModel()
        {
            
        }
    }
}
