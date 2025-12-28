using System;
using Zoo.Common;

class Program
{
    static void Main()
    {
        // Створюємо CRUD сервіс для Animal
        var animalService = new CrudService<Animal>();

        // Створюємо об'єкт
        var lion = new Animal("Лев", 5, 200);
        animalService.Create(lion);

        // Читаємо об'єкт
        var readLion = animalService.Read(lion.Id);
        Console.WriteLine($"Знайдено: {readLion.Name}, Вік: {readLion.Age}, Вага: {readLion.Weight}");

        // Оновлюємо об'єкт
        lion.Age = 6;
        animalService.Update(lion);

        // Видаляємо об'єкт
        animalService.Remove(lion);

        // Виводимо кількість об'єктів після видалення
        Console.WriteLine($"Кількість тварин після видалення: {animalService.ReadAll().Count()}");
    }
}