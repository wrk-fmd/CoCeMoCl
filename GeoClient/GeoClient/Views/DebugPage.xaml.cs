using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DebugPage : ContentPage, ILocationUpdateListener
    {
        public DebugPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            LocationChangeRegistry.Instance.RegisterListener(this);
        }

        protected override void OnDisappearing()
        {
            LocationChangeRegistry.Instance.UnregisterListener(this);
        }

        public void LocationUpdated(Location updatedLocation)
        {
            if (updatedLocation != null)
            {
                LabelLatitude.Text = "Latitude: " + updatedLocation.Latitude;
                LabelLongitude.Text = "Longitude: " + updatedLocation.Longitude;
                LabelSpeed.Text = "Speed: " + updatedLocation.Speed;
                LabelAccuracy.Text = "Accuracy: " + updatedLocation.Accuracy;
                LabelAltitude.Text = "Altitude: " + updatedLocation.Altitude;
            }
            else
            {
                Console.WriteLine("Updated location is null.");
                LabelLatitude.Text = "Latitude: N/A";
                LabelLongitude.Text = "Longitude: N/A";
                LabelSpeed.Text = "Speed: N/A";
                LabelAccuracy.Text = "Accuracy: N/A";
                LabelAltitude.Text = "Altitude: N/A";
            }
        }
    }
}