﻿using GeoClient.Models;
using GeoClient.Models.Geobroker;
using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using GeoClient.Services.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeoClient.Services.Boundary
{
    public delegate RestSendingFinalizer RestSendingHook();

    public delegate void RestSendingFinalizer();

    public class RestService : ILocationUpdateListener
    {
        private const string LocationEndpointUri = "/endpoint/positions/";
        private const string ScopeEndpointUri = "/endpoint/scope/";

        private const string JsonContentType = "application/json";

        private readonly RegistrationService _registrationService;
        private readonly HttpClient _positionHttpClient;
        private readonly TaskScheduler _taskScheduler;
        private readonly IncidentUpdateRegistry _incidentUpdateRegistry;

        public RestSendingHook BeforeLocationSending = delegate { return () => { }; };

        public static RestService Instance { get; } = new RestService();

        static RestService()
        {
        }

        private RestService()
        {
            _registrationService = RegistrationService.Instance;
            _positionHttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            _taskScheduler = new InterceptedSingleThreadTaskScheduler();
            _incidentUpdateRegistry = IncidentUpdateRegistry.Instance;
        }

        public void LocationUpdated(Xamarin.Essentials.Location updatedLocation)
        {
            if (_registrationService.IsRegistered())
            {
                SendPosition(updatedLocation);
            }
            else
            {
                Console.WriteLine("Got a location update without being registered to a server. Cannot send update!");
            }
        }

        public async void GetScope()
        {
            if (!_registrationService.IsRegistered())
                return;

            var scopeUrl = CreateGeoServerScopeUrl();
            Console.WriteLine("Getting Scope from Server");

            await _positionHttpClient.GetAsync(scopeUrl).ContinueWith(getScopeResponse =>
            {
                try
                {
                    var responseString = getScopeResponse.Result.Content.ReadAsStringAsync().Result;
                    ReadPayloadOfResponse(responseString, out var incidents, out var units, out var oneTimeActions);

                    UpdateIncidentItemList(incidents, units, oneTimeActions);
                    UpdateOwnUnitInformation(units);
                }
                catch (JsonReaderException e)
                {
                    Console.WriteLine("Failed to parse JSON from response! Is the unit deleted on the server?");
                    Console.WriteLine(e.ToString());
                    _incidentUpdateRegistry.IncidentsInvalidated(IncidentInvalidationReason.UnitNotAvailableOnServer);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to get response from server!");
                    Console.WriteLine(e.ToString());
                    _incidentUpdateRegistry.IncidentsInvalidated(IncidentInvalidationReason.ConnectionError);
                }
            });
        }

        private void UpdateOwnUnitInformation(IEnumerable<JObject> units)
        {
            var ownUnitId = _registrationService.GetRegistrationInfo()?.Id;
            if (ownUnitId != null)
            {
                var ownUnit = units.FirstOrDefault(unit =>
                    unit.Value<string>(GeobrokerConstants.UnitIdProperty) == ownUnitId);
                var ownUnitInformation = new UnitInformation(ownUnitId,
                    ownUnit?.Value<string>(GeobrokerConstants.UnitNameProperty));
                _registrationService.RegisteredUnitInformation = ownUnitInformation;
            }
        }

        private void UpdateIncidentItemList(List<JObject> incidents, List<JObject> units, List<JObject> oneTimeActionJsonObjects)
        {
            var incidentItemList = IncidentItemFactory.CreateIncidentItemList(incidents, units, oneTimeActionJsonObjects);
            _incidentUpdateRegistry.IncidentsUpdated(incidentItemList);
        }

        private static void ReadPayloadOfResponse(
            string responseString,
            out List<JObject> incidents,
            out List<JObject> units,
            out List<JObject> oneTimeActions)
        {
            JObject scopeArray = JObject.Parse(responseString);

            JArray incidentArray = (JArray)scopeArray[GeobrokerConstants.ScopeIncidentsProperty];
            JArray unitsArray = (JArray)scopeArray[GeobrokerConstants.ScopeUnitsProperty];
            JArray oneTimeActionsArray = (JArray)scopeArray[GeobrokerConstants.ScopeOneTimeActionsProperty];

            incidents = incidentArray != null
                ? incidentArray.Select(c => (JObject)c).ToList()
                : new List<JObject>();
            units = unitsArray != null
                ? unitsArray.Select(c => (JObject)c).ToList()
                : new List<JObject>();
            oneTimeActions = oneTimeActionsArray != null
                ? oneTimeActionsArray.Select(c => (JObject)c).ToList()
                : new List<JObject>();
        }

        private void SendPosition(Xamarin.Essentials.Location location)
        {
            var positionsUrl = CreateGeoServerLocationUrl();
            var positionObject = new JObject
            {
                {GeobrokerConstants.GeoPositionLatitudeProperty, location.Latitude},
                {GeobrokerConstants.GeoPositionLongitudeProperty, location.Longitude},
                {GeobrokerConstants.GeoPositionTimestampProperty, location.Timestamp.UtcDateTime.ToString("s") + "Z"},
                {GeobrokerConstants.GeoPositionAccuracyProperty, location.Accuracy},
                {GeobrokerConstants.GeoPositionHeadingProperty, location.Course},
                {GeobrokerConstants.GeoPositionSpeedProperty, location.Speed}
            };

            new Task(async () =>
            {
                var locationSendingFinalizer = BeforeLocationSending();
                var positionObjectJsonString = positionObject.ToString(Formatting.None);
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": Sending Data to Server: " + positionObjectJsonString);

                var postPositionAsyncTask = _positionHttpClient.PostAsync(
                    positionsUrl,
                    new StringContent(positionObjectJsonString, Encoding.UTF8, JsonContentType));
                await postPositionAsyncTask.ContinueWith(postPositionResponse =>
                {
                    try
                    {
                        var responseString = postPositionResponse.Result.Content.ReadAsStringAsync().Result;
                        var statusString = postPositionResponse.Result.StatusCode.ToString();
                        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": Response from Server: Status: " +
                                          statusString + ", response: " + responseString);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to get response from server!");
                        Console.WriteLine(e.ToString());
                    }

                    locationSendingFinalizer();
                });
            }).RunSynchronously(_taskScheduler);
        }

        private string CreateGeoServerLocationUrl()
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            return registrationInfo.BaseUrl + LocationEndpointUri + registrationInfo.Id + "?token=" +
                   registrationInfo.Token;
        }

        private string CreateGeoServerScopeUrl()
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            return registrationInfo.BaseUrl + ScopeEndpointUri + registrationInfo.Id + "?token=" +
                   registrationInfo.Token;
        }
    }
}