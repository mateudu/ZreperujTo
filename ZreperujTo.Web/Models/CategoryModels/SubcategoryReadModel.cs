using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Models.CategoryModels
{
    public class SubcategoryReadModel
    {
        public SubcategoryReadModel(SubcategoryDbModel model)
        {
            this.Id = model.Id.ToString();
            this.CategoryId = model.CategoryId.ToString();
            this.Name = model.Name;
        }

        public SubcategoryReadModel()
        {
            
        }

        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string Name { get; set; }
    }
}
