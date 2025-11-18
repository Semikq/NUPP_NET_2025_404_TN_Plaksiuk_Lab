using System;

namespace Zoo.Common
{
    public class Bird : Animal
    {
        public double WingSpan { get; set; }
        public bool CanFly { get; set; }
        public string BeakType { get; set; }
        public static int TotalBirds;
        static Bird() { TotalBirds = 0; }

        public Bird(string name, int age, double weight, double wingSpan, bool canFly, string beakType)
            : base(name, age, weight)
        {
            WingSpan = wingSpan;
            CanFly = canFly;
            BeakType = beakType;
            TotalBirds++;
        }

        private static Random _rand = new Random();
        private static string[] _names = { "Орел", "Горобець", "Пінгвін", "Сова", "Папуга" };
        private static string[] _beakTypes = { "Гачкуватий", "Конічний", "Зондуючий", "Всеїдний" };

        public static Bird CreateNew()
        {
            bool canFly = _rand.Next(0, 2) == 1;
            string name = _names[_rand.Next(_names.Length)];
            
            // Типу пінгвіни не літають
            if (name == "Пінгвін") canFly = false; 

            return new Bird(
                name,
                _rand.Next(1, 10),
                _rand.NextDouble() * 5 + 1, 
                _rand.NextDouble() * 1.5 + 0.5, 
                canFly,
                _beakTypes[_rand.Next(_beakTypes.Length)]
            );
        }

        public override void MakeSound()
        {
            Console.WriteLine("Bird chirps");
        }

        public override void Eat()
        {
            Console.WriteLine($"{Name} їсть.");
        }

        public override void Sleep()
        {
            Console.WriteLine($"{Name} спить.");
        }

        public override void Feed()
        {
            Console.WriteLine($"{Name} харчується.");
        }

        public override object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}