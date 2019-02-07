namespace GeoClient.Services.Location
{
    public interface ILocationUpdateListener
    {
        void LocationUpdated(Xamarin.Essentials.Location updatedLocation);
    }
}