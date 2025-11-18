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
        public string FullName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string Shift { get; set; } = string.Empty;

        private List<Animal> _animals = new List<Animal>();
        public IReadOnlyCollection<Animal> Animals => _animals.AsReadOnly();

        public event Action<ZooKeeper, Animal>? OnFeedAnimals;

        public ZooKeeper(string fullName, int experienceYears, string shift)
        {
            FullName = fullName;
            ExperienceYears = experienceYears;
            Shift = shift;
        }

        public void FeedAnimals()
        {
            foreach (var animal in _animals)
            {
                OnFeedAnimals?.Invoke(this, animal);
            }
        }
    }
}
