using GeoClient.Models;
using GeoClient.Services.Boundary;
using GeoClient.Services.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoClient.Views.Utils;

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
        }

        public async Task<bool> AddItemAsync(IncidentItem incidentItem)
        {
            _incidents.TryAdd(incidentItem.Id, incidentItem);

            return await Task.FromResult(true);
        }

        // TODO: Code-Smell: Get informed about fetched data, do not read it from field in other class.
        public async Task<IEnumerable<IncidentItem>> GetItemsAsync()
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
            IncidentItem incidentItem = new IncidentItem((string) incident["id"]);
            incidentItem.Info = (string) incident["info"];
            incidentItem.Type = GeoIncidentTypeFactory.GetTypeFromString((string) incident["type"]);
            incidentItem.Priority = bool.Parse((string) incident["priority"]);
            incidentItem.Blue = bool.Parse((string) incident["blue"]);
            incidentItem.Location = CreateGeoPoint(incident["location"]);

            Dictionary<string, string> rawAssignedUnits =
                incident["assignedUnits"].ToObject<Dictionary<string, string>>();

            incidentItem.Units = CreateUnitList(rawAssignedUnits);

            return incidentItem;
        }

        private static GeoPoint CreateGeoPoint(JToken incidentPoint)
        {
            GeoPoint geoPoint = null;

            string latitude = (string)incidentPoint?["latitude"];
            string longitude = (string)incidentPoint?["longitude"];

            if (GeoPointUtil.NotBlank(latitude) && GeoPointUtil.NotBlank(longitude))
            {
                geoPoint = new GeoPoint(latitude, longitude);
            }

            return geoPoint;
        }

        private SortedSet<Unit> CreateUnitList(IReadOnlyDictionary<string, string> rawAssignedUnits)
        {
            var units = new SortedSet<Unit>();
            if (_restService.units != null)
            {
                foreach (var unitJsonObject in _restService.units)
                {
                    Unit unit = new Unit((string) unitJsonObject["id"]);
                    unit.Name = (string) unitJsonObject["name"];
                    unit.LastPoint = CreateGeoPoint(unitJsonObject["lastPoint"]);

                    rawAssignedUnits.TryGetValue(unit.Id, out var rawUnitState);
                    unit.State = IncidentTaskStateFactory.GetTaskStateFromString(rawUnitState);

                    units.Add(unit);
                }
            }

            return units;
        }
    }
}