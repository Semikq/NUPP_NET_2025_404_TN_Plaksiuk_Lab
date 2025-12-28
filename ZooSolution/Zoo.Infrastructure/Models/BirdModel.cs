namespace Zoo.Infrastructure.Models
{
    public class BirdModel : AnimalModel
    {
        public double Wingspan { get; set; }
        public bool CanFly { get; set; }
        public string FeatherColor { get; set; } = string.Empty;
    }
}


