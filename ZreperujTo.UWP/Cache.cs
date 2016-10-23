using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZreperujTo.UWP.Models.CategoryModels;

namespace ZreperujTo.UWP
{
    class Cache
    {
        public static List<CategoryReadModel> Categories { get; set; }
        public static List<SubcategoryReadModel> Subcategories { get; set; }
    }
}
