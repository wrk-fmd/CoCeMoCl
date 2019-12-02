using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using GeoClient.Services.Boundary;
using GeoClient.Services.Utils;

namespace GeoClient.Droid.Location
{
    public static class InterceptedTaskSchedulerWakeLock
    {
        private const string LoggerTag = "InterceptedTaskSchedulerWakeLock";
        private const string WakeLockTag = "cocemocl:interceptorWakeLock";

        public static void BindWakeLocksToInterceptorTaskScheduler()
        {
            Log.Debug(LoggerTag, "Registering wake lock delegates on intercepted task scheduler.");

            InterceptedSingleThreadTaskScheduler.AroundTaskExecution = () =>
            {
                var powerManager = (PowerManager)Application.Context.GetSystemService(Context.PowerService);
                var wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, WakeLockTag);
                wakeLock.Acquire();
                Log.Debug(LoggerTag, "Acquired wake lock (interceptor).");

                return () =>
                {
                    wakeLock.Release();
                    Log.Debug(LoggerTag, "Released wake lock (interceptor).");
                };
            };


            RestService.Instance.BeforeLocationSending = () =>
            {
                var powerManager = (PowerManager)Application.Context.GetSystemService(Context.PowerService);
                var wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, WakeLockTag);
                wakeLock.Acquire();
                Log.Debug(LoggerTag, "Acquired wake lock (service).");

                return () =>
                {
                    wakeLock.Release();
                    Log.Debug(LoggerTag, "Released wake lock (service).");
                };
            };
        }
    }
}