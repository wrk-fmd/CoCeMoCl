using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeoClient.Services
{
    public class LocationService
    {
        private readonly TimeSpan _timeoutToWaitForLocation = TimeSpan.FromSeconds(30);

        private readonly List<ILocationListener> _locationListeners;
        private readonly RegistrationService _registrationService;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LocationService()
        {
        }

        private LocationService()
        {
            _registrationService = RegistrationService.Instance;
            _locationListeners = new List<ILocationListener>();
        }

        public static LocationService Instance { get; } = new LocationService();

        public void RegisterListener(ILocationListener listener)
        {
            Console.WriteLine("Register a new location listener.");
            _locationListeners.Add(listener);
        }

        public void GetLocationSync()
        {
            if (!_registrationService.IsRegistered())
            {
                Console.WriteLine("There is no unit registered. Reading location data is skipped.");
                return;
            }

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, _timeoutToWaitForLocation);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Console.WriteLine("Request location from device.");

                    var location = await Geolocation.GetLocationAsync(request);

                    if (location != null)
                        InformListeners(location);
                    else
                        Console.WriteLine("GeoLocation did not return a location.");
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InformListeners(Location location)
        {
            Console.WriteLine("Inform all listeners about updated location.");
            _locationListeners.ForEach((listener) => listener.LocationUpdated(location));
        }
    }
}
