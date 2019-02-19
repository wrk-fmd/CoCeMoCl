using System.Collections.Generic;
using GeoClient.Models;

namespace GeoClient.Services.Boundary
{
    public interface IIncidentUpdateListener
    {
        void IncidentsUpdated(List<IncidentItem> updatedIncidents);

        void IncidentsInvalidated();
    }
}
