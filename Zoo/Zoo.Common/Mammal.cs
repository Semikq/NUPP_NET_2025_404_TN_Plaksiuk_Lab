using System;

namespace Zoo.Common
{
    public class Mammal : Animal
    {
        public string FurColor { get; set; }
        public bool IsPredator { get; set; }
        public string Habitat { get; set; }

        public Mammal() : base() { }

        public Mammal(string name, int age, double weight, string furColor, bool isPredator, string habitat)
            : base(name, age, weight)
        {
            FurColor = furColor;
            IsPredator = isPredator;
            Habitat = habitat;
        }

        public void Eat(string food)
        {
            Console.WriteLine($"{Name} їсть {food}");
        }

        private static Random _rand = new Random();
        private static string[] _names = { "Лев", "Тигр", "Ведмідь", "Вовк", "Лисиця", "Олень", "Зебра" };
        private static string[] _colors = { "Коричневий", "Білий", "Чорний", "Рудий", "Плямистий" };
        private static string[] _habitats = { "Саванна", "Ліс", "Тундра", "Джунглі", "Гори" };

        public static Mammal CreateNew()
        {
            return new Mammal(
                _names[_rand.Next(_names.Length)],
                _rand.Next(1, 20),
                _rand.NextDouble() * 200 + 50,
                _colors[_rand.Next(_colors.Length)],
                _rand.Next(0, 2) == 1,
                _habitats[_rand.Next(_habitats.Length)]
            );
        }
    }
}