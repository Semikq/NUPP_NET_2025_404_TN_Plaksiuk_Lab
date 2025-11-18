namespace Zoo.Infrastructure.Models
{
    public class AnimalModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Weight { get; set; }

        public FoodModel Food { get; set; } = null!;

        public Guid ZooKeeperModelId { get; set; }
        public ZooKeeperModel ZooKeeper { get; set; } = null!;
    }
}