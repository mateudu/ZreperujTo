﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZreperujTo.Web.Models.UserInfoModels
{
    public class UserInfoMetaModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public bool Company { get; set; }
        public int RatingCount { get; set; }
        public int RatingSum { get; set; }
    }
}
