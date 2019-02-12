using GeoClient.Models;
using System;

namespace GeoClient.Services.Utils
{
    public static class GeoIncidentTypeFactory
    {
        public static GeoIncidentType GetTypeFromString(string typeString)
        {
            GeoIncidentType incidentType;
            try
            {
                incidentType = (GeoIncidentType) Enum.Parse(typeof(GeoIncidentType), typeString, true);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Tried to convert null to GeoIncidentType!");
                incidentType = GeoIncidentType.Unknown;
            }
            catch (ArgumentException)
            {
                incidentType = GeoIncidentType.Unknown;
            }

            return incidentType;
        }
    }
}
