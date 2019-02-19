using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using GeoClient.Services.Utils;
using GeoClient.ViewModels;
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
        public const string ServerBaseUrl = "https://geo.fmd.wrk.at/";
        private const string LocationEndpointUri = "endpoint/positions/";
        private const string ScopeEndpointUri = "endpoint/scope/";
        private const string JsonContentType = "application/json";

        private readonly RegistrationService _registrationService;
        private readonly HttpClient _positionHttpClient;
        private readonly TaskScheduler _taskScheduler;

        public RestSendingHook BeforeLocationSending = delegate { return () => { }; };

        public static RestService Instance { get; } = new RestService();

        static RestService()
        {
        }

        private RestService()
        {
            _registrationService = RegistrationService.Instance;
            _positionHttpClient = new HttpClient();
            _taskScheduler = new InterceptedSingleThreadTaskScheduler();
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
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": Getting Scope from Server");

            await _positionHttpClient.GetAsync(scopeUrl).ContinueWith(getScopeResponse =>
            {
                try
                {
                    var responseString = getScopeResponse.Result.Content.ReadAsStringAsync().Result;
                    JObject scopeArray = JObject.Parse(responseString);

                    JArray incidentArray = (JArray) scopeArray["incidents"];
                    JArray unitsArray = (JArray) scopeArray["units"];
                    var incidents = incidentArray.Select(c => (JObject) c).ToList();
                    var units = unitsArray.Select(c => (JObject) c).ToList();

                    var incidentItemList = IncidentItemFactory.CreateIncidentItemList(incidents, units);
                    IncidentUpdateRegistry.Instance.IncidentsUpdated(incidentItemList);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to get response from server!");
                    Console.WriteLine(e.ToString());
                }
            });
        }

        private void SendPosition(Xamarin.Essentials.Location location)
        {
            var positionsUrl = CreateGeoServerLocationUrl();
            var positionObject = new JObject
            {
                {"latitude", location.Latitude},
                {"longitude", location.Longitude},
                {"timestamp", DateTime.UtcNow.ToString("s") + "Z"},
                {"accuracy", location.Accuracy}
            };

            new Task(async () =>
            {
                var locationSendingFinalizer = BeforeLocationSending();
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": Sending Data to Server: " + positionObject);

                var postPositionAsyncTask = _positionHttpClient.PostAsync(
                    positionsUrl,
                    new StringContent(positionObject.ToString(), Encoding.UTF8, JsonContentType));
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
            return ServerBaseUrl + LocationEndpointUri + registrationInfo.Id + "?token=" + registrationInfo.Token;
        }

        private string CreateGeoServerScopeUrl()
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            return ServerBaseUrl + ScopeEndpointUri + registrationInfo.Id + "?token=" + registrationInfo.Token;
        }
    }
}