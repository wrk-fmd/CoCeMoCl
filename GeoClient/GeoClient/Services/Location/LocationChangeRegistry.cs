using System;
using System.Collections.Concurrent;
using GeoClient.Services.Boundary;
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
            RegisterListener(RestService.Instance);
        }

        public static LocationChangeRegistry Instance { get; } = new LocationChangeRegistry();

        public void RegisterListener(ILocationUpdateListener listener)
        {
            if (listener != null)
            {
                Console.WriteLine("Registering a new location listener.");
                _locationListeners.TryAdd(listener, 1);
            }
            else
            {
                Console.WriteLine("Cannot register 'null' as location listener.");
            }
        }

        public void UnregisterListener(ILocationUpdateListener listener)
        {
            if (listener != null)
            {
                Console.WriteLine("Unregistering a location listener.");
                _locationListeners.TryRemove(listener, out _);
            }
            else
            {
                Console.WriteLine("Cannot unregister 'null' as location listener.");
            }
        }

        public void LocationUpdated(Xamarin.Essentials.Location updatedLocation)
        {
            Console.WriteLine("Got " + _locationListeners.Count + " location listener instances to inform about updated location.");
            _locationListeners.ForEach(listener =>
            {
                Console.WriteLine("Informing " + listener.Key.GetType().Name + " about changed location.");
                listener.Key.LocationUpdated(updatedLocation);
            });
        }
    }
}
