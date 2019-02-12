
using GeoClient.Models;

namespace GeoClient.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public IncidentItem IncidentItem { get; set; }
        public ItemDetailViewModel(IncidentItem incidentItem = null)
        {
            // TODO create a meaningful title
            Title = incidentItem?.Type.ToString();
            IncidentItem = incidentItem;
        }
    }
}
