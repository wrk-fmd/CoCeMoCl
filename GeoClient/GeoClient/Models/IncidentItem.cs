using System;
using System.Collections.Generic;
using GeoClient.Services.Utils;
using Xamarin.Forms;
using System.Linq;
using GeoClient.Services.Registration;

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