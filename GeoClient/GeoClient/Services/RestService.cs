using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GeoClient.Services
{
    public class RestService : ILocationListener
    {
        private readonly string sbaseUrl = "https://geo.fmd.wrk.at/endpoint/";
        private readonly string sContentType = "application/json";

        private readonly RegistrationService _registrationService;
        private readonly HttpClient _positionHttpClient;

        public RestService()
        {
            _registrationService = RegistrationService.Instance;
            _positionHttpClient = new HttpClient();
        }

        public void LocationUpdated(Location updatedLocation)
        {
            SendPosition(updatedLocation);
        }

        private void SendPosition(Location location)
        {
            string id = RegistrationService.Instance.GetId();
            string token = RegistrationService.Instance.GetToken();
            string url = RegistrationService.Instance.GetUrl();

            string positionsUrl = sbaseUrl + "positions/" + id + "?token=" + token;
            //Console.WriteLine(positionsUrl);
            JObject positionObject = new JObject
            {
                { "latitude", location.Latitude },
                { "longitude", location.Longitude },
                { "timestamp", DateTime.UtcNow.ToString("s") + "Z" },
                { "accuracy", location.Accuracy }
            };

            Console.WriteLine("Sending Data to Server: " + positionObject.ToString());
            Task<HttpResponseMessage> postPositionAsyncTask = _positionHttpClient.PostAsync(positionsUrl, new StringContent(positionObject.ToString(), Encoding.UTF8, sContentType));
            postPositionAsyncTask.ContinueWith((postPositionResponse) =>
            {
                String responseString = postPositionResponse.Result.Content.ReadAsStringAsync().Result;
                String statusString = postPositionResponse.Status.ToString();
                Console.WriteLine("Response from Server: Staus: " + statusString + ", response: " + responseString);
            });
        }

        public void GetScope()
        {
            /*string scopeUrl = sbaseUrl + "scope/" + id + "?token=" + token;
            HttpClient scopeHttpClient = new HttpClient();

            var getScopeTask = scopeHttpClient.GetAsync(scopeUrl);
            getScopeTask.ContinueWith((getResponseMessage) =>
            {
                Log.Debug(TAG, "Getting updates via scope endpoint");
                var response = getResponseMessage.Result.Content.ReadAsStringAsync().Result;
                Models.Scope scope = new Models.Scope();
                scope.parseData(response);
            });*/
        }
    }
}
