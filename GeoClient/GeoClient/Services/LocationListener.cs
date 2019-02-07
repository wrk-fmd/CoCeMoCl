using Xamarin.Essentials;

namespace GeoClient.Services
{
    public interface ILocationListener
    {
        void LocationUpdated(Location updatedLocation);
    }
}