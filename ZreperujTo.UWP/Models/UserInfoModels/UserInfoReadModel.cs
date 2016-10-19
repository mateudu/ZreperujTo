using System.Collections.Generic;
using ZreperujTo.UWP.Models.CommonModels;

namespace ZreperujTo.UWP.Models.UserInfoModels
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

    }
}
