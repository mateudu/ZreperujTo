using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Models.CategoryModels
{
    public class CategoryReadModel
    {
        public CategoryReadModel(CategoryDbModel model)
        {
            this.Id = model.Id.ToString();
            this.Name = model.Name;
        }

        public CategoryReadModel()
        {
            
        }

        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public List<SubcategoryReadModel> Subcategories { get; set; }
    }
}
