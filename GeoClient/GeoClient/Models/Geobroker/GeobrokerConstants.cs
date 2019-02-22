namespace GeoClient.Models.Geobroker
{
    public static class GeobrokerConstants
    {
        public const string ScopeIncidentsProperty = "incidents";
        public const string ScopeUnitsProperty = "units";

        public const string IncidentIdProperty = "id";
        public const string IncidentInfoProperty = "info";
        public const string IncidentTypeProperty = "type";
        public const string IncidentPriorityProperty = "priority";
        public const string IncidentBlueProperty = "blue";
        public const string IncidentLocationProperty = "location";
        public const string IncidentAssignedUnitsProperty = "assignedUnits";

        public const string UnitIdProperty = "id";
        public const string UnitNameProperty = "name";
        public const string UnitLastPointProperty = "lastPoint";

        public const string GeoPointLatitudeProperty = "latitude";
        public const string GeoPointLongitudeProperty = "longitude";

        public const string GeoPositionLatitudeProperty = GeoPointLatitudeProperty;
        public const string GeoPositionLongitudeProperty = GeoPointLongitudeProperty;
        public const string GeoPositionAccuracyProperty = "accuracy";
        public const string GeoPositionTimestampProperty = "timestamp";
    }
}
