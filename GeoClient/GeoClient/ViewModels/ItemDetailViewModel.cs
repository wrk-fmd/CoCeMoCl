
using GeoClient.Models;

namespace GeoClient.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public IncidentItem IncidentItem { get; set; }
        public ItemDetailViewModel(IncidentItem incidentItem = null)
        {
            Title = incidentItem?.DescriptiveType;
            IncidentItem = incidentItem;
        }
    }
}
