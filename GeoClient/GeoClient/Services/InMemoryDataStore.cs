using GeoClient.Models;
using GeoClient.Services.Boundary;
using GeoClient.Services.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoClient.Services
{
    public class InMemoryDataStore : IDataStore<IncidentItem>
    {
        private ConcurrentDictionary<string, IncidentItem> _items;
        RestService restService;
        public InMemoryDataStore()
        {
            restService = RestService.Instance;
            _items = new ConcurrentDictionary<string, IncidentItem>();
//            AddMockedItemsForUiDesign();
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
            
            if (restService.incidents != null)
            {
                foreach (var _incident in restService.incidents)
                {
                    IncidentItem incident = new IncidentItem((string)_incident["id"]);
                    incident.Info = (string)_incident["info"];
                    incident.Type = GeoIncidentTypeFactory.GetTypeFromString((string)_incident["type"]);
                    incident.Priority = (bool)_incident["priority"];
                    incident.Blue = (bool)_incident["blue"];
                    incident.Location = new GeoPoint(long.Parse((string)_incident["location"]["latitude"]), long.Parse((string)_incident["location"]["longitude"]));
                    //incident.AssignedUnits = (string)_incident["assignedUnits"];
                    Console.WriteLine(incident.ToString());
                    await AddItemAsync(incident);
                }
            }
            return await Task.FromResult(_items.Values);
        }
    }
}