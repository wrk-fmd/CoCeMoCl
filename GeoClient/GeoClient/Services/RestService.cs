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
        string _id;
        string _url;
        string _token;

        public RestService()
        {
        }
        public void sendPosition(Location location)
        {
            _id = RegistrationService.Instance.getId();
            _token = RegistrationService.Instance.getToken();
            _url = RegistrationService.Instance.getUrl();

            string positionsUrl = sbaseUrl + "positions/" + _id + "?token=" + _token;
            //Console.WriteLine(positionsUrl);
            JObject positionObject = new JObject();

            positionObject.Add("latitude", location.Latitude);
            positionObject.Add("longitude", location.Longitude);
            positionObject.Add("timestamp", DateTime.UtcNow.ToString("s") + "Z");
            positionObject.Add("accuracy", location.Accuracy);

            HttpClient positionHttpClient = new HttpClient();
            var postPositionAsyncTask = positionHttpClient.PostAsync(positionsUrl, new StringContent(positionObject.ToString(), Encoding.UTF8, sContentType));
            //Console.WriteLine($"Sending Data to Server: {positionObject.ToString()}");
            postPositionAsyncTask.ContinueWith((postPositionResponse) =>
            {
                Console.WriteLine($"Response from Server: {postPositionResponse.Result.Content.ReadAsStringAsync().Result}");
            });
            //getScope();
            
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
