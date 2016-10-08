﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Models.FailModels
{
    public class FailWriteModel
    {
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AuctionValidThrough { get; set; }
        public List<string> Pictures { get; set; }
        public LocationInfo Location { get; set; }
        public Budget Budget { get; set; }
        public List<SpecialRequirement> Requirements { get; set; }
        // TODO: Highlited enum
    }
    
}
