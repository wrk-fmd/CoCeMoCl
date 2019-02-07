using Android.Content;
using Android.OS;
using Android.Util;
using GeoClient.Services;

namespace GeoClient.Droid.Location
{
    [BroadcastReceiver]
    public class AndroidLocationSender : BroadcastReceiver
    {
        private const string LoggerTag = "AndroidLocationSender";

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug(LoggerTag, "OnReceive called.");

            PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
            PowerManager.WakeLock wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, LoggerTag);
            wakeLock.Acquire();

            Log.Debug(LoggerTag, "Running code with wake lock.");
            LocationService.Instance.GetLocationSync();

            Log.Debug(LoggerTag, "Releasing wake lock.");
            wakeLock.Release();
        }
    }
}