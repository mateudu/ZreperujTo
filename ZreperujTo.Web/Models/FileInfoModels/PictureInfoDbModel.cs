using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace ZreperujTo.Web.Models.FileInfoModels
{
    public class PictureInfoDbModel
    {
        public ObjectId Id { get; set; }
        public string BaseName { get; set; }
        public string OriginalSizeUri { get; set; }
        public string ThumbnailUri { get; set; }
    }
}