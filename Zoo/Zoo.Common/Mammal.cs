namespace Zoo.Common
{
    public class Mammal : Animal
    {
        public string FurColor { get; set; }
        public bool IsPredator { get; set; }
        public string Habitat { get; set; }

        public void Eat(string food)
        {
            Console.WriteLine($"{Name} їсть {food}");
        }
    }
}