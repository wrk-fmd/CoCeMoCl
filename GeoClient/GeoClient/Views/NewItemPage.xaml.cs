using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using GeoClient.Models;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewItemPage : ContentPage
    {
        public IncidentItem IncidentItem { get; set; }

        public NewItemPage()
        {
            InitializeComponent();

            IncidentItem = new IncidentItem(Guid.NewGuid().ToString())
            {
                Type = GeoIncidentType.Relocation,
                Info = "This is an item description."
            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", IncidentItem);
            await Navigation.PopModalAsync();
        }
    }
}