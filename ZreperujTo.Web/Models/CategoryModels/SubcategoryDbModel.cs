using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ZreperujTo.Web.Models.CommonModels
{
    public class SubcategoryDbModel
    {
        public ObjectId Id { get; set; }
        public ObjectId CategoryId { get; set; }
        public string Name { get; set; }
    }
}
