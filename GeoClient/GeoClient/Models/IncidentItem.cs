using System;
using System.Collections.Generic;
using GeoClient.Services.Utils;
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
        public Dictionary<string, IncidentTaskState> OtherTaskStates { get; set; }
        public IncidentTaskState? OwnTaskState { get; set; }

        public string DescriptiveType => GetDescriptiveType();
        public Color BackgroundColor => GetBackgroundColor();
        public string OwnTaskStateIcon => GetOwnTaskStateIcon();

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
            return Blue ? Color.RoyalBlue : Color.Default;
        }

        private string GetOwnTaskStateIcon()
        {
            return StatusIconResolver.GetIconForTaskState(OwnTaskState);
        }
    }
}