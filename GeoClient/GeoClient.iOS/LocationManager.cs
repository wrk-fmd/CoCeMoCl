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
                _manager.RequestAlwaysAuthorization(); // works in background
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
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
                    // fire our custom Location Updated event
                    var lastLocation = e.Locations[e.Locations.Length - 1];
                    var updatedLocation = new Location(lastLocation.Coordinate.Latitude, lastLocation.Coordinate.Longitude, DateTimeOffset.Now)
                    {
                        Accuracy = lastLocation.HorizontalAccuracy,
                        Altitude = lastLocation.Altitude,
                        Speed = lastLocation.Speed
                    };

                    _locationChangeRegistry.LocationUpdated(updatedLocation);
                };
                _manager.StartUpdatingLocation();
            }
            else
            {
                Console.Write("Location updates are not enabled!");
            }
        }
    }
}