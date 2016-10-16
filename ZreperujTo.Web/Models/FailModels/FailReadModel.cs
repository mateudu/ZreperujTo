using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.CategoryModels;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.UserInfoModels;

namespace ZreperujTo.Web.Models.FailModels
{
    public class FailReadModel
    {
        public string Id { get; set; }
        public CategoryReadModel Category { get; set; }
        public SubcategoryReadModel Subcategory { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AuctionValidThrough { get; set; }
        public List<string> Pictures { get; set; }
        public LocationInfo Location { get; set; }
        public decimal Budget { get; set; }
        public List<SpecialRequirement> Requirements { get; set; }
        public bool Highlited { get; set; }
        public bool Active { get; set; }
        public UserInfoMetaModel UserInfo { get; set; }
        public List<BidReadModel> Bids { get; set; }
        public BidReadModel AssignedBid { get; set; }
    }
}
