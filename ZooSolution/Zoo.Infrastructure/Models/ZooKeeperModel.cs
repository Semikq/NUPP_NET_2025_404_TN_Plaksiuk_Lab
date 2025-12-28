using System.Collections.Generic;

namespace Zoo.Infrastructure.Models
{
    public class ZooKeeperModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public string Shift { get; set; } = string.Empty;

        public ICollection<AnimalModel> Animals { get; set; } = new List<AnimalModel>();
    }
}


