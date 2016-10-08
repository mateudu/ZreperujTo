﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.FailModels;

namespace ZreperujTo.Web.Models.CommonModels
{
    public class LocationInfo
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Apartment { get; set; }
        public string AdditionalInfo { get; set; }
    }
}