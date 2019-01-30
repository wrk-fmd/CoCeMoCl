using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GeoClient.Services
{
    class LocationService
    {
        public Location _location;
        private object _registrationInformation; 
        RegistrationService registrationService;
        public LocationService()
        {
            registrationService = new RegistrationService();
            _registrationInformation = registrationService.getRegistrationInfo();
        }

        public async Task<Location> getLocationAsync()
        {
            if(registrationService.isRegistered())
            {
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(1));
                    _location = await Geolocation.GetLocationAsync(request);
                    if (_location != null)
                    {
                        Console.WriteLine($"Timestamp: {_location.Timestamp}, Latitude: {_location.Latitude}, Longitude: {_location.Longitude}, Altitude: {_location.Altitude}");
                        RestService restService = new RestService(_registrationInformation);
                    }
                    return _location;
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
            return null;
        }
    }
}
