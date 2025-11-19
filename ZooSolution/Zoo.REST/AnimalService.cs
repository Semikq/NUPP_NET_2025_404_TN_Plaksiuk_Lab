using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
public class AnimalService : ICrudServiceAsync<Animal>
{
    private readonly List<Animal> _animals = new List<Animal>();

    public Task<bool> CreateAsync(Animal element)
    {
        element.Id = Guid.NewGuid();
        _animals.Add(element);
        return Task.FromResult(true);
    }
    public Task<Animal> ReadAsync(Guid id) =>
        Task.FromResult(_animals.FirstOrDefault(a => a.Id == id));

    public Task<IEnumerable<Animal>> ReadAllAsync() =>
        Task.FromResult<IEnumerable<Animal>>(_animals);

    public Task<IEnumerable<Animal>> ReadAllAsync(int page, int amount)
    {
        var result = _animals.Skip((page - 1) * amount).Take(amount);
        return Task.FromResult<IEnumerable<Animal>>(result);
    }
    public Task<bool> UpdateAsync(Animal element)
    {
        var existing = _animals.FirstOrDefault(a => a.Id == element.Id);
        if (existing == null) return Task.FromResult(false);
        existing.Species = element.Species;
        existing.Name = element.Name;
        existing.Age = element.Age;
        existing.EnclosureId = element.EnclosureId;
        return Task.FromResult(true);
    }
    public Task<bool> RemoveAsync(Animal element) =>
        Task.FromResult(_animals.Remove(element));

    public Task<bool> SaveAsync()
    {
        return Task.FromResult(true);
    }
}