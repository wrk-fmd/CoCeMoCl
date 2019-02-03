using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GeoClient.Services
{
    public sealed class RegistrationService
    {
        public string _url;
        public string _id;
        public string _token;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RegistrationService()
        {
        }

        private RegistrationService()
        {
        }

        public static RegistrationService Instance { get; } = new RegistrationService();

        public string GetUrl()
        {
            return _url;
        }
        public string GetId()
        {
            return _id;
        }  
        public string GetToken()
        {
            return _token; 
        }
        
        public bool IsRegistered()
        {
            if (_url != null)
            {
                return true;
            }
            return false;
        }

        public async void SetRegistrationInfo(string url)
        {
            await SecureStorage.SetAsync("url", url);
            
            if (url != null)
            {
                var idpos = url.IndexOf("id=");
                var tokenpos = url.IndexOf("&token=");
                var id = url.Substring(idpos + 3, tokenpos - idpos - 3);
                var token = url.Substring(tokenpos + 4);
                await SecureStorage.SetAsync("id", id);
                await SecureStorage.SetAsync("token", token);
                _url = url;
                _id = id;
                _token = token;
            }
        }
        public async Task<object> GetRegistrationInfo()
        {
            var _url = await SecureStorage.GetAsync("url");
            var _id = await SecureStorage.GetAsync("id");
            var _token = await SecureStorage.GetAsync("token");
            var _registrationInfo = new
            {
                url = _url,
                id = _id,
                token = _token
            };
            return _registrationInfo;
        }
        
    }
}
