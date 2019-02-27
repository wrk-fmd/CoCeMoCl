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
            return IsLocationAvailable() ? "Berufungsort auf Karte anzeigen" : "Berufungsort ist nicht verortet";
        }

        private bool IsLocationAvailable()
        {
            return IncidentItem?.Location != null;
        }
    }
}