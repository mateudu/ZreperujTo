using System.ComponentModel.DataAnnotations;

namespace ZreperujTo.UWP.Models.CommonModels
{
    public class Rating
    {
        public string UserId { get; set; }
        public string BidId { get; set; }
        [Range(0,10)]
        public decimal Points { get; set; }
        public string Description { get; set; }
    }
}
