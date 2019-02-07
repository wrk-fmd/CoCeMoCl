using GeoClient.Services.Registration;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeoClient.Services.Location
{
    public class LocationService
    {
        private readonly TimeSpan _timeoutToWaitForLocation = TimeSpan.FromSeconds(30);

        private readonly RegistrationService _registrationService;
        private readonly LocationChangeRegistry _locationChangeRegistry;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LocationService()
        {
        }

        private LocationService()
        {
            _registrationService = RegistrationService.Instance;
            _locationChangeRegistry = LocationChangeRegistry.Instance;
        }

        public static LocationService Instance { get; } = new LocationService();

        public void TriggerLocationAsync()
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

        private void InformListeners(Xamarin.Essentials.Location location)
        {
            _locationChangeRegistry.LocationUpdated(location);
        }
    }
}
