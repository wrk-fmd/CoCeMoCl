using GeoClient.Services;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage, ILocationListener
    {

        public AboutPage()
        {
            InitializeComponent();
            LocationService.Instance.RegisterListener(this);
        }

        protected override void OnAppearing()
        {
            if (RegistrationService.Instance.IsRegistered())
            {
                displayRegistrationInfo();
            } 
        }

        async void registerDevice_Clicked(object sender, EventArgs e)
        {
            #if __ANDROID__
	            // Initialize the scanner first so it can track the current context
	            MobileBarcodeScanner.Initialize (Application);
            #endif
            var scanPage = new ZXingScannerPage();
            // Navigate to our scanner page
            await Navigation.PushAsync(scanPage);
            scanPage.OnScanResult += (result) =>
            {
                // Stop scanning
                scanPage.IsScanning = false;

                // Pop the page and show the result
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PopAsync();
                    await DisplayAlert("Registrierung erfolgreich", "Dieses Gerät ist nun erfolgreich registriert.", "OK");
                    RegistrationService.Instance.SetRegistrationInfo(result.Text);
                    if (RegistrationService.Instance.IsRegistered())
                    {
                        displayRegistrationInfo();
                    }
                });
            };
        }

        void displayRegistrationInfo()
        {
            registrationinfo.Text = "Dieses Gerät hat die ID " + RegistrationService.Instance.GetId() + " mit dem Token " + RegistrationService.Instance.GetToken();
            registrationButton.Text = "Erneut registrieren / Zu anderer Einheit zuordnen";
        }

        public void LocationUpdated(Location updatedLocation)
        {
            if (updatedLocation != null)
            {
                lblLatitude.Text = "Latitude: " + updatedLocation.Latitude.ToString();
                lblLongitude.Text = "Longitude:" + updatedLocation.Longitude.ToString();
                lblAccuracy.Text = "Accuracy: " + updatedLocation.Accuracy.ToString();
            } else
            {
                Console.WriteLine("Updated location is null.");
            }
        }
    }
}