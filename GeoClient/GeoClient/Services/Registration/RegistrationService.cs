using GeoClient.Models;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace GeoClient.Services.Registration
{
    public sealed class RegistrationService
    {
        private readonly List<IGeoRegistrationListener> _registrationListeners;

        private RegistrationInfo _cachedRegistrationInfo;
        private bool _wasConfigurationReadFromDisk;

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

        public void RegisterListener(IGeoRegistrationListener listener)
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
            TryToLoadRegistrationInfo();
            return _cachedRegistrationInfo;
        }

        private void TryToLoadRegistrationInfo()
        {
            if (_wasConfigurationReadFromDisk)
                return;

            bool needToLoadConfiguration;
            lock (this)
            {
                needToLoadConfiguration = !_wasConfigurationReadFromDisk;
                if (needToLoadConfiguration)
                    _wasConfigurationReadFromDisk = true;
            }

            if (needToLoadConfiguration)
            {
                Console.WriteLine("Registration info was not loaded yet. Try to read from disk.");
                LoadRegistrationInfo();
            }
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