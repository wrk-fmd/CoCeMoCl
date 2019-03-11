using GeoClient.Services.Registration;
using GeoClient.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using GeoClient.Views.Utils;
using Xamarin.Forms;

namespace GeoClient.Models
{
    public class IncidentItem : IComparable<IncidentItem>
    {
        public string Id { get; }
        public GeoIncidentType? Type { get; }
        public string Info { get; }
        public bool Priority { get; }
        public bool Blue { get; }
        public GeoPoint Location { get; }
        public GeoPoint Destination { get; }

        public List<UnitOfIncident> Units { get; }

        public string DescriptiveType => GetDescriptiveType();
        public Color BackgroundColor => GetBackgroundColor();
        public Color ForegroundColor => GetForegroundColor();
        public IncidentTaskState? OwnTaskState => GetOwnTaskState();
        public string OwnTaskStateIcon => GetOwnTaskStateIcon();
        public string OwnTaskStateDescription => OwnTaskState?.GetDescription();
        public bool IsUnitAssignedToTask => GetIsUnitAssignedToTask();

        RegistrationService _registrationService = RegistrationService.Instance;

        public IncidentItem(
            string id,
            GeoIncidentType type = GeoIncidentType.Unknown,
            string info = "",
            bool priority = false,
            bool blue = false,
            GeoPoint location = null,
            GeoPoint destination = null,
            List<UnitOfIncident> units = null)
        {
            Id = id;
            Type = type;
            Info = info;
            Priority = priority;
            Blue = blue;
            Location = location;
            Destination = destination;
            Units = units ?? new List<UnitOfIncident>();
        }

        protected bool Equals(IncidentItem other)
        {
            return string.Equals(Id, other.Id)
                   && Type == other.Type
                   && string.Equals(Info, other.Info)
                   && Priority == other.Priority
                   && Blue == other.Blue
                   && Equals(Location, other.Location)
                   && Equals(Destination, other.Destination)
                   && ListEquals(Units, other.Units);
        }

        private bool ListEquals(List<UnitOfIncident> units, List<UnitOfIncident> otherUnits)
        {
            if (units == null)
                return otherUnits == null;
            if (otherUnits == null)
                return false;
            return units.SequenceEqual(otherUnits);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IncidentItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Type.GetHashCode();
                hashCode = (hashCode * 397) ^ (Info != null ? Info.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Priority.GetHashCode();
                hashCode = (hashCode * 397) ^ Blue.GetHashCode();
                hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Destination != null ? Destination.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Units != null ? Units.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(Type)}: {Type}, {nameof(Info)}: {Info}, {nameof(Priority)}: {Priority}, {nameof(Blue)}: {Blue}, {nameof(Location)}: {Location}, {nameof(Destination)}: {Destination}, Units.Count: {Units.Count}";
        }

        private string GetDescriptiveType()
        {
            string descriptiveType = Blue ? "Einsatz" : "Auftrag";
            switch (Type)
            {
                case GeoIncidentType.Relocation:
                    descriptiveType += " (Standortwechsel)";
                    break;
                case GeoIncidentType.Transport:
                    descriptiveType += " (Abtransport)";
                    break;
            }

            return descriptiveType;
        }

        private Color GetBackgroundColor()
        {
            Color customBlue = System.Drawing.Color.FromArgb(72, 147, 216);
            return Blue ? customBlue : Color.Default;
        }

        private Color GetForegroundColor()
        {
            return Blue ? Color.White : Color.Black;
        }

        private IncidentTaskState GetOwnTaskState()
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            if (registrationInfo?.Id != null)
            {
                foreach (var unit in Units)
                {
                    if (unit.Id == registrationInfo.Id)
                        return unit.State;
                }
            }
            else
            {
                Console.WriteLine("Device is not registered. Own task state cannot be read.");
            }

            return IncidentTaskState.Unknown;
        }

        private string GetOwnTaskStateIcon()
        {
            return StatusIconResolver.GetIconForTaskState(OwnTaskState);
        }

        private bool GetIsUnitAssignedToTask()
        {
            return OwnTaskState != IncidentTaskState.Unknown;
        }

        public int CompareTo(IncidentItem other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            var assignmentComparison = IsUnitAssignedToTask.CompareTo(other.IsUnitAssignedToTask);
            if (assignmentComparison != 0)
                return -assignmentComparison;

            var blueComparison = Blue.CompareTo(other.Blue);
            if (blueComparison != 0)
                return -blueComparison;

            var priorityComparison = Priority.CompareTo(other.Priority);
            if (priorityComparison != 0)
                return -priorityComparison;

            var typeComparison = Nullable.Compare(Type, other.Type);
            if (typeComparison != 0) return typeComparison;

            return string.Compare(Info, other.Info, StringComparison.OrdinalIgnoreCase);
        }
    }
}