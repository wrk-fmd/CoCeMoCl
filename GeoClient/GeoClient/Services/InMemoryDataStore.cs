using GeoClient.Models;
using GeoClient.Services.Boundary;
using GeoClient.Services.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GeoClient.Services
{
    public class InMemoryDataStore : IDataStore<IncidentItem>
    {
        private readonly ConcurrentDictionary<string, IncidentItem> _incidents;
        private readonly RestService _restService;

        public InMemoryDataStore()
        {
            _incidents = new ConcurrentDictionary<string, IncidentItem>();
            _restService = RestService.Instance;
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

        // TODO: Code-Smell: Get informed about fetched data, do not read it from field in other class.
        public async Task<IEnumerable<IncidentItem>> GetItemsAsync(bool forceRefresh = false)
        {

            if (_restService.incidents != null)
            {
                foreach (var incident in _restService.incidents)
                {
                    var incidentItem = CreateIncidentItem(incident);

                    await AddItemAsync(incidentItem);
                }
            }
            Console.WriteLine(_incidents.Values);
            return await Task.FromResult(_incidents.Values);
        }

        private IncidentItem CreateIncidentItem(JObject incident)
        {
            string latitude = (string) incident["location"]["latitude"];
            string longitude = (string) incident["location"]["longitude"];


            IncidentItem incidentItem = new IncidentItem((string) incident["id"]);
            incidentItem.Info = (string) incident["info"];
            incidentItem.Type = GeoIncidentTypeFactory.GetTypeFromString((string) incident["type"]);
            incidentItem.Priority = bool.Parse((string) incident["priority"]);
            incidentItem.Blue = bool.Parse((string) incident["blue"]);
            incidentItem.Location = new GeoPoint(latitude, longitude);

            List<KeyValuePair<string, IncidentTaskState>> assignedUnits = new List<KeyValuePair<string, IncidentTaskState>>();
            Dictionary<string, string> rawAssignedUnits = incident["assignedUnits"].ToObject<Dictionary<string, string>>();
            foreach (KeyValuePair<string, string> unit in rawAssignedUnits)
            {
                assignedUnits.Add(new KeyValuePair<string, IncidentTaskState>(unit.Key, IncidentTaskStateFactory.GetTaskStateFromString(unit.Value)));
            }

            incidentItem.AssignedUnits = assignedUnits;
            
            incidentItem.Units = CreateUnitList(rawAssignedUnits);

            return incidentItem;
        }

        private List<Unit> CreateUnitList(Dictionary<string, string> rawAssignedUnits)
        {
            List<Unit> units = new List<Unit>();
            if (_restService.units != null)
            {
                foreach (var unitJsonObject in _restService.units)
                {
                    Unit unit = new Unit((string) unitJsonObject["id"]);
                    unit.Name = (string) unitJsonObject["name"];
                    unit.LastPoint = new GeoPoint((string) unitJsonObject["lastPoint"]["latitude"],
                        (string) unitJsonObject["lastPoint"]["longitude"]);

                    rawAssignedUnits.TryGetValue(unit.Id, out var rawUnitState);
                    unit.State = IncidentTaskStateFactory.GetTaskStateFromString(rawUnitState);
                    
                    units.Add(unit);
                }
            }

            return units;
        }
    }
}