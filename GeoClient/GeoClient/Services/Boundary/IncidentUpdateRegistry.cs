using GeoClient.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GeoClient.Services.Registration;
using Xamarin.Forms.Internals;

namespace GeoClient.Services.Boundary
{
    public class IncidentUpdateRegistry : IIncidentUpdateListener, IGeoRegistrationListener
    {
        private readonly ConcurrentDictionary<IIncidentUpdateListener, byte> _incidentUpdateListeners;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static IncidentUpdateRegistry()
        {
        }

        private IncidentUpdateRegistry()
        {
            _incidentUpdateListeners = new ConcurrentDictionary<IIncidentUpdateListener, byte>();
            RegistrationService.Instance.RegisterListener(this);
        }

        public static IncidentUpdateRegistry Instance { get; } = new IncidentUpdateRegistry();

        public void RegisterListener(IIncidentUpdateListener listener)
        {
            if (listener != null)
            {
                Console.WriteLine("Registering a new incident update listener.");
                _incidentUpdateListeners.TryAdd(listener, 1);
            }
            else
            {
                Console.WriteLine("Cannot register 'null' as incident update listener.");
            }
        }

        public void UnregisterListener(IIncidentUpdateListener listener)
        {
            if (listener != null)
            {
                Console.WriteLine("Unregistering an incident update listener.");
                _incidentUpdateListeners.TryRemove(listener, out _);
            }
            else
            {
                Console.WriteLine("Cannot unregister 'null' as incident update listener.");
            }
        }

        public void IncidentsUpdated(List<IncidentItem> updatedIncidents)
        {
            Console.WriteLine("Got " + _incidentUpdateListeners.Count +
                              " incident update listener instances to inform about update.");
            _incidentUpdateListeners.ForEach(listener =>
            {
                Console.WriteLine("Informing " + listener.Key.GetType().Name + " about changed incident items.");
                listener.Key.IncidentsUpdated(updatedIncidents);
            });
        }

        public void IncidentsInvalidated(IncidentInvalidationReason reason)
        {
            _incidentUpdateListeners.ForEach(listener => listener.Key.IncidentsInvalidated(reason));
        }

        public void GeoServerRegistered()
        {
        }

        public void GeoServerUnregistered()
        {
            Console.WriteLine("Device was unregistered. Current incidents are invalidated.");
            IncidentsInvalidated(IncidentInvalidationReason.ClientNoLongerRegistered);
        }
    }
}