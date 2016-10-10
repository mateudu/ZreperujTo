using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public LocationInfo TrimStrings()
        {
            var obj = new LocationInfo();
            foreach (var prop in this.GetType().GetProperties().Where(x => x.PropertyType == typeof(string)))
            {
                var value = prop.GetValue(this) as string;
                value = value.Trim();
                prop.SetValue(obj, value);
            }
            return obj;
        }
    }
}
