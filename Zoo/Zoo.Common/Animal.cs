namespace Zoo.Common
{
    public class Animal
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }

        // порожній конструктор
        public Animal()
        {
            Id = Guid.NewGuid();
        }

        // конструктор з параметрами
        public Animal(string name, int age, double weight)
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
            Weight = weight;
        }
    }
}