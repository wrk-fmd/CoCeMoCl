using Android.App;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.OS;
using Android.Util;

namespace GeoClient.Droid.Location
{
    public class GooglePlayLocationProvider : Java.Lang.Object, ILocationListener, ILocationProvider, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private const string LoggerTag = "GooglePlayLocationProvider";

        private LocationProviderListener _updateDelegate = (location) =>
        {
            Log.Warn(LoggerTag, "No location update delegate registered!");
        };

        private readonly GoogleApiClient _googleApiClient;

        public GooglePlayLocationProvider()
        {
            _googleApiClient = new GoogleApiClient.Builder(Application.Context)
                .AddApi(LocationServices.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();
        }

        public void RegisterLocationUpdateDelegate(LocationProviderListener updateDelegate)
        {
            _updateDelegate = updateDelegate;
        }

        public void StartLocationProvider()
        {
            _googleApiClient.Connect();
        }

        public void StopLocationProvider()
        {
            _googleApiClient.Disconnect();
        }

        public void OnConnected(Bundle connectionHint)
        {
            var locationRequest = new LocationRequest()
                .SetPriority(LocationRequest.PriorityHighAccuracy)
                .SetInterval(10000)
                .SetFastestInterval(5000);
            LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, locationRequest, this);
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Warn(LoggerTag, "Connection of google API client has been suspended!");
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Warn(LoggerTag, "Connection to google API client failed!");
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            if (location != null)
            {
                var updatedLocation = XamarinLocationFactory.CreateXamarinLocation(location);
                _updateDelegate.Invoke(updatedLocation);
            }
            else
            {
                Log.Debug(LoggerTag, "Received empty location update from google play provider!");
            }
        }
    }
}