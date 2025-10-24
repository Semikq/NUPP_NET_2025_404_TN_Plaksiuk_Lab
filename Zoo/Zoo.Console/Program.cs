using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zoo.Common;

class Program
{
    static async Task Main()
    {
        string filePath = "mammals_collection.json";
        var mammalService = await CrudServiceAsync<Mammal>.CreateAndLoadAsync(filePath);
        Console.WriteLine($"Завантажено {mammalService.Count()} ссавців з файлу '{filePath}");

        int numToCreate = 1000;
        Console.WriteLine($"Створення {numToCreate} нових ссавців");

        var sw = Stopwatch.StartNew();
        Parallel.For(0, numToCreate, i =>
        { mammalService.CreateAsync(Mammal.CreateNew()).Wait(); });
        sw.Stop();

        Console.WriteLine($"Сворено {numToCreate} ссавців за {sw.ElapsedMilliseconds} мс.");

        var allMammals = await mammalService.ReadAllAsync();
        Console.WriteLine($"Загальна кількість сервісі: {allMammals.Count()}");

        if (allMammals.Any())
        {
            double minAge = allMammals.Min(m => m.Age);
            double maxAge = allMammals.Max(m => m.Age);
            double avgAge = allMammals.Average(m => m.Age);
            Console.WriteLine($"Вік: \t Мін: {minAge:F0}, \t Макс: {maxAge:F0}, \t Середнє: {avgAge:F2}");

            double minWeight = allMammals.Min(m => m.Weight);
            double maxWeight = allMammals.Max(m => m.Weight);
            double avgWeight = allMammals.Average(m => m.Weight);
            Console.WriteLine($"Вага: \t Мін: {minWeight:F2}, \t Макс: {maxWeight:F2}, \t Середнє: {avgWeight:F2}");
        }

        await mammalService.SaveAsync();

        var page2 = await mammalService.ReadAllAsync(page: 2, amount: 5);
        foreach (var mammal in page2)
        {
            Console.WriteLine($"  > ID: {mammal.Id}, Ім'я: {mammal.Name}, Вік: {mammal.Age}");
        }
        SyncPrimitivesDemo.RunAllDemos(); 
    }
} 

public static class SyncPrimitivesDemo
{
    private static int _counter = 0;
    private static readonly object _lockObject = new object();

    private static void DemoLock()
    {
        Task.Run(() => IncrementCounter());
        Task.Run(() => IncrementCounter());
        Task.Run(() => IncrementCounter());

        Thread.Sleep(500);
        Console.WriteLine($"Фінальний лічильник: {_counter}");
    }

    private static void IncrementCounter()
    {
        for (int i = 0; i < 10000; i++)
        {
            lock (_lockObject)
            {
                _counter++;
            }
        }
    }
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(2, 2);

    private static async Task DemoSemaphore()
    {
        var tasks = new Task[5];
        for (int i = 1; i <= 5; i++)
        {
            tasks[i - 1] = AccessResource(i);
        }
        await Task.WhenAll(tasks);
    }

    private static async Task AccessResource(int id)
    {
        await _semaphore.WaitAsync();
        try{ await Task.Delay(1000); }
        finally{ _semaphore.Release(); }
    }

    private static AutoResetEvent _are = new AutoResetEvent(false); 
    private static string _data = null;

    private static void DemoAutoResetEvent()
    {
        var consumer = Task.Run(() =>
        {_are.WaitOne(); });

        var producer = Task.Run(async () =>
        { await Task.Delay(1000); _are.Set(); });

        Task.WaitAll(consumer, producer);
    }

    private static Mutex _mutex = new Mutex();
    private static void DemoMutex()
    {
        Task.Run(() => UseMutexResource("Task A"));
        Task.Run(() => UseMutexResource("Task B"));
        Thread.Sleep(1500); 
    }

    private static void UseMutexResource(string taskName)
    {
        Console.WriteLine($"[{taskName}] Чекає на м'ютекс");
        _mutex.WaitOne(); 
        
        Console.WriteLine($"[{taskName}] Захопив м'ютекс, працює");
        Thread.Sleep(500);
        
        _mutex.ReleaseMutex(); 
        Console.WriteLine($"[{taskName}] Звільнив м'ютекс");
    }

    public static void RunAllDemos()
    {
        DemoLock();
        DemoSemaphore().Wait();
        DemoAutoResetEvent();
        DemoMutex();
    }
}