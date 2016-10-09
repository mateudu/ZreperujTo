using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Models.DbModels
{
    public class UserDbModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int ReportCount { get; set; }
        public bool Company { get; set; }
        public int RatingCount { get; set; }
        public int RatingSum { get; set; }
        public List<Rating> Ratings { get; set; }
        public List<Badge> Badges { get; set; }
    }
}
