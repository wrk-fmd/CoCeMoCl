using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using GeoClient.Services.Location;

namespace GeoClient.Droid.Location
{
    [Service]
    public class AndroidLocationService : Service
    {
        private const string LoggerTag = "AndroidLocationService";
        private const string WakeLockTag = "cocemocl:locationServiceWakeLock";

        private const int RunningServiceNotificationId = 144;
        private const string NotificationChannelId = "at.wrk.fmd.geo.location.channel";

        private readonly LocationChangeRegistry _locationChangeRegistry;
        private readonly PowerManager _powerManager;
        private readonly LocationProviderFactory _locationProviderFactory;

        private ILocationProvider _locationProvider;

        public AndroidLocationService()
        {
            _powerManager = Application.Context.GetSystemService(PowerService) as PowerManager;
            _locationChangeRegistry = LocationChangeRegistry.Instance;
            _locationProviderFactory = new LocationProviderFactory();
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

            RemoveLocationProviderIfPresent();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(LoggerTag, "LocationService received start command");

            // Check if device is running Android 8.0 or higher and call StartForeground() if so
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notification = BuildForegroundServiceNotification();
                CreateForegroundServiceNotificationChannel();
                StartForeground(RunningServiceNotificationId, notification);
            }

            return StartCommandResult.Sticky;
        }

        private void OnLocationChanged(Xamarin.Essentials.Location updatedLocation)
        {
            Log.Debug(LoggerTag, "Got a location update from location provider. location: " + updatedLocation);
            var wakeLock = _powerManager.NewWakeLock(WakeLockFlags.Partial, WakeLockTag);
            wakeLock.Acquire();
            Log.Debug(LoggerTag, "Acquired wake lock in android location service.");

            if (updatedLocation != null)
            {
                _locationChangeRegistry.LocationUpdated(updatedLocation);
            }
            else
            {
                Log.Info(LoggerTag, "Updated location was null!");
            }

            wakeLock.Release();
            Log.Debug(LoggerTag, "Released wake lock in android location service.");
        }

        private void CreateForegroundServiceNotificationChannel()
        {
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;

            var notificationChannel = new NotificationChannel(
                NotificationChannelId,
                "Location Notification",
                NotificationImportance.Low);
            notificationChannel.SetShowBadge(false);

            notificationManager.CreateNotificationChannel(notificationChannel);
        }

        private Notification BuildForegroundServiceNotification()
        {
            var pendingIntent = BuildMainActivityPendingIntent();

            var notification = new NotificationCompat.Builder(this, NotificationChannelId)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(Resources.GetString(Resource.String.notification_text))
                .SetSmallIcon(Resource.Mipmap.notification_bar_icon)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.web_hi_res_512))
                .SetColor(Resource.Color.colorPrimary)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent)
                .Build();
            return notification;
        }

        private PendingIntent BuildMainActivityPendingIntent()
        {
            var pendingIntentFlags = PendingIntentFlags.UpdateCurrent;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                pendingIntentFlags |= PendingIntentFlags.Mutable;
            }

            var resultIntent = new Intent(Application.Context, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity(this, 0, resultIntent, pendingIntentFlags);
            return pendingIntent;
        }

        private void StartLocationUpdates()
        {
            RemoveLocationProviderIfPresent();

            AssignNewLocationProvider();
            _locationProvider.StartLocationProvider();
        }

        private void AssignNewLocationProvider()
        {
            _locationProvider = _locationProviderFactory.CreateLocationProvider();
            _locationProvider.RegisterLocationUpdateDelegate(OnLocationChanged);
        }

        private void RemoveLocationProviderIfPresent()
        {
            if (_locationProvider != null)
            {
                var oldProvider = _locationProvider;
                _locationProvider = null;

                oldProvider.StopLocationProvider();
            }
        }
    }
}