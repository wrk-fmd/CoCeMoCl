using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Util;
using Object = Java.Lang.Object;

namespace GeoClient.Droid.Location
{
    public class NativeAndroidLocationProvider : Object, ILocationProvider, ILocationListener
    {
        private const string LoggerTag = "NativeAndroidLocationProvider";

        private const long MinimumElapsedTimeInMilliseconds = 10000;

        // Only use coarse or fine here! Other values are only for bearing, horizontal accuracy, etc.
        private const Accuracy RequiredAccuracy = Accuracy.Fine;

        private readonly LocationManager _locationManager;

        private LocationProviderListener _locationUpdateDelegate = (location) =>
        {
            Log.Warn(LoggerTag, "No location update delegate registered!");
        };

        public NativeAndroidLocationProvider()
        {
            _locationManager = Application.Context.GetSystemService(Context.LocationService) as LocationManager;
        }

        public void RegisterLocationUpdateDelegate(LocationProviderListener updateDelegate)
        {
            _locationUpdateDelegate = updateDelegate;
        }

        public void StartLocationProvider()
        {
            var locationProvider = GetLocationProvider();
            _locationManager.RequestLocationUpdates(locationProvider, MinimumElapsedTimeInMilliseconds, 0, this);
            Log.Debug(LoggerTag, "Native android location provider is registered for location updates.");
        }

        public void StopLocationProvider()
        {
            _locationManager.RemoveUpdates(this);
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            var updatedLocation = XamarinLocationFactory.CreateXamarinLocation(location);
            _locationUpdateDelegate.Invoke(updatedLocation);
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Warn(LoggerTag, "Location provider was disabled! provider: " + provider);
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Info(LoggerTag, "Location provider was enabled. provider: " + provider);
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Info(
                LoggerTag,
                "Status of location provider changed. provider: " + provider + ", availability: " + status);
        }

        private string GetLocationProvider()
        {
            var locationCriteria = new Criteria
            {
                Accuracy = RequiredAccuracy,
                PowerRequirement = Power.NoRequirement
            };

            var locationProvider = _locationManager.GetBestProvider(locationCriteria, true);
            Log.Debug(LoggerTag, $"You are about to get location updates via {locationProvider}");
            return locationProvider;
        }
    }
}