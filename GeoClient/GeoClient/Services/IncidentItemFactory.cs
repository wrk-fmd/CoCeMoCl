using GeoClient.Models;
using GeoClient.Services.Utils;
using GeoClient.Views.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GeoClient.Models.Geobroker;
using static System.Double;

namespace GeoClient.Services
{
    public static class IncidentItemFactory
    {
        public static List<IncidentItem> CreateIncidentItemList(List<JObject> incidents, List<JObject> units, List<JObject> oneTimeActionJsonObjects)
        {
            var incidentList = new List<IncidentItem>();

            if (incidents != null)
            {
                foreach (var incident in incidents)
                {
                    var incidentItem = CreateIncidentItem(incident, units, oneTimeActionJsonObjects);

                    incidentList.Add(incidentItem);
                }
            }
            else
            {
                Console.WriteLine("Provided incidents are null. Cannot parse incident items.");
            }

            return incidentList;
        }

        private static IncidentItem CreateIncidentItem(JObject incident, List<JObject> unitJsonObjects, List<JObject> oneTimeActionJsonObjects)
        {
            string incidentId = incident.Value<string>(GeobrokerConstants.IncidentIdProperty);
            Dictionary<string, string> rawAssignedUnits =
                incident[GeobrokerConstants.IncidentAssignedUnitsProperty]
                    .ToObject<Dictionary<string, string>>();
            var unitList = CreateUnitList(rawAssignedUnits, unitJsonObjects);
            var oneTimeActionList = CreateNextStateOneTimeActionList(incidentId, oneTimeActionJsonObjects);
            
            return new IncidentItem(
                incidentId,
                GeoIncidentTypeFactory.GetTypeFromString(incident.Value<string>(GeobrokerConstants.IncidentTypeProperty)),
                incident.Value<string>(GeobrokerConstants.IncidentInfoProperty),
                incident.Value<bool>(GeobrokerConstants.IncidentPriorityProperty),
                incident.Value<bool>(GeobrokerConstants.IncidentBlueProperty),
                CreateGeoPoint(incident[GeobrokerConstants.IncidentLocationProperty]),
                CreateGeoPoint(incident[GeobrokerConstants.IncidentDestinationProperty]),
                unitList,
                oneTimeActionList.FirstOrDefault());
        }

        private static GeoPoint CreateGeoPoint(JToken pointJToken)
        {
            GeoPoint geoPoint = null;

            var latitudeString = pointJToken?.Value<string>(GeobrokerConstants.GeoPointLatitudeProperty);
            var longitudeString = pointJToken?.Value<string>(GeobrokerConstants.GeoPointLongitudeProperty);

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

        private static List<NextStateActionOfIncident> CreateNextStateOneTimeActionList(
            string incidentId,
            List<JObject> oneTimeActionJsonObjects)
        {
            return oneTimeActionJsonObjects
                .FindAll(action => action.Value<string>(GeobrokerActionConstants.typeProperty) == "nextState" && action.Value<string>(GeobrokerActionConstants.incidentIdProperty) == incidentId)
                .ConvertAll(action => CreateNextStateAction(action))
                .FindAll(action => action != null);
        }

        private static NextStateActionOfIncident CreateNextStateAction(JObject actionJsonObject)
        {
            NextStateActionOfIncident createdAction = null;
            try
            {
                Uri parsedUrl = new Uri(actionJsonObject.Value<string>(GeobrokerActionConstants.urlProperty));
                OneTimeActionTaskState parsedPlannedState = (OneTimeActionTaskState)Enum.Parse(typeof(OneTimeActionTaskState), actionJsonObject.Value<string>(GeobrokerActionConstants.additionalDataProperty));

                createdAction = new NextStateActionOfIncident(parsedUrl, parsedPlannedState);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Failed to build one-time-action from received JSON.");
                Console.WriteLine(e.ToString());
            }
            catch (UriFormatException e)
            {
                Console.WriteLine("Failed to create URL of one-time-action.");
                Console.WriteLine(e.ToString());
            }

            return createdAction;
        }

        private static List<UnitOfIncident> CreateUnitList(
            IReadOnlyDictionary<string, string> rawAssignedUnits,
            List<JObject> unitJsonObjects)
        {
            var units = new List<UnitOfIncident>();
            if (unitJsonObjects != null)
            {
                foreach (var rawAssignedUnit in rawAssignedUnits)
                {
                    var taskState = IncidentTaskStateFactory.GetTaskStateFromString(rawAssignedUnit.Value);
                    var unit = GetUnitFromUnitList(rawAssignedUnit.Key, unitJsonObjects, taskState);
                    units.Add(unit);
                }
            }

            return units;
        }

        private static UnitOfIncident GetUnitFromUnitList(
            string unitId,
            List<JObject> unitJsonObjects,
            IncidentTaskState taskStateOfUnit)
        {
            UnitOfIncident ownUnit = null;

            var matchingUnitJson =
                unitJsonObjects.FirstOrDefault(unit => unit.Value<string>(GeobrokerConstants.UnitIdProperty) == unitId);

            if (matchingUnitJson != null)
            {
                ownUnit = new UnitOfIncident(
                    unitId,
                    matchingUnitJson.Value<string>(GeobrokerConstants.UnitNameProperty),
                    CreateGeoPoint(matchingUnitJson[GeobrokerConstants.UnitLastPointProperty]),
                    taskStateOfUnit
                );
            }
            else
            {
                Console.WriteLine($"Unit with ID {unitId} is not present in list of units!");
            }

            return ownUnit;
        }
    }
}