using System;
using CoreLocation;
using GeoClient.Services.Location;
using UIKit;
using Xamarin.Essentials;

// See https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/backgrounding/ios-backgrounding-walkthroughs/location-walkthrough.
namespace GeoClient.iOS
{
    public class LocationManager
    {
        private readonly CLLocationManager _manager;
        private readonly LocationChangeRegistry _locationChangeRegistry;

        public LocationManager()
        {
            _manager = new CLLocationManager();
            _locationChangeRegistry = LocationChangeRegistry.Instance;
            _manager.PausesLocationUpdatesAutomatically = false;

            // iOS 8 has additional permissions requirements
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                Console.WriteLine("GeoClient is running in iOS 8 or higher. 'RequestAlwaysAuthorization' is requested async.");
                _manager.RequestAlwaysAuthorization(); // works in background
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                Console.WriteLine("GeoClient is running in iOS 9 or higher. 'AllowsBackgroundLocationUpdates' is set to true.");
                _manager.AllowsBackgroundLocationUpdates = true;
            }
        }

        public void StartLocationUpdates()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                _manager.DesiredAccuracy = CLLocation.AccuracyBest;
                _manager.LocationsUpdated += (sender, e) =>
                {
                    Console.WriteLine("Got a location update from location manager.");
                    if (e.Locations.Length > 0)
                    {
                        var lastLocation = e.Locations[e.Locations.Length - 1];
                        InformRegistryAboutLocationUpdate(lastLocation);
                    } else
                    {
                        Console.WriteLine("Got a location update without locations!");
                    }
                };

                Console.WriteLine("Requesting start of location updates from underlying location manager.");
                _manager.StartUpdatingLocation();
            }
            else
            {
                Console.Write("Location updates are not enabled!");
            }
        }

        private void InformRegistryAboutLocationUpdate(CLLocation lastLocation)
        {
            Location updatedLocation = CreateXamarinLocation(lastLocation);
            _locationChangeRegistry.LocationUpdated(updatedLocation);
        }

        private static Location CreateXamarinLocation(CLLocation lastLocation)
        {
            return new Location(lastLocation.Coordinate.Latitude, lastLocation.Coordinate.Longitude, DateTimeOffset.Now)
            {
                Accuracy = lastLocation.HorizontalAccuracy,
                Altitude = lastLocation.Altitude,
                Speed = lastLocation.Speed
            };
        }
    }
}