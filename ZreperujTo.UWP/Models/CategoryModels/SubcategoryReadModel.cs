namespace ZreperujTo.UWP.Models.CategoryModels
{
    public class SubcategoryReadModel
    {
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
