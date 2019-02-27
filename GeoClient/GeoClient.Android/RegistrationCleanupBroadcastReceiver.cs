using Android.Content;
using Android.Util;
using GeoClient.Services.Registration;

namespace GeoClient.Droid
{
    [BroadcastReceiver]
    public class RegistrationCleanupBroadcastReceiver : BroadcastReceiver
    {
        private const string LoggerTag = nameof(RegistrationCleanupBroadcastReceiver);

        public override void OnReceive(Context context, Intent intent)
        {
            Log.Info(LoggerTag, "Cleanup was triggered by alarm manager. Removing registration.");
            RegistrationService.Instance.SetRegistrationInfo(null);
        }
    }
}