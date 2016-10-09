﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZreperujTo.Web.Models.CommonModels
{
    public class UserInfo
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int Points { get; set; }
        public bool Company { get; set; }
        public int RatingCount { get; set; }
        public int RatingSum { get; set; }
        public List<Rating> Ratings { get; set; }
        public List<Badge> Badges { get; set; }
    }
}
