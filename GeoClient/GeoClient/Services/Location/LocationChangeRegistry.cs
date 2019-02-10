using System;
using System.Collections.Generic;
using System.Text;

namespace GeoClient.Services.Location
{
    public class LocationChangeRegistry : ILocationUpdateListener
    {
        private readonly List<ILocationUpdateListener> _locationListeners;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LocationChangeRegistry()
        {
        }

        private LocationChangeRegistry()
        {
            _locationListeners = new List<ILocationUpdateListener>();
        }

        public static LocationChangeRegistry Instance { get; } = new LocationChangeRegistry();

        public void RegisterListener(ILocationUpdateListener listener)
        {
            Console.WriteLine("Registering a new location listener.");
            _locationListeners.Add(listener);
        }

        public void LocationUpdated(Xamarin.Essentials.Location updatedLocation)
        {
            Console.WriteLine("Got " + _locationListeners.Count + " to inform about update.");
            _locationListeners.ForEach(listener =>
            {
                Console.WriteLine("Informing " + listener.GetType().Name + " about changed location.");
                listener.LocationUpdated(updatedLocation);
            });
        }
    }
}
