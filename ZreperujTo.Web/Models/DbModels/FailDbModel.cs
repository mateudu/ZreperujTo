using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Models.DbModels
{
    public class FailDbModel
    {
        // TODO: Replace for MONGODB ID
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AuctionValidThrough { get; set; }
        public List<string> Pictures { get; set; }
        public LocationInfo Location { get; set; }
        public Budget Budget { get; set; }
        public bool Highlited { get; set; }
        public bool Active { get; set; }
        public string AssignedBidId { get; set; }
        public List<SpecialRequirement> Requirements { get; set; }
    }
}
