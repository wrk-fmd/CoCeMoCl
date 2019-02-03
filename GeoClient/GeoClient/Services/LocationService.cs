using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GeoClient.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeoClient.Services
{
    class LocationService
    {
        private List<ILocationListener> _locationListeners;


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LocationService()
        {
        }

        private LocationService()
        {
            _locationListeners = new List<ILocationListener>();
        }

        public static LocationService Instance { get; } = new LocationService();

        public void RegisterListener(ILocationListener listener)
        {
            Console.WriteLine("Register a new location listener.");
            _locationListeners.Add(listener);
        }

        public void GetLocationAsync()
        {

            if (RegistrationService.Instance.IsRegistered())
            {
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(1));
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        Console.WriteLine("Request location from device.");

                        Location location = await Geolocation.GetLocationAsync(request);

                        if (location != null)
                        {
                            Console.WriteLine("Inform all listeners about updated location.");
                            _locationListeners.ForEach((listener) => listener.LocationUpdated(location));
                        }
                    });
                
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    Console.WriteLine(fnsEx.Message);
                }
                catch (PermissionException pEx)
                {
                    Console.WriteLine(pEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
