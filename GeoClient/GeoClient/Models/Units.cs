using System;

namespace GeoClient.Models
{
    public class Unit : IComparable<Unit>
    {
        public string Id { get; }
        public string Name { get; set; }

        // TODO: Code-Smell: This is actually a position, not only a point. There is more detailed information available.
        public GeoPoint LastPoint { get; set; }
        public IncidentTaskState State { get; set; }

        public Unit(string id)
        {
            Id = id;
        }

        public int CompareTo(Unit other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}