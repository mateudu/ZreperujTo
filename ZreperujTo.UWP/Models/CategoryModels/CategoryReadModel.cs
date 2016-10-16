using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZreperujTo.UWP.Models.CategoryModels
{
    public class CategoryReadModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<SubcategoryReadModel> Subcategories { get; set; }
    }
}
