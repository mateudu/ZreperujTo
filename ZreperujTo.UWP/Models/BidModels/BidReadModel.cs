using ZreperujTo.UWP.Models.UserInfoModels;

namespace ZreperujTo.UWP.Models.BidModels
{
    public class BidReadModel
    {
        public string Id { get; set; }
        public string FailId { get; set; }
        public string UserId { get; set; }
        public UserInfoReadModel UserInfo { get; set; }
        public string Description { get; set; }
        public bool Assigned { get; set; }
        public bool Active { get; set; }
        public decimal Budget { get; set; }
    }
}
