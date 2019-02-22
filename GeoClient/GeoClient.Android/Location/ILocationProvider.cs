namespace GeoClient.Droid.Location
{
    public delegate void LocationProviderListener(Xamarin.Essentials.Location updatedLocation);

    public interface ILocationProvider
    {
        void RegisterLocationUpdateDelegate(LocationProviderListener updateDelegate);

        void StartLocationProvider();

        void StopLocationProvider();
    }
}