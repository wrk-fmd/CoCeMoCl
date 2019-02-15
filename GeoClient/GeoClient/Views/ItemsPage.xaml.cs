using GeoClient.Models;
using GeoClient.ViewModels;
using System;
using GeoClient.Services.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GeoClient.Services.Boundary;
using GeoClient.Services.Registration;

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
    }
}