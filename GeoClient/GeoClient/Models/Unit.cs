using GeoClient.Services.Utils;
using System;

namespace GeoClient.Models
{
    public class Unit : IComparable<Unit>
    {
        public string Id { get; }
        public string Name { get; }

        // TODO: This is actually a position, not only a point. There is more detailed information available.
        public GeoPoint LastPoint { get; }
        public IncidentTaskState State { get; }

        public string TaskStateIcon => GetTaskStateIcon();

        public Unit(
            string id,
            string name,
            GeoPoint lastPoint = null,
            IncidentTaskState state = IncidentTaskState.Unknown)
        {
            Id = id;
            Name = name;
            LastPoint = lastPoint;
            State = state;
        }

        public int CompareTo(Unit other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        protected bool Equals(Unit other)
        {
            return string.Equals(Id, other.Id)
                   && string.Equals(Name, other.Name)
                   && Equals(LastPoint, other.LastPoint)
                   && State == other.State;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Unit) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LastPoint != null ? LastPoint.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) State;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(LastPoint)}: {LastPoint}, {nameof(State)}: {State}";
        }

        private string GetTaskStateIcon()
        {
            return StatusIconResolver.GetIconForTaskState(State);
        }
    }
}