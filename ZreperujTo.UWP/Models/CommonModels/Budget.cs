using System.ComponentModel.DataAnnotations;

namespace ZreperujTo.UWP.Models.CommonModels
{
    public class Budget
    {
        [Required]
        public BudgetType Type { get; set; }
        public decimal MinimalPrice { get; set; }
        [Required]
        public decimal MaximalPrice { get; set; }
    }

    public enum BudgetType : int
    {
        HourlyRate = 1,
        FixedPrice = 2
    }
}
