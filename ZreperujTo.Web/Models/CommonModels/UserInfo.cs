using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZreperujTo.Web.Models.CommonModels
{
    public class UserInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int Points { get; set; }
        public bool Company { get; set; }
    }
}
