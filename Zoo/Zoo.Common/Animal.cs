namespace Zoo.Common
{
    public abstract class Animal : IFeedable, ICloneable
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public double Weight { get; set; }

        public Animal()
        {
            Id = Guid.NewGuid();
        }

        public Animal(string name, int age, double weight)
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
            Weight = weight;
        }

        public abstract void MakeSound();
        public abstract void Eat();
        public abstract void Sleep();
        public abstract void Feed();
        public abstract object Clone();
    }
}