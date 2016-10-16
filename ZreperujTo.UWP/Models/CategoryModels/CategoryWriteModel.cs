using System.ComponentModel.DataAnnotations;

namespace ZreperujTo.UWP.Models.CategoryModels
{
    public class CategoryWriteModel
    {
        [Required]
        public string Name { get; set; }
    }
}
