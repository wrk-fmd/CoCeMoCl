using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace GeoClient.Services.Boundary
{
    public delegate RestSendingFinalizer RestSendingHook();

    public delegate void RestSendingFinalizer();

    public class RestService : ILocationUpdateListener
    {
        public const string ServerBaseUrl = "https://geo.fmd.wrk.at/";
        private const string EndpointUri = "endpoint/positions/";
        private const string JsonContentType = "application/json";

        private readonly RegistrationService _registrationService;
        private readonly HttpClient _positionHttpClient;

        public RestSendingHook BeforeLocationSending = delegate { return () => { }; };

        public static RestService Instance { get; } = new RestService();

        static RestService()
        {
        }

        private RestService()
        {
            _registrationService = RegistrationService.Instance;
            _positionHttpClient = new HttpClient();
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
            var positionsUrl = CreateGeoServerUrl();
            var positionObject = new JObject
            {
                {"latitude", location.Latitude},
                {"longitude", location.Longitude},
                {"timestamp", DateTime.UtcNow.ToString("s") + "Z"},
                {"accuracy", location.Accuracy}
            };

            var locationSendingFinalizer = BeforeLocationSending();
            Console.WriteLine("Sending Data to Server: " + positionObject);

            var postPositionAsyncTask = _positionHttpClient.PostAsync(
                positionsUrl,
                new StringContent(positionObject.ToString(), Encoding.UTF8, JsonContentType));
            postPositionAsyncTask.ContinueWith(postPositionResponse =>
            {
                var responseString = postPositionResponse.Result.Content.ReadAsStringAsync().Result;
                var statusString = postPositionResponse.Result.StatusCode.ToString();
                Console.WriteLine("Response from Server: Status: " + statusString + ", response: " + responseString);
                locationSendingFinalizer();
            });
        }

        private string CreateGeoServerUrl()
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            return ServerBaseUrl + EndpointUri + registrationInfo.Id + "?token=" + registrationInfo.Token;
        }
    }
}