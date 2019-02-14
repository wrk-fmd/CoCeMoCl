using GeoClient.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoClient.Services
{
    public class InMemoryDataStore : IDataStore<IncidentItem>
    {
        private readonly ConcurrentDictionary<string, IncidentItem> _items;

        public InMemoryDataStore()
        {
            _items = new ConcurrentDictionary<string, IncidentItem>();
            AddMockedItemsForUiDesign();
        }

        public async Task<bool> AddItemAsync(IncidentItem incidentItem)
        {
            _items.TryAdd(incidentItem.Id, incidentItem);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(IncidentItem incidentItem)
        {
            _items.TryAdd(incidentItem.Id, incidentItem);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            _items.TryRemove(id, out _);

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<IncidentItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(_items.Values);
        }

        private void AddMockedItemsForUiDesign()
        {
            var mockItems = new List<IncidentItem>
            {
                new IncidentItem(Guid.NewGuid().ToString())
                {
                    Info = "Demo Incident at location XYZ\nSomething happened there!\n\nContact details can be asked at the control room.",
                    Type = GeoIncidentType.Task,
                    Blue = true,
                    OwnTaskState = IncidentTaskState.Assigned
                },
                new IncidentItem(Guid.NewGuid().ToString())
                {
                    Info = "Another active incident",
                    Type = GeoIncidentType.Relocation,
                    OwnTaskState = IncidentTaskState.Zao
                },
                new IncidentItem(Guid.NewGuid().ToString())
                {
                    Type = GeoIncidentType.Relocation,
                    Priority = true,
                    Blue = true,
                    Location = new GeoPoint(42, 13)
                }
            };

            foreach (var item in mockItems)
            {
                _items.TryAdd(item.Id, item);
            }
        }
    }
}