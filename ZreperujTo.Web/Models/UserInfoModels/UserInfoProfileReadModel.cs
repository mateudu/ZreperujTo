using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZreperujTo.Web.Models.BidModels;
using ZreperujTo.Web.Models.FailModels;

namespace ZreperujTo.Web.Models.UserInfoModels
{
    public class UserInfoProfileReadModel
    {
        public UserInfoReadModel User { get; set; }
        public List<KeyValuePair<BidReadModel, FailMetaModel>> BidsAndFails { get; set; }
        public List<FailMetaModel> UserFails { get; set; }
    }
}
