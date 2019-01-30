using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GeoClient.Services
{
    public class RegistrationService
    {
        private string _url;
        private string _isRegistered;

        public async void setRegistrationInfo(string url)
        {
            var _url = await SecureStorage.GetAsync("url");
            var idpos = _url.IndexOf("id=");
            var tokenpos = _url.IndexOf("&token=");
            var id = _url.Substring(idpos + 3, tokenpos - idpos - 3);
            var token = _url.Substring(tokenpos + 4);
            await SecureStorage.SetAsync("id", id);
            await SecureStorage.SetAsync("token", token);
            _url = url; 
        }
        public async Task<object> getRegistrationInfo()
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
        public bool isRegistered()
        {
            if(_url != null)
            {
                return true; 
            }
            return false; 
        }
    }
}
