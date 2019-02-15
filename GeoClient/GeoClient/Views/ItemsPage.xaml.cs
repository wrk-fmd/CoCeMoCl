using GeoClient.Models;
using GeoClient.ViewModels;
using System;
using GeoClient.Services.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

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
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

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