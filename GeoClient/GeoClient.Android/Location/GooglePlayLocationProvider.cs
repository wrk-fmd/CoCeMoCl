﻿using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.OS;
using Android.Util;

namespace GeoClient.Droid.Location
{
    public class GooglePlayLocationProvider : LocationCallback, ILocationProvider
    {
        private const string LoggerTag = "GooglePlayLocationProvider";

        private LocationProviderListener _updateDelegate = (location) =>
        {
            Log.Warn(LoggerTag, "No location update delegate registered!");
        };

        private readonly IFusedLocationProviderClient _fusedLocationClient;

        public GooglePlayLocationProvider()
        {
            _fusedLocationClient = LocationServices.GetFusedLocationProviderClient(Application.Context);
        }

        public void RegisterLocationUpdateDelegate(LocationProviderListener updateDelegate)
        {
            _updateDelegate = updateDelegate;
        }

        public void StartLocationProvider()
        {
            var locationRequest = new LocationRequest.Builder(15000)
                .SetPriority(Priority.PriorityHighAccuracy)
                .SetMinUpdateIntervalMillis(5000)
                .Build();
            _fusedLocationClient.RequestLocationUpdates(locationRequest, this, Looper.MainLooper);
        }

        public void StopLocationProvider()
        {
            _fusedLocationClient.RemoveLocationUpdates(this);
        }

        public override void OnLocationResult(LocationResult location)
        {
            if (location != null)
            {
                var updatedLocation = XamarinLocationFactory.CreateXamarinLocation(location.LastLocation);
                _updateDelegate.Invoke(updatedLocation);
            }
            else
            {
                Log.Debug(LoggerTag, "Received empty location update from Google Play provider!");
            }
        }
    }
}