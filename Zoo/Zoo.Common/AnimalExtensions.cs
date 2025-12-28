using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoo.Common
{
    public static class AnimalExtensions
    {
        // метод розширення
        public static void DisplayInfo(this Animal animal)
        {
            Console.WriteLine($"Тварина: {animal.Name}, Вік: {animal.Age}, Вага: {animal.Weight}");
        }
    }
}
