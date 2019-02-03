using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoClient.Models;

namespace GeoClient.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> _items;

        public MockDataStore()
        {
            _items = new List<Item>();
            var mockItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "Demo incident", Description="This is a placeholder for an incident." },
                new Item { Id = Guid.NewGuid().ToString(), Text = "Another active incident", Description="Another incident would be shown here." }
            };

            foreach (var item in mockItems)
            {
                _items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = _items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            _items.Remove(oldItem);
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = _items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            _items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(_items);
        }
    }
}