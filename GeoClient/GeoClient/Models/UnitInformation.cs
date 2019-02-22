namespace GeoClient.Models
{
    public class UnitInformation
    {
        public string UnitId { get; }
        public string UnitName { get; }

        public UnitInformation(string unitId, string unitName)
        {
            UnitId = unitId ?? "";
            UnitName = unitName ?? "";
        }
    }
}
