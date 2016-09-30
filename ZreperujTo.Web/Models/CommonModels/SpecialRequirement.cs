using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZreperujTo.Web.Models.CommonModels
{
    public enum SpecialRequirement : int
    {
        CompanyOnly = 1,
        BronzeOrMore = 2,
        SilverOrMore = 3,
        GoldenOrMore = 4
    }
}
