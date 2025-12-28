using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace Zoo.Common
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        private List<T> _items = new List<T>();

        private Guid GetId(T element)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null)
                throw new Exception("Тип не має властивості Id");
            return (Guid)prop.GetValue(element);
        }

        public void Create(T element)
        {
            _items.Add(element);
        }

        public T Read(Guid id)
        {
            return _items.FirstOrDefault(e => GetId(e) == id);
        }

        public IEnumerable<T> ReadAll()
        {
            return _items;
        }

        public void Update(T element)
        {
            var id = GetId(element);
            var index = _items.FindIndex(e => GetId(e) == id);
            if (index != -1)
                _items[index] = element;
        }

        public void Remove(T element)
        {
            var id = GetId(element);
            var item = _items.FirstOrDefault(e => GetId(e) == id);
            if (item != null)
                _items.Remove(item);
        }

        public void Save(string FilePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(_items, options);
            File.WriteAllText(FilePath, json);
        }

        public void Load(string FilePath)
        {
            if (!File.Exists(FilePath))
                throw new FileNotFoundException($"Файл не знайдено: {FilePath}");

            var json = File.ReadAllText(FilePath);
            var items = JsonSerializer.Deserialize<List<T>>(json);
            
            if (items != null)
                _items = items;
            else
                _items = new List<T>();
        }
    }
}
