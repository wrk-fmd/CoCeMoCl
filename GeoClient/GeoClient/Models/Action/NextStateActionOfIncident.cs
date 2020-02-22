using System;

namespace GeoClient.Models
{
    public class NextStateActionOfIncident
    {
        public Uri Url { get; }
        public OneTimeActionTaskState PlannedState { get; }

        public NextStateActionOfIncident(Uri url, OneTimeActionTaskState plannedState)
        {
            Url = url;
            PlannedState = plannedState;
        }
    }
}
