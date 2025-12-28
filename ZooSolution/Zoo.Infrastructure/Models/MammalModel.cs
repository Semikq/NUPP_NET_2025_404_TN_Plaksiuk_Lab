namespace Zoo.Infrastructure.Models
{
    public class MammalModel : AnimalModel
    {
        public string FurColor { get; set; } = string.Empty;
        public bool IsPredator { get; set; }
        public string Habitat { get; set; } = string.Empty;
    }
}


