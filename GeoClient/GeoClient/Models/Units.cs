using System;
using GeoClient.Services.Utils;

namespace GeoClient.Models
{
    public class Unit : IComparable<Unit>
    {
        public string Id { get; }
        public string Name { get; set; }

        // TODO: This is actually a position, not only a point. There is more detailed information available.
        public GeoPoint LastPoint { get; set; }
        public IncidentTaskState State { get; set; }

        public string TaskStateIcon => GetTaskStateIcon();

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

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(LastPoint)}: {LastPoint}, {nameof(State)}: {State}";
        }

        private string GetTaskStateIcon()
        {
            return StatusIconResolver.GetIconForTaskState(State);
        }
    }
}