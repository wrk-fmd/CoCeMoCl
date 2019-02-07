using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeoClient.Services.Boundary
{
    public class RestService : ILocationListener
    {
        public const string ServerBaseUrl = "https://geo.fmd.wrk.at/";
        private const string EndpointUri = "endpoint/positions/";
        private const string JsonContentType = "application/json";

        private readonly RegistrationService _registrationService;
        private readonly HttpClient _positionHttpClient;

        public RestService()
        {
            _registrationService = RegistrationService.Instance;
            _positionHttpClient = new HttpClient();
        }

        public void LocationUpdated(Xamarin.Essentials.Location updatedLocation)
        {
            SendPosition(updatedLocation);
        }

        private void SendPosition(Xamarin.Essentials.Location location)
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();

            string positionsUrl =
                ServerBaseUrl + EndpointUri + registrationInfo.Id + "?token=" + registrationInfo.Token;
            //Console.WriteLine(positionsUrl);
            var positionObject = new JObject
            {
                {"latitude", location.Latitude},
                {"longitude", location.Longitude},
                {"timestamp", DateTime.UtcNow.ToString("s") + "Z"},
                {"accuracy", location.Accuracy}
            };

            Console.WriteLine("Sending Data to Server: " + positionObject);
            Task<HttpResponseMessage> postPositionAsyncTask = _positionHttpClient.PostAsync(positionsUrl,
                new StringContent(positionObject.ToString(), Encoding.UTF8, JsonContentType));
            postPositionAsyncTask.ContinueWith((postPositionResponse) =>
            {
                var responseString = postPositionResponse.Result.Content.ReadAsStringAsync().Result;
                var statusString = postPositionResponse.Status.ToString();
                Console.WriteLine("Response from Server: Status: " + statusString + ", response: " + responseString);
            });
        }
    }
}