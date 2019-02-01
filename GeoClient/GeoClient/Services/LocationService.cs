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
        private RestService _restService;
        public LocationService()
        {
            _restService = new RestService();
        }

        public async Task<Location> getLocationAsync()
        {
            if (RegistrationService.Instance.isRegistered())
            {
                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(1));
                    _location = await Geolocation.GetLocationAsync(request);
                    if (_location != null)
                    {
                        _restService.sendPosition(_location);
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
