using System;

namespace GeoClient.Models
{
    public class IncidentItem
    {
        public string Id { get; }
        public GeoIncidentType Type { get; set; }
        public string Info { get; set; }
        public bool Priority { get; set; }
        public bool Blue { get; set; }
        public GeoPoint Location { get; set; }

        public IncidentItem(string id)
        {
            Id = id;
        }
    }

}