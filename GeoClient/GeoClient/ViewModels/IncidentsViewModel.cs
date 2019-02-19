using GeoClient.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using GeoClient.Services.Boundary;
using Xamarin.Forms;

namespace GeoClient.ViewModels
{
    public class IncidentsViewModel : BaseViewModel
    {
        public ObservableCollection<IncidentItem> Incidents { get; set; }
        public Command LoadItemsCommand { get; set; }

        private readonly RestService _restService;

        public IncidentsViewModel()
        {
            Title = "Aufträge / Einsätze";
            Incidents = new ObservableCollection<IncidentItem>();
            LoadItemsCommand = new Command(ExecuteLoadItemsCommand);

            _restService = RestService.Instance;
        }
        
        private void ExecuteLoadItemsCommand()
        {
            if (IsBusy)
            {
                Console.WriteLine("Update of incidents is still busy.");
                return;
            }

            _restService.GetScope();
        }
    }
}