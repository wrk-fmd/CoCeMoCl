using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using GeoClient.Models;
using GeoClient.Views;
using GeoClient.Services.Boundary;
using GeoClient.Services.Utils;

namespace GeoClient.ViewModels
{
    public class IncidentsViewModel : BaseViewModel
    {
        public ObservableCollection<IncidentItem> Incidents { get; set; }
        public Command LoadItemsCommand { get; set; }
        RestService restService = RestService.Instance;

        public IncidentsViewModel()
        {
            Title = "Aufträge / Einsätze";
            Incidents = new ObservableCollection<IncidentItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            
            MessagingCenter.Subscribe<ItemsPage, IncidentItem>(this, "AddItem", async (obj, item) =>
            {
                await DataStore.GetItemsAsync();
            });            
        }

        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Incidents.Clear();
                var incidents = await DataStore.GetItemsAsync(true);
                foreach(IncidentItem incident in incidents)
                {
                    Incidents.Add(incident);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}