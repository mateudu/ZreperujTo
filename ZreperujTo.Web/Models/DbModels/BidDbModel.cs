using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Models.DbModels
{
    public class BidDbModel
    {
        public int Id { get; set; }
        public string FailId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public bool Assigned { get; set; }
        public bool Active { get; set; }
        public Budget Budget { get; set; }
    }
}
