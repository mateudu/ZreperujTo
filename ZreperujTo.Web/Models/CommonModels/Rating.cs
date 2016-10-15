using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZreperujTo.Web.Models.CommonModels
{
    public class Rating
    {
        public string UserId { get; set; }
        public string BidId { get; set; }
        [Range(0,10)]
        public decimal Points { get; set; }
        public string Description { get; set; }
    }
}
