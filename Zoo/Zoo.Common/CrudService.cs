using System;
using System.Collections.Generic;
using System.Linq;

namespace Zoo.Common
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        // внутрішня колекція для зберігання елементів
        private List<T> _items = new List<T>();

        // функція для отримання Id через reflection
        private Guid GetId(T element)
        {
            var prop = typeof(T).GetProperty("Id");
            if (prop == null)
                throw new Exception("Тип не має властивості Id");
            return (Guid)prop.GetValue(element);
        }

        // Create - додамо елемент
        public void Create(T element)
        {
            _items.Add(element);
        }

        // Read - шукаємо елемент по Id
        public T Read(Guid id)
        {
            return _items.FirstOrDefault(e => GetId(e) == id);
        }

        // ReadAll - повертає всі елементи
        public IEnumerable<T> ReadAll()
        {
            return _items;
        }

        // Update - змінюємо існуючий елемент по Id
        public void Update(T element)
        {
            var id = GetId(element);
            var index = _items.FindIndex(e => GetId(e) == id);
            if (index != -1)
                _items[index] = element;
        }

        // Remove - видаляємо елемент
        public void Remove(T element)
        {
            var id = GetId(element);
            var item = _items.FirstOrDefault(e => GetId(e) == id);
            if (item != null)
                _items.Remove(item);
        }
    }
}
