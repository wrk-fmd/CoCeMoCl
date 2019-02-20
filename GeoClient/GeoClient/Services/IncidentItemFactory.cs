using GeoClient.Models;
using GeoClient.Services.Utils;
using GeoClient.Views.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using static System.Double;

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
            else
            {
                Console.WriteLine("Provided incidents are null. Cannot parse incident items.");
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

        private static GeoPoint CreateGeoPoint(JToken pointJToken)
        {
            GeoPoint geoPoint = null;

            string latitudeString = (string) pointJToken?["latitude"];
            string longitudeString = (string) pointJToken?["longitude"];

            if (GeoPointUtil.NotBlank(latitudeString) && GeoPointUtil.NotBlank(longitudeString))
            {
                geoPoint = CreateGeoPoint(latitudeString, longitudeString);
            }

            return geoPoint;
        }

        private static GeoPoint CreateGeoPoint(string latitudeString, string longitudeString)
        {
            GeoPoint geoPoint = null;
            try
            {
                double latitude = Parse(latitudeString, CultureInfo.InvariantCulture);
                double longitude = Parse(longitudeString, CultureInfo.InvariantCulture);
                geoPoint = new GeoPoint(latitude, longitude);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Cannot parse double of coordinate value 'null'.");
            }
            catch (FormatException)
            {
                Console.WriteLine(
                    "Provided coordinates does not contain valid numbers. lat=" + latitudeString +
                    ", lon=" + longitudeString);
            }
            catch (OverflowException)
            {
                Console.WriteLine("Coordinate values are out of range.");
            }

            return geoPoint;
        }

        private static SortedSet<Unit> CreateUnitList(
            IReadOnlyDictionary<string, string> rawAssignedUnits,
            List<JObject> unitJsonObjects)
        {
            var units = new SortedSet<Unit>();
            if (unitJsonObjects != null)
            {
                foreach (var rawAssignedUnit in rawAssignedUnits)
                {
                    var unit = GetUnitFromUnitList(rawAssignedUnit.Key, unitJsonObjects);
                    unit.State = IncidentTaskStateFactory.GetTaskStateFromString(rawAssignedUnit.Value);
                    units.Add(unit);
                }
            }

            return units;
        }

        private static Unit GetUnitFromUnitList(string unitId, List<JObject> unitJsonObjects)
        {
            foreach (var unitJsonObject in unitJsonObjects)
            {
                if ((string) unitJsonObject["id"] == unitId)
                {
                    Unit unit = new Unit(unitId)
                    {
                        Name = (string) unitJsonObject["name"],
                        LastPoint = CreateGeoPoint(unitJsonObject["lastPoint"])
                    };

                    return unit;
                }
            }

            Console.WriteLine($"Unit with ID {unitId} is not present in list of units!");
            return null;
        }
    }
}