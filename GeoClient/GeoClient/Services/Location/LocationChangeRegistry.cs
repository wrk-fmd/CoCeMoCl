using System;
using System.Collections.Concurrent;
using Xamarin.Forms.Internals;

namespace GeoClient.Services.Location
{
    public class LocationChangeRegistry : ILocationUpdateListener
    {
        private readonly ConcurrentDictionary<ILocationUpdateListener, byte> _locationListeners;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LocationChangeRegistry()
        {
        }

        private LocationChangeRegistry()
        {
            _locationListeners = new ConcurrentDictionary<ILocationUpdateListener, byte>();
        }

        public static LocationChangeRegistry Instance { get; } = new LocationChangeRegistry();

        public void RegisterListener(ILocationUpdateListener listener)
        {
            Console.WriteLine("Registering a new location listener.");
            _locationListeners.TryAdd(listener, 1);
        }

        public void LocationUpdated(Xamarin.Essentials.Location updatedLocation)
        {
            Console.WriteLine("Got " + _locationListeners.Count + " to inform about update.");
            _locationListeners.ForEach(listener =>
            {
                Console.WriteLine("Informing " + listener.GetType().Name + " about changed location.");
                listener.Key.LocationUpdated(updatedLocation);
            });
        }
    }
}
