using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace GeoClient.Services
{
    public class RestService
    {
        string sbaseUrl = "https://geo.fmd.wrk.at/endpoint/";
        string sContentType = "application/json";
        object oRegistrationInformation;
        public RestService(object registrationInformation)
        {
            oRegistrationInformation = registrationInformation;
        }
        public void sendPosition(Location location)
        {

            /*string positionsUrl = sbaseUrl + "positions/" + oRegistrationInformation.id + "?token=" + oRegistrationInformation.token;

            JObject positionObject = new JObject();

            positionObject.Add("latitude", location.Latitude);
            positionObject.Add("longitude", location.Longitude);
            positionObject.Add("timestamp", DateTime.UtcNow.ToString("s") + "Z");
            positionObject.Add("accuracy", location.Accuracy);

            HttpClient positionHttpClient = new HttpClient();
            var postPositionAsyncTask = positionHttpClient.PostAsync(positionsUrl, new StringContent(positionObject.ToString(), Encoding.UTF8, sContentType));
            Console.WriteLine( positionObject.ToString());
            Console.WriteLine($"Sending Data to Server: {positionObject.ToString()}");
            postPositionAsyncTask.ContinueWith((postPositionResponse) =>
            {
                Console.WriteLine($"Sending Data to Server: {postPositionResponse.Result.Content.ReadAsStringAsync().Result}");
            });
            //getScope();
            */
        }
        public void getScope()
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
