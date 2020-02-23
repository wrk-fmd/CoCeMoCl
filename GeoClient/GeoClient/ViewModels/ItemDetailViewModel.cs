using GeoClient.Models;
using Xamarin.Forms;

namespace GeoClient.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        private static readonly Color ActiveButtonColor = Color.FromHex("#B70E0C");
        private static readonly Color DisableButtonColor = Color.Gray;

        public IncidentItem IncidentItem { get; set; }

        public Color OpenLocationButtonColor => GetOpenLocationButtonColor();
        public string OpenLocationButtonText => GetOpenLocationButtonText();
        public bool IsOpenLocationButtonApplicable => IsOpenLocationApplicable();

        public Color OpenDestinationButtonColor => GetOpenDestinationButtonColor();
        public string OpenDestinationButtonText => GetOpenDestinationButtonText();

        public Color SetNextStateButtonColor => GetSetNextStateButtonColor();
        public string SetNextStateButtonText => GetSetNextStateButtonText();
        public bool IsNextStateButtonVisible => IsNextStateActionApplicable();

        public ItemDetailViewModel(IncidentItem incidentItem = null)
        {
            Title = incidentItem?.DescriptiveType;
            IncidentItem = incidentItem;
        }

        private Color GetOpenLocationButtonColor()
        {
            return IsLocationAvailable() ? ActiveButtonColor : DisableButtonColor;
        }

        private string GetOpenLocationButtonText()
        {
            return IsLocationAvailable() ? "Berufungsort auf Karte anzeigen" : "Berufungsort nicht verfügbar";
        }

        private bool IsLocationAvailable()
        {
            return IncidentItem?.Location != null;
        }

        private bool IsOpenLocationApplicable()
        {
            return IncidentItem?.Type != GeoIncidentType.Relocation;
        }

        private Color GetOpenDestinationButtonColor()
        {
            return IsDestinationAvailable() ? ActiveButtonColor : DisableButtonColor;
        }

        private string GetOpenDestinationButtonText()
        {
            return IsDestinationAvailable() ? "Zielort auf Karte anzeigen" : "Zielort nicht verfügbar";
        }

        private bool IsDestinationAvailable()
        {
            return IncidentItem?.Destination != null;
        }

        private Color GetSetNextStateButtonColor()
        {
            return IsNextStateActionAvailable() ? ActiveButtonColor : DisableButtonColor;
        }

        private string GetSetNextStateButtonText()
        {
            return IsNextStateActionAvailable() ? GenerateNextStateButtonText() : "Statusänderung nicht möglich";
        }

        private bool IsNextStateActionApplicable()
        {
            return IncidentItem?.IsUnitAssignedToTask ?? false;
        }

        private string GenerateNextStateButtonText()
        {
            var plannedState = IncidentItem.NextStateAction.PlannedState;
            return plannedState == OneTimeActionTaskState.DETACHED
                ? "Einsatz beenden"
                : plannedState + " setzen";
        }

        private bool IsNextStateActionAvailable()
        {
            return IncidentItem?.NextStateAction != null;
        }
    }
}