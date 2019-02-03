using Xamarin.Essentials;

namespace GeoClient.Services
{
    interface ILocationListener
    {
        void LocationUpdated(Location updatedLocation);
    }
}