using GeoClient.Models;
using GeoClient.Services.Boundary;
using System;
using System.Collections.Concurrent;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace GeoClient.Services.Registration
{
    public sealed class RegistrationService
    {
        private readonly ConcurrentDictionary<IGeoRegistrationListener, byte> _registrationListeners;

        private RegistrationInfo _cachedRegistrationInfo;
        private bool _wasConfigurationReadFromDisk;
        private const string UrlStorageKey = "url";

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RegistrationService()
        {
        }

        private RegistrationService()
        {
            _registrationListeners = new ConcurrentDictionary<IGeoRegistrationListener, byte>();
        }

        public static RegistrationService Instance { get; } = new RegistrationService();

        public void RegisterListener(IGeoRegistrationListener listener)
        {
            if (listener != null)
            {
                _registrationListeners.TryAdd(listener, 1);
            }
            else
            {
                Console.WriteLine("Cannot add 'null' to registration listeners.");
            }
        }

        public void UnregisterListener(IGeoRegistrationListener listener)
        {
            if (listener != null)
            {
                _registrationListeners.TryRemove(listener, out _);
            }
            else
            {
                Console.WriteLine("Cannot remove 'null' from registration listeners.");
            }
        }

        public bool IsRegistered()
        {
            return _cachedRegistrationInfo != null;
        }

        public async void SetRegistrationInfo(string url)
        {
            ParseRegistrationInfoFromUrl(url);
            if (url != null)
            {
                await SecureStorage.SetAsync(UrlStorageKey, url);
            }
            else
            {
                Console.WriteLine("Removing URL from secure storage.");
                SecureStorage.Remove(UrlStorageKey);
            }
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
            var loadedUrl = await SecureStorage.GetAsync(UrlStorageKey);
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
            Console.WriteLine(
                $"URL for geoserver is now registered. Got {_registrationListeners.Count} listeners to inform.");
            _registrationListeners.ForEach(listener => listener.Key.GeoServerRegistered());
        }

        private void ServerUnregistered()
        {
            Console.WriteLine("URL for geoserver is no longer registered.");
            _registrationListeners.ForEach(listener => listener.Key.GeoServerUnregistered());
        }
    }
}