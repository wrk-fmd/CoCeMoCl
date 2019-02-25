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
        private readonly IncidentUpdateRegistry _incidentUpdateRegistry;

        public ItemsPage()
        {
            InitializeComponent();

            _registrationService = RegistrationService.Instance;
            _restService = RestService.Instance;
            _incidentUpdateRegistry = IncidentUpdateRegistry.Instance;

            BindingContext = _viewModel = new IncidentsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is IncidentItem item))
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        private async void RefreshItems_Clicked(object sender, EventArgs e)
        {
            if (_registrationService.IsRegistered())
            {
                _restService.GetScope();
            }
            else
            {
                await DisplayAlert(
                    "Nicht registriert",
                    "Um die Liste mit aktuellen Einsätzen zu aktualisieren, muss das Gerät zuerst registriert werden.",
                    "OK");
            }
        }

        protected override void OnAppearing()
        {
            _incidentUpdateRegistry.RegisterListener(this);

            if (_registrationService.IsRegistered())
            {
                _restService.GetScope();
            }
            else
            {
                IncidentsInvalidated(IncidentInvalidationReason.ClientNoLongerRegistered);
            }

            CheckIfDataSaverIsActive();
        }

        protected override void OnDisappearing()
        {
            _incidentUpdateRegistry.UnregisterListener(this);
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
            var sortedIncidents = updatedIncidents.OrderBy(x => x);

            Device.BeginInvokeOnMainThread(() =>
            {
                _viewModel.EmptyListMessage = CreateEmptyListMessage(IncidentInvalidationReason.EmptyUpdateFromServer);
                _viewModel.Incidents.Clear();

                foreach (var incident in sortedIncidents)
                {
                    _viewModel.Incidents.Add(incident);
                }

                SetBusyIndicationToFalse();
            });
        }

        public void IncidentsInvalidated(IncidentInvalidationReason reason)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _viewModel.EmptyListMessage = CreateEmptyListMessage(reason);
                _viewModel.Incidents.Clear();
                SetBusyIndicationToFalse();
            });
        }

        private string CreateEmptyListMessage(IncidentInvalidationReason reason)
        {
            string message;

            switch (reason)
            {
                case IncidentInvalidationReason.ClientNoLongerRegistered:
                    message = "Gerät ist nicht registriert.";
                    break;
                case IncidentInvalidationReason.UnitNotAvailableOnServer:
                    message = "Derzeit keine Daten verfügbar. Bitte versuche es später erneut.";
                    break;
                case IncidentInvalidationReason.ConnectionError:
                    message = "Verbindungsfehler. Bitte versuche es später erneut.";
                    break;
                default:
                    message = "Keine Aufträge / Einsätze.";
                    break;
            }

            return message;
        }

        private void SetBusyIndicationToFalse()
        {
            _viewModel.IsBusy = false;
        }
    }
}