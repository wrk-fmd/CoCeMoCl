using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Accessibility;
using Android.Widget;
using GeoClient.Services.Boundary;

namespace GeoClient.Droid.Location
{
    public static class RestServiceWakeLock
    {
        public static void BindWakeLocksToRestService()
        {
            const string loggerTag = "RestServiceWakeLock";

            Log.Debug(loggerTag, "Registering wake lock delegates on RestService.");

            RestService.Instance.BeforeLocationSending = () =>
            {
                var powerManager = (PowerManager) Application.Context.GetSystemService(Context.PowerService);
                var wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, loggerTag);
                wakeLock.Acquire();
                Log.Debug(loggerTag, "Acquired wake lock.");

                return () =>
                {
                    wakeLock.Release();
                    Log.Debug(loggerTag, "Released wake lock.");
                };
            };
        }
    }
}