using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage, ILocationUpdateListener
    {
        private static readonly CultureInfo GermanCultureInfo = new CultureInfo("de-DE");

        private readonly RegistrationService _registrationService;

        public AboutPage()
        {
            _registrationService = RegistrationService.Instance;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (_registrationService.IsRegistered())
                DisplayRegistrationInfo();
            else
                ResetRegistrationInfo();

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
                ContentSentAt.Text = updatedLocation.Timestamp.LocalDateTime.ToString(GermanCultureInfo);
                ContentAccuracy.Text = updatedLocation.Accuracy?.ToString("F0");
            }
        }

        private async void OpenUnitUrl_Clicked(object sender, EventArgs e)
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            if (registrationInfo != null)
            {
                Device.OpenUri(new Uri(registrationInfo.GetMapViewUrl()));
            }
            else
            {
                await DisplayAlert(
                    "Gerät nicht registriert",
                    "Die App ist nicht registriert. Scanne deinen QR Code um auf die Kartenansicht zugreifen zu können.",
                    "OK");
            }
        }

        private async void RegisterDevice_Clicked(object sender, EventArgs e)
        {
#if __ANDROID__
	            // Initialize the scanner first so it can track the current context
	            MobileBarcodeScanner.Initialize (Application);
#endif
            var scanPage = CreateScannerPage();
            // Navigate to our scanner page
            await Navigation.PushModalAsync(scanPage);
        }

        private void UnregisterDevice_Clicked(object sender, EventArgs e)
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
                await DisplayAlert(
                    "Registrierung erfolgreich",
                    "Dieses Gerät ist nun erfolgreich registriert.",
                    "OK");
                DisplayRegistrationInfo();
            }
            else if (unregisteredOnPurpose)
            {
                await DisplayAlert(
                    "Registrierung gelöscht",
                    "Registrierung entfernt.",
                    "OK");
                ResetRegistrationInfo();
            }
            else
            {
                await DisplayAlert(
                    "Registrierung fehlgeschlagen",
                    "Es wurde keine gültige Registrierungs URL gefunden.",
                    "OK");
                ResetRegistrationInfo();
            }
        }

        private void ResetRegistrationInfo()
        {
            ContentUnitRegistered.Text = "Nein";
            ContentUnitId.Text = "";
            ContentUnitName.Text = "";
            ContentUnitUrl.Text = "nicht verfügbar";
            RegisterButton.Text = "Jetzt registrieren";
        }

        private void DisplayRegistrationInfo()
        {
            ContentUnitRegistered.Text = "Ja";

            var registrationInfo = _registrationService.GetRegistrationInfo();
            ContentUnitId.Text = registrationInfo?.Id;
            ContentUnitName.Text =
                _registrationService.RegisteredUnitInformation?.UnitName ??
                "Wird von Server abgefragt...";
            ContentUnitUrl.Text = "Im Browser öffnen";

            RegisterButton.Text = "Andere Einheit registrieren";
        }

        private Page CreateScannerPage()
        {
            var barcodeScanningOptions = CreateMobileBarcodeScanningOptions();
            var scanOverlay = new ZXingDefaultOverlay
            {
                ShowFlashButton = false,
                TopText = "QR Code wird gesucht...",
                BottomText = string.Empty
            };
            scanOverlay.BindingContext = scanOverlay;

            var scanPage = new ZXingScannerPage(barcodeScanningOptions, scanOverlay);
            scanPage.OnScanResult += result => HandleScanResult(scanPage, result);

            return scanPage;
        }

        private void HandleScanResult(ZXingScannerPage scanPage, Result result)
        {
            // Stop scanning
            scanPage.IsScanning = false;

            // Pop the page and show the result
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PopModalAsync();
                _registrationService.SetRegistrationInfo(result.Text);
                await UpdateRegistrationInformation(false);
            });
        }

        private static MobileBarcodeScanningOptions CreateMobileBarcodeScanningOptions()
        {
            return new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.QR_CODE
                }
            };
        }
    }
}