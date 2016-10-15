using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZreperujTo.Web.Models.CategoryModels
{
    public class CategoryWriteModel
    {
        [Required]
        public string Name { get; set; }
    }
}
