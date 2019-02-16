using GeoClient.Models;
using GeoClient.ViewModels;
using System;
using GeoClient.Services.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GeoClient.Services.Boundary;
using GeoClient.Services.Registration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        RestService restService;
        private readonly RegistrationService _registrationService;

        public ItemsPage()
        {
            InitializeComponent();
        
            _registrationService = RegistrationService.Instance;
            restService = RestService.Instance;
            
            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as IncidentItem;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void RefreshItems_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
                       
            if (_registrationService.IsRegistered())
            {
                restService.GetScope();
                getIncidents();
            } else
            {
                await DisplayAlert("Nicht registriert", "Um die Liste mit aktuellen Einsätzen zu aktualisieren, müssen Sie das Gerät zuerst registrieren", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_registrationService.IsRegistered())
            {
                restService.GetScope();
                getIncidents();
            }

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);

            CheckIfDataSaverIsActive();
        }

        private async void CheckIfDataSaverIsActive()
        {
            var isDataSaverBlockingBackgroundData = PrerequisitesChecking.IsDataSaverBlockingBackgroundData();
            if (isDataSaverBlockingBackgroundData)
            {
                await DisplayAlert("Datensparmodus ist aktiv!", "Position kann nicht zuverlässig gesendet werden.", "OK");
            }
        }
        public void getIncidents()
        {
            /*
            if (restService.incidents != null)
            {
                foreach (var _incident in restService.incidents)
                {
                    IncidentItem incident = new IncidentItem((string)_incident["id"]);
                    incident.Info = (string)_incident["info"];
                    incident.Type = GeoIncidentTypeFactory.GetTypeFromString((string)_incident["type"]);
                    incident.Priority = (bool)_incident["priority"];
                    incident.Blue = (bool)_incident["blue"];
                    incident.Location = new GeoPoint((long)_incident["location"]["latitude"], (long)_incident["location"]["longitude"]);
                    //incident.AssignedUnits = (string)_incident["assignedUnits"];
                    Console.WriteLine(incident.ToString());
                    MessagingCenter.Send(this, "AddItem", incident);
                }
            }
            BindingContext = this;
            Console.WriteLine(viewModel.Items.Count);
            Console.WriteLine(viewModel.Items.ToString());
            */
            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}