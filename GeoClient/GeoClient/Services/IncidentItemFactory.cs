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
    public static class IncidentItemFactory
    {
        public static List<IncidentItem> CreateIncidentItemList(List<JObject> incidents, List<JObject> units)
        {
            var incidentList = new List<IncidentItem>();

            if (incidents != null)
            {
                foreach (var incident in incidents)
                {
                    var incidentItem = CreateIncidentItem(incident, units);

                    incidentList.Add(incidentItem);
                }
            }

            return incidentList;
        }

        private static IncidentItem CreateIncidentItem(JObject incident, List<JObject> unitJsonObjects)
        {
            IncidentItem incidentItem = new IncidentItem((string) incident["id"]);
            incidentItem.Info = (string) incident["info"];
            incidentItem.Type = GeoIncidentTypeFactory.GetTypeFromString((string) incident["type"]);
            incidentItem.Priority = bool.Parse((string) incident["priority"]);
            incidentItem.Blue = bool.Parse((string) incident["blue"]);
            incidentItem.Location = CreateGeoPoint(incident["location"]);

            Dictionary<string, string> rawAssignedUnits =
                incident["assignedUnits"].ToObject<Dictionary<string, string>>();

            incidentItem.Units = CreateUnitList(rawAssignedUnits, unitJsonObjects);

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

        private static SortedSet<Unit> CreateUnitList(IReadOnlyDictionary<string, string> rawAssignedUnits, List<JObject> unitJsonObjects)
        {
            var units = new SortedSet<Unit>();
            if (unitJsonObjects != null)
            {
                foreach (var unitJsonObject in unitJsonObjects)
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