﻿using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using GeoClient.Droid.Location;
using GeoClient.Services.Registration;
using GeoClient.Services.Utils;
using Java.Lang;
using System.Threading.Tasks;

namespace GeoClient.Droid
{
    [Activity(
        Label = "GeoClient",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IGeoRegistrationListener
    {
        private const string LoggerTag = "MainActivity";

        private static readonly int RequestLocationPermissionCode = 1000;
        private static readonly string[] RequiredLocationPermissions = { Manifest.Permission.AccessFineLocation };

        private static readonly int RequestCameraPermissionCode = 1001;
        private static readonly string[] RequiredCameraPermissions = { Manifest.Permission.Camera };

        private static readonly int RequestNotificationPermissionCode = 1002;
        private static readonly string[] RequiredNotificationPermissions = { Manifest.Permission.PostNotifications };

        private static readonly long CleanupTimeoutInMilliseconds = 24 * 60 * 60 * 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            PrerequisitesChecking.IsDataSaverBlockingBackgroundData = IsDataSaverEnabled;
            PrerequisitesChecking.IsDeveloperModeActive = IsDeveloperModeActive;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            PerformXamarinStartup(savedInstanceState);

            InterceptedTaskSchedulerWakeLock.BindWakeLocksToInterceptorTaskScheduler();
            RegistrationService.Instance.RegisterListener(this);

            RequestCameraPermissionIfNecessary();
            RequestBatteryOptimizationWhitelisting();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug(LoggerTag, "Main activity is being destroyed.");
            RegistrationService.Instance.UnregisterListener(this);
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(
                requestCode,
                permissions,
                grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestLocationPermissionCode)
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    Log.Info(LoggerTag, "User granted permission for location.");
                    InitializeLocationChangeHandling();
                }
                else
                {
                    Log.Warn(LoggerTag, "User did not grant permission for the location. Resetting registration.");
                    RegistrationService.Instance.SetRegistrationInfo(null);
                }

            if (requestCode == RequestNotificationPermissionCode)
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    Log.Info(LoggerTag, "User granted permission for notification posting.");
                    InitializeLocationChangeHandling();
                }
                else
                {
                    Log.Warn(LoggerTag, "User did not grant permission for notification posting. Resetting registration.");
                    RegistrationService.Instance.SetRegistrationInfo(null);
                }
        }

        private bool IsDataSaverEnabled()
        {
            bool dataSaverEnabled = false;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                var connectivityManager = (ConnectivityManager)GetSystemService(Context.ConnectivityService);

                if (connectivityManager.RestrictBackgroundStatus == RestrictBackgroundStatus.Enabled)
                {
                    dataSaverEnabled = true;
                }
            }

