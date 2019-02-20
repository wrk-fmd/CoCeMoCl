using System.ComponentModel;

namespace GeoClient.Models
{
    public enum IncidentTaskState
    {
        [Description("Alarmiert")]
        Assigned,

        [Description("Zum Berufungsort")]
        Zbo,

        [Description("Am Berufungsort")]
        Abo,

        [Description("Zum Abgabeort")]
        Zao,

        [Description("Am Abgabeort")]
        Aao,

        [Description("Unbekannt")]
        Unknown
    }
}
