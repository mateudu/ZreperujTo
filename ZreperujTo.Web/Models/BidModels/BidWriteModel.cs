using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Models.BidModels
{
    public class BidWriteModel
    {
        public string FailId { get; set; }
        public string Description { get; set; }
        public decimal Budget { get; set; }
    }
}
