using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Zoo.Common
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly ConcurrentDictionary<Guid, T> _items = new ConcurrentDictionary<Guid, T>();
        private readonly string _filePath;
        private readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);
        private readonly Func<T, Guid> _getId;
        private CrudServiceAsync(string filePath)
        {
            _filePath = filePath;
            
            var prop = typeof(T).GetProperty("Id");
            if (prop == null)
                throw new ArgumentException("Повинен мати властивість id");
            
            _getId = (T element) => (Guid)prop.GetValue(element);
        }

        public static async Task<CrudServiceAsync<T>> CreateAndLoadAsync(string filePath)
        {
            var service = new CrudServiceAsync<T>(filePath);
            await service.LoadAsync();
            return service;
        }

        private async Task LoadAsync()
        {
            if (!File.Exists(_filePath))
                return;

            await _fileLock.WaitAsync();
            try
            {
                string json = await File.ReadAllTextAsync(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var itemsFromFile = JsonSerializer.Deserialize<IEnumerable<T>>(json);
                _items.Clear();
                foreach (var item in itemsFromFile)
                {
                    _items.TryAdd(_getId(item), item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка {ex.Message}");
            }
            finally
            {
                _fileLock.Release();
            }
        }

        public Task<bool> CreateAsync(T element)
        {
            return Task.FromResult(_items.TryAdd(_getId(element), element));
        }

        public Task<T> ReadAsync(Guid id)
        {
            _items.TryGetValue(id, out var item);
            return Task.FromResult(item);
        }

        public Task<IEnumerable<T>> ReadAllAsync()
        {
            return Task.FromResult(_items.Values.AsEnumerable());
        }

        public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            var pagedItems = _items.Values
                .Skip((page - 1) * amount)
                .Take(amount);
            return Task.FromResult(pagedItems);
        }

        public Task<bool> UpdateAsync(T element)
        {
            var id = _getId(element);
            if (!_items.ContainsKey(id))
                return Task.FromResult(false);
            
            _items[id] = element;
            return Task.FromResult(true);
        }

        public Task<bool> RemoveAsync(T element)
        {
            return Task.FromResult(_items.TryRemove(_getId(element), out _));
        }

        public async Task<bool> SaveAsync()
        {
            await _fileLock.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_items.Values, options);
                await File.WriteAllTextAsync(_filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                return false;
            }
            finally
            {
                _fileLock.Release();
            }
        }
        public IEnumerator<T> GetEnumerator() => _items.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}