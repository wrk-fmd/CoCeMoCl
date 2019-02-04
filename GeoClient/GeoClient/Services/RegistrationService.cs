using System;
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

        public async void LoadRegistrationInfo()
        {
            var loadedUrl = await SecureStorage.GetAsync("url");
            if (loadedUrl != null)
            {
                ParseRegistrationInfoFromUrl(loadedUrl);
            }
        }

        private void ParseRegistrationInfoFromUrl(string url)
        {
            var wasRegisteredBefore = IsRegistered();
            _cachedRegistrationInfo = RegistrationInfoParser.ParseRegistrationInfo(url);
            var isRegisteredAfterUpdate = IsRegistered();

            HandleRegistrationStatusChange(wasRegisteredBefore, isRegisteredAfterUpdate);
        }

        private static void HandleRegistrationStatusChange(bool wasRegisteredBefore, bool isRegisteredAfterUpdate)
        {
            if (!wasRegisteredBefore && isRegisteredAfterUpdate)
            {
                ServerRegistered();
            }
            else if (wasRegisteredBefore && !isRegisteredAfterUpdate)
            {
                ServerUnregistered();
            }
        }

        private static void ServerRegistered()
        {
            Console.WriteLine("URL for geo server is now registered.");
            LocationSender.Instance.StartTimer();
        }

        private static void ServerUnregistered()
        {
            Console.WriteLine("URL for geo server is no longer registered.");
        }
    }
}