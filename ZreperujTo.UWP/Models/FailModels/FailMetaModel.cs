using System;
using System.Collections.Generic;
using ZreperujTo.UWP.Models.CategoryModels;
using ZreperujTo.UWP.Models.CommonModels;

namespace ZreperujTo.UWP.Models.FailModels
{
    public class FailMetaModel
    {
        public string FailId { get; set; }
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
    }
}
