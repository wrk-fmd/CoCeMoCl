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
        public Color ForegroundColor => GetForegroundColor();
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
            Color _blue = System.Drawing.Color.FromArgb(72, 147, 216);
            return Blue ? _blue : Color.Default;
        }

        private Color GetForegroundColor()
        {
            return Blue ? Color.White : Color.Black;
        }

        private string GetOwnTaskStateIcon()
        {
            return StatusIconResolver.GetIconForTaskState(OwnTaskState);
        }
    }
}