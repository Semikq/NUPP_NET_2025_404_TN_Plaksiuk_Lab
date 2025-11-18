namespace Zoo.Infrastructure.Models
{
    public class BirdModel : AnimalModel
    {
        public double WingSpan { get; set; }
        public bool CanFly { get; set; }
        public string BeakType { get; set; } = string.Empty;
    }
}