            return dataSaverEnabled;
        }

        private bool IsDeveloperModeActive()
        {
            var intOfDevSetting =
                Settings.Secure.GetInt(ContentResolver, Settings.Global.DevelopmentSettingsEnabled, 0);
            Log.Debug(LoggerTag, "Developer setting enabled returned: " + intOfDevSetting);
            return intOfDevSetting != 0;
        }

        private void PerformXamarinStartup(Bundle savedInstanceState)
        {
            Log.Debug(LoggerTag, "Starting main activity of android specific implementation.");
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            LoadApplication(new App());
        }

        private void InitializeLocationChangeHandling()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu && !IsPermissionGranted(Manifest.Permission.PostNotifications))
            {
                Log.Debug(LoggerTag, "Permission to show foreground notification is missing. Requesting permission...");
                RequestNotificationPermission();
            }
            else if (!IsPermissionGranted(Manifest.Permission.AccessFineLocation))
            {
                Log.Debug(LoggerTag, "Permission to request location information is missing. Requesting permission...");
                RequestLocationPermission();
            }
            else
            {
                Log.Debug(LoggerTag, "All necessary permissions are granted. Starting location service.");
                StartLocationService();
            }
        }

        private bool IsPermissionGranted(string permissionToCheck)
        {
            return ContextCompat.CheckSelfPermission(this, permissionToCheck) == (int)Permission.Granted;
        }

        private void RequestLocationPermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                var layout = FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(layout,
                        Resource.String.permission_location_rationale,
                        Snackbar.LengthIndefinite)
                    .SetAction(Resource.String.ok,
                        delegate
                        {
                            ActivityCompat.RequestPermissions(
                                this,
                                RequiredLocationPermissions,
                                RequestLocationPermissionCode);
                        }
                    ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, RequiredLocationPermissions, RequestLocationPermissionCode);
            }
        }

        private void RequestNotificationPermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.PostNotifications))
            {
                var layout = FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(layout,
                        Resource.String.permission_notification_rationale,
                        Snackbar.LengthIndefinite)
                    .SetAction(Resource.String.ok,
                        delegate
                        {
                            ActivityCompat.RequestPermissions(
                                this,
                                RequiredNotificationPermissions,
                                RequestNotificationPermissionCode);
                        }
                    ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, RequiredNotificationPermissions, RequestNotificationPermissionCode);
            }
        }

        /// <summary>
        /// This is a workaround for the broken implementation of the QR code reader.
        /// </summary>
        private void RequestCameraPermissionIfNecessary()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {
                Log.Debug(LoggerTag, "Camera permission is already granted for app.");
                return;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {
                var layout = FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(layout,
                        Resource.String.permission_camera_rationale,
                        Snackbar.LengthIndefinite)
                    .SetAction(Resource.String.ok,
                        delegate
                        {
                            ActivityCompat.RequestPermissions(
                                this,
                                RequiredCameraPermissions,
                                RequestCameraPermissionCode);
                        }
                    ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, RequiredCameraPermissions, RequestCameraPermissionCode);
            }
        }

        private void RequestBatteryOptimizationWhitelisting()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var packageName = "at.wrk.fmd.cocemocl";

                PowerManager powerManager = Application.Context.GetSystemService(PowerService) as PowerManager;
                if (!powerManager.IsIgnoringBatteryOptimizations(packageName))
                {
                    Log.Debug(LoggerTag, "Request to disable the battery optimizations.");
                    var intent = new Intent();
                    intent.SetAction(Settings.ActionRequestIgnoreBatteryOptimizations);
                    intent.SetData(Uri.Parse("package:" + packageName));
                    StartActivity(intent);
                }
            }
        }

        private void StartLocationService()
        {
            var startServiceIntent = new Intent(Application.Context, typeof(AndroidLocationService));

            new Task(() =>
            {
                Log.Debug("App", "Starting android specific location service.");

                // Check if device is running Android 8.0 or higher and if so, use the newer StartForegroundService() method
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    Application.Context.StartForegroundService(startServiceIntent);
                }
                else // For older versions, use the traditional StartService() method
                {
                    Application.Context.StartService(startServiceIntent);
                }
            }).Start();
        }

        private void StopLocationService()
        {
            var stopServiceIntent = new Intent(Application.Context, typeof(AndroidLocationService));

            new Task(() =>
            {
                Log.Debug("App", "Stopping android specific location service.");
                Application.Context.StopService(stopServiceIntent);
            }).Start();
        }

        public void GeoServerRegistered()
        {
            InitializeLocationChangeHandling();
            RequestAlarmForCleanup();
        }

        public void GeoServerUnregistered()
        {
            StopLocationService();
            CancelAlarmForCleanup();
        }

        private void RequestAlarmForCleanup()
        {
            var pendingIntent = CreatePendingIntentForCleanup();
            var alarmManager = GetAlarmManager();
            // Previous alarm has to be canceled explicitly, because cancel_current option did not work out.
            alarmManager.Cancel(pendingIntent);
            alarmManager.Set(
                AlarmType.RtcWakeup,
                JavaSystem.CurrentTimeMillis() + CleanupTimeoutInMilliseconds,
                pendingIntent);
        }

        private void CancelAlarmForCleanup()
        {
            var pendingIntent = CreatePendingIntentForCleanup();
            GetAlarmManager().Cancel(pendingIntent);
        }

        private AlarmManager GetAlarmManager()
        {
            var alarmManager = GetSystemService(AlarmService) as AlarmManager;
            return alarmManager;
        }

        private PendingIntent CreatePendingIntentForCleanup()
        {
            var pendingIntentFlags = PendingIntentFlags.UpdateCurrent;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                pendingIntentFlags |= PendingIntentFlags.Mutable;
            }

            var alarmIntent = new Intent(this, typeof(RegistrationCleanupBroadcastReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(this, 0, alarmIntent, pendingIntentFlags);
            return pendingIntent;
        }
    }
}