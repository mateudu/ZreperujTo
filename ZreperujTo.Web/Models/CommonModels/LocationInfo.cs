using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string City { get; set; }
        [Required]
        public string PostalCode { get; set; }
        public string District { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string Number { get; set; }
        public string Apartment { get; set; }
        public string AdditionalInfo { get; set; }

        public LocationInfo TrimStrings()
        {
            var obj = new LocationInfo();
            foreach (var prop in this.GetType().GetProperties().Where(x => x.PropertyType == typeof(string)))
            {
                var value = prop.GetValue(this) as string;
                if (!String.IsNullOrWhiteSpace(value))
                {
                    value = value.Trim();
                    prop.SetValue(obj, value);
                }
            }
            return obj;
        }
    }
}
