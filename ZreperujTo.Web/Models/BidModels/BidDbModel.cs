﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using ZreperujTo.Web.Models.CommonModels;

namespace ZreperujTo.Web.Models.BidModels
{
    public class BidDbModel
    {
        public ObjectId Id { get; set; }
        public ObjectId FailId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public bool Assigned { get; set; }
        public bool Active { get; set; }
        public decimal Budget { get; set; }
    }
}
