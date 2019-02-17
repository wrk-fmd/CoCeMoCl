using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using GeoClient.Models;
using GeoClient.ViewModels;

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

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new IncidentItem(Guid.NewGuid().ToString())
            {
                Type = GeoIncidentType.Task,
                Info = "This is an test incident description."
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }

        private async void openLocation_Clicked(object sender, EventArgs e)
        {
            var item = viewModel.IncidentItem;

            if (item.Location.Latitude > 0 && item.Location.Longitude > 0)
            {
                #if __IOS__
                    var request = string.Format("maps://maps.google.com/?daddr=" + item.Location.Latitude + "," + item.Location.Longitude + "");
                #else
                    var request = string.Format("http://maps.google.com/?daddr=" + item.Location.Latitude + "," + item.Location.Longitude + "");
                #endif

                Device.OpenUri(new Uri(request));
            }
            else
            {
                await DisplayAlert("Adresse nicht verortet", "Die Adresse konnte leider nicht automatisch verortet werden.", "OK");
            }
        }
    }
}