using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.Common
{
    public class Bird : Animal
    {
        public double WingSpan { get; set; }
        public bool CanFly { get; set; }
        public string BeakType { get; set; }

        // статистичне поле
        public static int TotalBirds;

        // статистичний конструктор
        static Bird()
        {
            TotalBirds = 0;
        }

        // конструктор
        public Bird(string name, int age, double weight, double wingSpan, bool canFly, string beakType)
            : base(name, age, weight)
        {
            WingSpan = wingSpan;
            CanFly = canFly;
            BeakType = beakType;
            TotalBirds++;
        }
    }
}
