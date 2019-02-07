namespace GeoClient.Services.Location
{
    public interface ILocationListener
    {
        void LocationUpdated(Xamarin.Essentials.Location updatedLocation);
    }
}