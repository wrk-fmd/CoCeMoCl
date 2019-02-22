using Android.App;
using Android.Gms.Common;
using Android.Util;

namespace GeoClient.Droid.Location
{
    public class LocationProviderFactory
    {
        private const string LoggerTag = nameof(LocationProviderFactory);

        public ILocationProvider CreateLocationProvider()
        {
            ILocationProvider locationProvider;

            if (IsGooglePlayServicesInstalled())
            {
                locationProvider = new GooglePlayLocationProvider();
            }
            else
            {
                locationProvider = new NativeAndroidLocationProvider();
            }

            return locationProvider;
        }

        private bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info(LoggerTag, "Google Play Services is installed on this device. Will be used as location provider.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error(LoggerTag, "There is a problem with Google Play Services on this device: {0} - {1}",
                    queryResult, errorString);
            }
            else
            {
                Log.Error(LoggerTag, "Unkown error while checking if Google Play Services are installed.");
            }

            return false;
        }
    }
}