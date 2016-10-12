using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZreperujTo.Web.Models.CommonModels
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
