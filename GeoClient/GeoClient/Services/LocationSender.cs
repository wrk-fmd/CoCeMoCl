using System;
using Xamarin.Forms;

namespace GeoClient.Services
{
    public class LocationSender
    {
        private static readonly TimeSpan IntervalOfLocationSending = TimeSpan.FromSeconds(20);

        private bool _timerRunning;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LocationSender()
        {
        }

        private LocationSender()
        {
        }

        public static LocationSender Instance { get; } = new LocationSender();

        public void StartTimer()
        {
            bool isRunning;
            lock (this)
            {
                isRunning = _timerRunning;
                if (!isRunning)
                    _timerRunning = true;
            }

            if (!isRunning)
            {
                Console.WriteLine("No timer for location updates running. New timer is started.");
                Device.StartTimer(IntervalOfLocationSending, TriggerLocationUpdate);
            }
        }

        private bool TriggerLocationUpdate()
        {
            LocationService.Instance.TriggerLocationUpdateAsync();
            var isRegistered = RegistrationService.Instance.IsRegistered();

            if (!isRegistered)
            {
                Console.WriteLine("Geo server is no longer registered. Location updates are stopped.");
                _timerRunning = false;
            }

            return isRegistered;
        }
    }
}