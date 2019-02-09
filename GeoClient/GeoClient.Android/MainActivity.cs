using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using GeoClient.Droid.Location;
using GeoClient.Services.Utils;
using System.Threading.Tasks;
using GeoClient.Services.Registration;

namespace GeoClient.Droid
{
    [Activity(Label = "GeoClient", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IGeoRegistrationListener
    {
        private const string LoggerTag = "MainActivity";

        private static readonly int RequestLocationPermissionCode = 1000;
        private static readonly string[] RequiredLocationPermissions = {Manifest.Permission.AccessFineLocation};

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            PerformXamarinStartup(savedInstanceState);

            InterceptedTaskSchedulerWakeLock.BindWakeLocksToInterceptorTaskScheduler();
            PrerequisitesChecking.IsDataSaverBlockingBackgroundData = IsDataSaverEnabled;
            RegistrationService.Instance.RegisterListener(this);

            RequestBatteryOptimizationWhitelisting();
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
                    StartLocationService();
                }
                else
                {
                    Log.Warn(LoggerTag, "User did not grant permission for the location.");
                }
        }

        private bool IsDataSaverEnabled()
        {
            var connectivityManager = (ConnectivityManager) GetSystemService(Context.ConnectivityService);
            bool dataSaverEnabled;

            switch (connectivityManager.RestrictBackgroundStatus)
            {
                case RestrictBackgroundStatus.Enabled:
                    // Background data usage and push notifications are blocked for this app
                    dataSaverEnabled = true;
                    break;

                case RestrictBackgroundStatus.Whitelisted:
                case RestrictBackgroundStatus.Disabled:
                default:
                    // Data Saver is disabled or the app is whitelisted  
                    dataSaverEnabled = false;
                    break;
            }

            return dataSaverEnabled;
        }

        private void PerformXamarinStartup(Bundle savedInstanceState)
        {
            Log.Debug(LoggerTag, "Starting main activity of android specific implementation.");
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        private void InitializeLocationChangeHandling()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) ==
                (int) Permission.Granted)
            {
                Log.Debug(LoggerTag, "User already has granted permission.");
                StartLocationService();
            }
            else
            {
                Log.Debug(LoggerTag, "Have to request permission from the user. ");
                RequestLocationPermission();
            }
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
                            ActivityCompat.RequestPermissions(this, RequiredLocationPermissions,
                                RequestLocationPermissionCode);
                        }
                    ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, RequiredLocationPermissions, RequestLocationPermissionCode);
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
                    intent.SetAction(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
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
        }

        public void GeoServerUnregistered()
        {
            StopLocationService();
        }
    }
}