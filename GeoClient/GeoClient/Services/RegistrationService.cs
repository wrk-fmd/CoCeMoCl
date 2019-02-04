using System;
using System.Collections.Generic;
using GeoClient.Models;
using Xamarin.Essentials;

namespace GeoClient.Services
{
    public sealed class RegistrationService
    {
        private readonly List<IGeoRegistrationListener> _registrationListeners;

        private RegistrationInfo _cachedRegistrationInfo;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RegistrationService()
        {
        }

        private RegistrationService()
        {
            _registrationListeners = new List<IGeoRegistrationListener>();
        }

        public static RegistrationService Instance { get; } = new RegistrationService();

        public void registerListener(IGeoRegistrationListener listener)
        {
            _registrationListeners.Add(listener);
        }

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

        private void HandleRegistrationStatusChange(bool wasRegisteredBefore, bool isRegisteredAfterUpdate)
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

        private void ServerRegistered()
        {
            Console.WriteLine("URL for geoserver is now registered.");
            _registrationListeners.ForEach(listener => listener.GeoServerRegistered());
        }

        private void ServerUnregistered()
        {
            Console.WriteLine("URL for geoserver is no longer registered.");
            _registrationListeners.ForEach(listener => listener.GeoServerUnregistered());
        }
    }
}