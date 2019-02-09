using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using GeoClient.Services.Location;
using System;
using Android.Graphics;

namespace GeoClient.Droid.Location
{
    [Service]
    public class AndroidLocationService : Service, ILocationListener
    {
        private const string LoggerTag = "AndroidLocationService";
        private const string WakeLockTag = "cocemocl:locationServiceWakeLock";

        private const int RunningServiceNotificationId = 144;
        private const string NotificationChannelId = "at.wrk.fmd.geo.location.channel";

        private const long MinimumElapsedTimeInMilliseconds = 10000;
        // Only use coarse or fine here! Other values are only for bearing, horizontal accuracy, etc.
        private const Accuracy RequiredAccuracy = Accuracy.Fine;

        private readonly LocationManager _locationManager;
        private readonly LocationChangeRegistry _locationChangeRegistry;
        private readonly PowerManager _powerManager;

        public AndroidLocationService()
        {
            _locationManager = Application.Context.GetSystemService(LocationService) as LocationManager;
            _powerManager = Application.Context.GetSystemService(PowerService) as PowerManager;
            _locationChangeRegistry = LocationChangeRegistry.Instance;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(LoggerTag, "Android specific location service was created.");
            StartLocationUpdates();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(LoggerTag, "Service has been terminated");

            _locationManager.RemoveUpdates(this);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(LoggerTag, "LocationService received start command");

            // Check if device is running Android 8.0 or higher and call StartForeground() if so
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notification = new NotificationCompat.Builder(this, NotificationChannelId)
                    .SetContentTitle(Resources.GetString(Resource.String.app_name))
                    .SetContentText(Resources.GetString(Resource.String.notification_text))
                    .SetSmallIcon(Resource.Mipmap.notification_bar_icon)
                    .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.web_hi_res_512))
                    .SetColor(Resource.Color.colorPrimary)
                    .SetOngoing(true)
                    .Build();

                var notificationManager = GetSystemService(NotificationService) as NotificationManager;

                var notificationChannel = new NotificationChannel(
                    NotificationChannelId, 
                    "Location Notification",
                    NotificationImportance.Default);
                notificationChannel.SetShowBadge(false);

                notificationManager.CreateNotificationChannel(notificationChannel);

                StartForeground(RunningServiceNotificationId, notification);
            }

            return StartCommandResult.Sticky;
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            Log.Debug(LoggerTag, "Got a location update from android location manager. location: " + location);
            var wakeLock = _powerManager.NewWakeLock(WakeLockFlags.Partial, WakeLockTag);
            wakeLock.Acquire();
            Log.Debug(LoggerTag, "Acquired wake lock.");

            if (location != null)
            {
                var updatedLocation = new Xamarin.Essentials.Location
                {
                    Accuracy = location.Accuracy,
                    Altitude = location.Altitude,
                    Course = location.Bearing,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Speed = location.Speed,
                    Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(location.Time)
                };

                _locationChangeRegistry.LocationUpdated(updatedLocation);
            }
            else
            {
                Log.Info(LoggerTag, "Updated location was null!");
            }

            wakeLock.Release();
            Log.Debug(LoggerTag, "Released wake lock.");
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
            Log.Info(LoggerTag, "Status of location provider changed. provider: " + provider + ", availability: " + status);
        }

        private void StartLocationUpdates()
        {
            var locationProvider = GetLocationProvider();
            _locationManager.RequestLocationUpdates(locationProvider, MinimumElapsedTimeInMilliseconds, 0, this);
            Log.Debug(LoggerTag, "Service is registered for location updates.");
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