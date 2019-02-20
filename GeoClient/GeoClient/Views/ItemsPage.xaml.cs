using GeoClient.Models;
using GeoClient.Services.Boundary;
using GeoClient.Services.Registration;
using GeoClient.Services.Utils;
using GeoClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage, IIncidentUpdateListener
    {
        private readonly IncidentsViewModel _viewModel;
        private readonly RestService _restService;
        private readonly RegistrationService _registrationService;

        public ItemsPage()
        {
            InitializeComponent();

            _registrationService = RegistrationService.Instance;
            _restService = RestService.Instance;

            BindingContext = _viewModel = new IncidentsViewModel();
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
                _restService.GetScope();
            }
            else
            {
                await DisplayAlert("Nicht registriert",
                    "Um die Liste mit aktuellen Einsätzen zu aktualisieren, müssen Sie das Gerät zuerst registrieren",
                    "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            IncidentUpdateRegistry.Instance.RegisterListener(this);

            if (_registrationService.IsRegistered())
            {
                _restService.GetScope();
            }

            CheckIfDataSaverIsActive();
        }

        private async void CheckIfDataSaverIsActive()
        {
            var isDataSaverBlockingBackgroundData = PrerequisitesChecking.IsDataSaverBlockingBackgroundData();
            if (isDataSaverBlockingBackgroundData)
            {
                await DisplayAlert(
                    "Datensparmodus ist aktiv!",
                    "Position kann nicht zuverlässig gesendet werden.",
                    "OK");
            }
        }

        public void IncidentsUpdated(List<IncidentItem> updatedIncidents)
        {
            IncidentsInvalidated();

            var sortedIncidents = updatedIncidents.OrderBy(x => x);

            foreach (var incident in sortedIncidents)
            {
                _viewModel.Incidents.Add(incident);
            }

            SetBusyIndicationToFalse();
        }

        public void IncidentsInvalidated()
        {
            _viewModel.EmptyListMessage = "Keine Aufträge / Einsätze.";
            _viewModel.Incidents.Clear();
            SetBusyIndicationToFalse();
        }

        private void SetBusyIndicationToFalse()
        {
            Device.BeginInvokeOnMainThread(() => _viewModel.IsBusy = false);
        }
    }
}