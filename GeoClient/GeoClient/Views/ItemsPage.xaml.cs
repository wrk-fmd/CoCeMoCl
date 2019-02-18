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
        IncidentsViewModel viewModel;
        RestService restService;
        private readonly RegistrationService _registrationService;

        public ItemsPage()
        {
            InitializeComponent();
        
            _registrationService = RegistrationService.Instance;
            restService = RestService.Instance;
            
            BindingContext = viewModel = new IncidentsViewModel();
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
            if (_registrationService.IsRegistered())
            {
                restService.GetScope();
                getIncidents();
            }
            else
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
            viewModel.LoadItemsCommand.Execute(null);
        }
    }
}   