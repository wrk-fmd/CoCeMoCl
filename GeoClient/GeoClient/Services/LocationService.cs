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
        public async Task<Location> getLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    return location; 
                }
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
            return null;
        }
    }
}
