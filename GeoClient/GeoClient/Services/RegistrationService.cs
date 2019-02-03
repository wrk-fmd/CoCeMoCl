using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GeoClient.Models;
using Xamarin.Essentials;

namespace GeoClient.Services
{
    public sealed class RegistrationService
    {
        private RegistrationInfo _cachedRegistrationInfo;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RegistrationService()
        {
        }

        private RegistrationService()
        {
        }

        public static RegistrationService Instance { get; } = new RegistrationService();
        
        public bool IsRegistered()
        {
            return _cachedRegistrationInfo != null;
        }

        public async void SetRegistrationInfo(string url)
        {
            ParseRegistrationInfoFromUrl(url);
            await SecureStorage.SetAsync("url", url);
        }

        public RegistrationInfo GetRegistrationInfo()
        {
            return _cachedRegistrationInfo;
        }

        private void ParseRegistrationInfoFromUrl(string url)
        {
            _cachedRegistrationInfo = RegistrationInfoParser.ParseRegistrationInfo(url);
        }

        public async void LoadRegistrationInfo()
        {
            var loadedUrl = await SecureStorage.GetAsync("url");
            if (loadedUrl != null)
            {
                ParseRegistrationInfoFromUrl(loadedUrl);
            }
        }
    }
}
