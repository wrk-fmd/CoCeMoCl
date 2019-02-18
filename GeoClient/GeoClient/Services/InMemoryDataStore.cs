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
        private ConcurrentDictionary<string, IncidentItem> _incidents;
        RestService restService;
        public InMemoryDataStore()
        {
            _incidents = new ConcurrentDictionary<string, IncidentItem>();
            restService = RestService.Instance;
            GetItemsAsync(true);
        }

        public async Task<bool> AddItemAsync(IncidentItem incidentItem)
        {
            _incidents.TryAdd(incidentItem.Id, incidentItem);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(IncidentItem incidentItem)
        {
            _incidents.TryAdd(incidentItem.Id, incidentItem);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            _incidents.TryRemove(id, out _);

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<IncidentItem>> GetItemsAsync(bool forceRefresh = false)
        {

            if (restService.incidents != null)
            {
                foreach (var _incident in restService.incidents)
                {
                    string latitude = (string)_incident["location"]["latitude"];
                    string longitude = (string)_incident["location"]["longitude"];
                    

                    IncidentItem incident = new IncidentItem((string)_incident["id"]);
                    incident.Info = (string)_incident["info"];
                    incident.Type = GeoIncidentTypeFactory.GetTypeFromString((string)_incident["type"]);
                    incident.Priority = bool.Parse((string)_incident["priority"]);
                    incident.Blue = bool.Parse((string)_incident["blue"]);
                    incident.Location = new GeoPoint(latitude, longitude);

                    List<KeyValuePair<string, string>> assignedUnits = new List<KeyValuePair<string, string>>();
                    Dictionary<string, string> units = _incident["assignedUnits"].ToObject<Dictionary<string, string>>();
                    foreach (KeyValuePair<string, string> unit in units)
                    {
                        assignedUnits.Add(new KeyValuePair<string, string>(unit.Key, unit.Value));
                    }
                    incident.AssignedUnits = assignedUnits;

                    await AddItemAsync(incident);
                }
            }
            Console.WriteLine(_incidents.Values);
            return await Task.FromResult(_incidents.Values);
        }
    }
}