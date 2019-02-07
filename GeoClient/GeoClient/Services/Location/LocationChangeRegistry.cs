using System;
using System.Collections.Generic;
using System.Text;

namespace GeoClient.Services.Location
{
    public class LocationChangeRegistry : ILocationListener
    {
        private readonly List<ILocationListener> _locationListeners;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static LocationChangeRegistry()
        {
        }

        private LocationChangeRegistry()
        {
            _locationListeners = new List<ILocationListener>();
        }

        public static LocationChangeRegistry Instance { get; } = new LocationChangeRegistry();

        public void RegisterListener(ILocationListener listener)
        {
            Console.WriteLine("Registering a new location listener.");
            _locationListeners.Add(listener);
        }

        public void LocationUpdated(Xamarin.Essentials.Location updatedLocation)
        {
            _locationListeners.ForEach(listener => listener.LocationUpdated(updatedLocation));
        }
    }
}
