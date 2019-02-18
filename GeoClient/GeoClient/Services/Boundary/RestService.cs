using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeoClient.Services.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoClient.Models;
using GeoClient.ViewModels;

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
        IncidentsViewModel viewModel;
        public List<JObject> incidents, units; 
        private readonly RegistrationService _registrationService;
        private readonly HttpClient _positionHttpClient;
        private readonly TaskScheduler _taskScheduler;

        public RestSendingHook BeforeLocationSending = delegate { return () => { }; };
        public RestSendingHook BeforeScopeGetting = delegate { return () => { }; };
        
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

        public void GetScope()
        {
            var scopeUrl = CreateGeoServerScopeUrl();
            new Task(async () =>
            {
                var scopeGettingFinalizer = BeforeScopeGetting();
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + ": Getting Scope from Server");

                var getScopeAsyncTask = _positionHttpClient.GetAsync(scopeUrl);
                await getScopeAsyncTask.ContinueWith(getScopeResponse =>
                {
                    try
                    {
                        var responseString = getScopeResponse.Result.Content.ReadAsStringAsync().Result;
                        var statusString = getScopeResponse.Result.StatusCode.ToString();
                        JObject scopeArray = JObject.Parse(responseString);

                        JArray incidentArray = (JArray)scopeArray["incidents"];
                        incidents = incidentArray.Select(c => (JObject)c).ToList();
                        JArray unitsArray = (JArray)scopeArray["units"];
                        units = unitsArray.Select(c => (JObject)c).ToList();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to get response from server!");
                        Console.WriteLine(e.ToString());
                    }

                    scopeGettingFinalizer();
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