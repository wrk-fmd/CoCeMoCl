using System.Collections.Generic;
using GeoClient.Models;

namespace GeoClient.Services.Boundary
{
    public enum IncidentInvalidationReason
    {
        /// <summary>
        /// The update from the server did not contain any incidents.
        /// </summary>
        EmptyUpdateFromServer,

        /// <summary>
        /// The client is no longer registered on the server.
        /// </summary>
        ClientNoLongerRegistered,

        /// <summary>
        /// The unit is not available on the server.
        /// </summary>
        UnitNotAvailableOnServer,

        /// <summary>
        /// Unknown connection error.
        /// </summary>
        ConnectionError,
    }

    public interface IIncidentUpdateListener
    {
        void IncidentsUpdated(List<IncidentItem> updatedIncidents);

        void IncidentsInvalidated(IncidentInvalidationReason reason);
    }
}
