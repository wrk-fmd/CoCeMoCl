using GeoClient.Models;
using System;

namespace GeoClient.Services.Utils
{
    public static class IncidentTaskStateFactory
    {
        public static IncidentTaskState GetTaskStateFromString(string taskStateString)
        {
            IncidentTaskState taskState;
            try
            {
                taskState = (IncidentTaskState) Enum.Parse(typeof(IncidentTaskState), taskStateString, true);
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Tried to convert null to IncidentTaskState!");
                taskState = IncidentTaskState.Unknown;
            }
            catch (ArgumentException)
            {
                taskState = IncidentTaskState.Unknown;
            }

            return taskState;
        }
    }
}
