using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Zoo.Common;
using Zoo.Infrastructure;
using Zoo.Infrastructure.Models;
using System.Linq;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        string? connectionString = hostContext.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ZooContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped(typeof(ICrudServiceAsync<>), typeof(CrudServiceAsync<>));
    })
    .Build();

await RunDemo(host.Services);

Console.ReadLine();

static async Task RunDemo(IServiceProvider services)
{
    using (var scope = services.CreateScope())
    {
        var zooKeeperService = scope.ServiceProvider.GetRequiredService<ICrudServiceAsync<ZooKeeperModel>>();
        var animalService = scope.ServiceProvider.GetRequiredService<ICrudServiceAsync<AnimalModel>>();

        var newKeeper = new ZooKeeperModel
        {
            Id = Guid.NewGuid(),
            FullName = "Ярослав Жук",
            ExperienceYears = 8,
            Shift = "Денна"
        };

        await zooKeeperService.CreateAsync(newKeeper);
        Console.WriteLine($"Доглядач {newKeeper.FullName} доданий до БД.");

        Console.WriteLine("\nСтворення нової тварини ");
        var newAnimal = new MammalModel
        {
            Id = Guid.NewGuid(),
            Name = "Ведмідь Бурий",
            Age = 4,
            Weight = 250,
            ZooKeeperModelId = newKeeper.Id,
            FurColor = "Бурий",
            IsPredator = true,
            Habitat = "Ліс"
        };

        await animalService.CreateAsync(newAnimal);
        Console.WriteLine($"Тварина {newAnimal.Name} додана до БД");

        var allKeepers = await zooKeeperService.ReadAllAsync();
        Console.WriteLine($"Знайдено глядачів у БД {allKeepers.Count()}");
        foreach (var keeper in allKeepers)
        {
            Console.WriteLine($"ID: {keeper.Id} Ім'я: {keeper.FullName}");
        }

        var allAnimals = await animalService.ReadAllAsync();
        Console.WriteLine($"Знайдено тварин у БД {allAnimals.Count()}");
        foreach (var animal in allAnimals)
        {
            if (animal is MammalModel mammal)
            {
                 Console.WriteLine($"ID: {mammal.Id} Ім'я: {mammal.Name} Доглядач: {mammal.ZooKeeperModelId}");
            }
        }
    }
}