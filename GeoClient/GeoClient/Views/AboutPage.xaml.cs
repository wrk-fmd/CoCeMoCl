﻿using GeoClient.Services;
using System;
using System.Threading.Tasks;
using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using GeoClient.Services.Boundary;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage, ILocationUpdateListener
    {
        private readonly RegistrationService _registrationService;

        public AboutPage()
        {
            _registrationService = RegistrationService.Instance;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (_registrationService.IsRegistered())
            {
                DisplayRegistrationInfo();
            }

            LocationChangeRegistry.Instance.RegisterListener(this);
        }

        protected override void OnDisappearing()
        {
            LocationChangeRegistry.Instance.UnregisterListener(this);
        }

        private async void registerDevice_Clicked(object sender, EventArgs e)
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
                    _registrationService.SetRegistrationInfo(result.Text);
                    await UpdateRegistrationInformation(false);
                });
            };
        }

        void unregisterDevice_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                _registrationService.SetRegistrationInfo(null);
                await UpdateRegistrationInformation(true);
            });
        }

        private async Task UpdateRegistrationInformation(bool unregisteredOnPurpose)
        {
            if (_registrationService.IsRegistered())
            {
                await DisplayAlert("Registrierung erfolgreich", "Dieses Gerät ist nun erfolgreich registriert.", "OK");
                DisplayRegistrationInfo();
            }
            else if (unregisteredOnPurpose)
            {
                await DisplayAlert("Registrierung gelöscht", "Registrierung entfernt.", "OK");
                ResetRegistrationInfo();
            } else
            {
                await DisplayAlert("Registrierung fehlgeschlagen", "Es wurde keine gültige Registrierungs URL gefunden.", "OK");
                ResetRegistrationInfo();
            }
        }

        private void ResetRegistrationInfo()
        {
            registrationinfo.Text = "Dieser Client ist derzeit keiner Einheit zugeordnet. Bitte registrieren Sie das Gerät mit dem ausgehändigten Informatonsblatt.";
            registrationButton.Text = "Jetzt registrieren";
        }

        private void DisplayRegistrationInfo()
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            registrationinfo.Text = "Dieses Gerät hat die ID " + registrationInfo.Id + " mit dem Token " + registrationInfo.Token;
            registrationButton.Text = "Erneut registrieren / Zu anderer Einheit zuordnen";
        }

        public void LocationUpdated(Location updatedLocation)
        {
            if (updatedLocation != null)
            {
                lblLatitude.Text = "Latitude: " + updatedLocation.Latitude;
                lblLongitude.Text = "Longitude: " + updatedLocation.Longitude;
                lblSpeed.Text = "Speed: " + updatedLocation.Speed;
                lblAccuracy.Text = "Accuracy: " + updatedLocation.Accuracy;
                lblAltitude.Text = "Altitude: " + updatedLocation.Altitude;
            }
            else
            {
                Console.WriteLine("Updated location is null.");
                lblLatitude.Text = "Latitude: N/A";
                lblLongitude.Text = "Longitude: N/A";
                lblSpeed.Text = "Speed: N/A";
                lblAccuracy.Text = "Accuracy: N/A";
                lblAltitude.Text = "Altitude: N/A";
            }
        }
    }
}