using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZreperujTo.UWP.Models.CommonModels;

namespace ZreperujTo.UWP.Models.FailModels
{
    public class FailWriteModel
    {
        [Required]
        public string CategoryId { get; set; }
        [Required]
        public string SubcategoryId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AuctionValidThrough { get; set; }
        public List<string> PictureIds { get; set; }
        [Required]
        public LocationInfo Location { get; set; }
        [Required]
        public decimal Budget { get; set; }
        public List<SpecialRequirement> Requirements { get; set; }
        public bool Highlited { get; set; }
        // TODO: Highlited enum
    }
    
}
