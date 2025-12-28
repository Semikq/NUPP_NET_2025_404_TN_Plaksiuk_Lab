namespace Zoo.Infrastructure.Models
{
    public class FoodModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;

        public Guid AnimalModelId { get; set; }
        public AnimalModel? Animal { get; set; }
    }
}


