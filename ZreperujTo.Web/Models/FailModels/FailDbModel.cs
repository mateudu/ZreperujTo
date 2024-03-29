﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using ZreperujTo.Web.Models.CommonModels;
using ZreperujTo.Web.Models.FileInfoModels;

namespace ZreperujTo.Web.Models.DbModels
{
    public class FailDbModel
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public ObjectId CategoryId { get; set; }
        public ObjectId SubcategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AuctionValidThrough { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PictureInfoReadModel> Pictures { get; set; }
        public LocationInfo Location { get; set; }
        public decimal Budget { get; set; }
        public bool Highlited { get; set; }
        public bool Active { get; set; }
        public ObjectId AssignedBidId { get; set; }
        public List<SpecialRequirement> Requirements { get; set; }
    }
}
