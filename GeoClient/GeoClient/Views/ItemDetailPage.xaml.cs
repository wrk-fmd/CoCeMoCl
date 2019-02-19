using GeoClient.Models;
using GeoClient.ViewModels;
using GeoClient.Views.Utils;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        private async void openLocation_Clicked(object sender, EventArgs e)
        {
            var item = viewModel.IncidentItem;

            var geoUri = GeoPointUtil.CreateGeoUri(item.Location, "GeoClient: Berufungsort");

            if (geoUri != null)
            {
                Device.OpenUri(geoUri);
            }
            else
            {
                await DisplayAlert("Adresse nicht verortet", "Die Adresse konnte leider nicht automatisch verortet werden.", "OK");
            }
        }
    }
}