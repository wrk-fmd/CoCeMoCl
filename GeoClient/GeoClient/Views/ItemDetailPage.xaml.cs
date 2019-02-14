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
            // TODO create link to open maps application.
            await DisplayAlert("Adresse nicht verortet", "Die Adresse konnte leider nicht automatisch verortet werden.", "OK");
        }
    }
}