namespace Zoo.Infrastructure.Models
{
    public class FoodModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public Guid AnimalModelId { get; set; }
        public AnimalModel Animal { get; set; } = null!;
    }
}