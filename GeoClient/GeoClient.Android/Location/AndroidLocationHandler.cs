using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using GeoClient.Services;
using static Android.Content.Context;

namespace GeoClient.Droid.Location
{
    public class AndroidLocationHandler : IGeoRegistrationListener
    {
        private const long FirstDelayInMilliseconds = 3000;
        private const long IntervalInMilliseconds = 30000;
        private const string LoggerTag = "AndroidLocationHandler";

        private readonly Context _context;

        public AndroidLocationHandler(Context context)
        {
            _context = context;
            RegistrationService.Instance.registerListener(this);
        }

        public void GeoServerRegistered()
        {
            StartLocationSender();
        }

        public void GeoServerUnregistered()
        {
            StopLocationSender();
        }

        private void StartLocationSender()
        {
            Log.Info(LoggerTag, "Starting pending intent for background location sending via alarm manager.");
            var pendingIntent = CreatePendingIntent();
            GetAlarmManager().SetRepeating(
                AlarmType.ElapsedRealtimeWakeup,
                SystemClock.ElapsedRealtime() + FirstDelayInMilliseconds, 
                IntervalInMilliseconds, 
                pendingIntent);
        }

        private void StopLocationSender()
        {
            Log.Info(LoggerTag, "Cancel pending intent for background location sending.");
            var pendingIntent = CreatePendingIntent();
            GetAlarmManager().Cancel(pendingIntent);
        }

        private AlarmManager GetAlarmManager()
        {
            var alarmManager = _context.GetSystemService(AlarmService).JavaCast<AlarmManager>();
            return alarmManager;
        }

        private PendingIntent CreatePendingIntent()
        {
            var alarmIntent = new Intent(_context, typeof(AndroidLocationSender));
            var pendingIntent = PendingIntent.GetBroadcast(_context, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }
    }
}