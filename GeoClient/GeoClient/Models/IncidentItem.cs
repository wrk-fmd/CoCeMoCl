using GeoClient.Services.Registration;
using GeoClient.Services.Utils;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GeoClient.Models
{
    public class IncidentItem
    {
        public string Id { get; }
        public GeoIncidentType? Type { get; set; }
        public string Info { get; set; }
        public bool Priority { get; set; }
        public bool Blue { get; set; }
        public GeoPoint Location { get; set; }

        public SortedSet<Unit> Units { get; set; }

        public string DescriptiveType => GetDescriptiveType();
        public Color BackgroundColor => GetBackgroundColor();
        public Color ForegroundColor => GetForegroundColor();
        public IncidentTaskState? OwnTaskState => GetOwnTaskState();
        public string OwnTaskStateIcon => GetOwnTaskStateIcon();

        RegistrationService _registrationService = RegistrationService.Instance;

        public IncidentItem(string id)
        {
            Id = id;
        }

        protected bool Equals(IncidentItem other)
        {
            return string.Equals(Id, other.Id)
                   && Type == other.Type
                   && string.Equals(Info, other.Info)
                   && Priority == other.Priority
                   && Blue == other.Blue
                   && Equals(Location, other.Location)
                   && SetEquals(Units, other.Units);
        }

        private bool SetEquals(SortedSet<Unit> units, SortedSet<Unit> otherUnits)
        {
            if (units == null)
                return otherUnits == null;
            if (otherUnits == null)
                return false;
            return units.SetEquals(otherUnits);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
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
                hashCode = (hashCode * 397) ^ (Units != null ? Units.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                $"{nameof(Id)}: {Id}, {nameof(Type)}: {Type}, {nameof(Info)}: {Info}, {nameof(Priority)}: {Priority}, {nameof(Blue)}: {Blue}, {nameof(Location)}: {Location}, Units.Count: {Units.Count}";
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
                foreach (Unit unit in Units)
                {
                    if (unit.Id == registrationInfo.Id)
                    {
                        return unit.State;
                    }
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
    }
}