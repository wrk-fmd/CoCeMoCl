using GeoClient.Models;

namespace GeoClient.Services.Utils
{
    public static class StatusIconResolver
    {
        public static string GetIconForTaskState(IncidentTaskState? taskState)
        {
            string resourceName = "unknown.png";

            switch (taskState)
            {
                case IncidentTaskState.Assigned:
                    resourceName = "assigned.png";
                    break;
                case IncidentTaskState.Zbo:
                    resourceName = "zbo.png";
                    break;
                case IncidentTaskState.Abo:
                    resourceName = "abo.png";
                    break;
                case IncidentTaskState.Zao:
                    resourceName = "zao.png";
                    break;
                case IncidentTaskState.Aao:
                    resourceName = "aao.png";
                    break;
            }

            return resourceName;
        }
    }
}