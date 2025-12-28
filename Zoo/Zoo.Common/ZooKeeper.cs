using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.Common
{
    public class ZooKeeper
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public int ExperienceYears { get; set; }
        public string Shift { get; set; }

        // делегат
        public delegate void FeedAnimalsHandler(string message);

        // подія
        public event FeedAnimalsHandler OnFeedAnimals;

        // метод для виклику події
        public void FeedAnimals()
        {
            OnFeedAnimals?.Invoke($"{FullName} годує тварин на зміні {Shift}");
        }
    }
}
