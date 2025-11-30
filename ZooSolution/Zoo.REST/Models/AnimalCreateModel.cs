using System.ComponentModel.DataAnnotations;

namespace Zoo.REST.Models
{
    public class AnimalCreateModel
    {
        [Required]
        public string Species { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(1, 150)]
        public int Age { get; set; }
    }
}