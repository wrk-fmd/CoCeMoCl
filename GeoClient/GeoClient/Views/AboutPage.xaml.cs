﻿using GeoClient.Services.Boundary;
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

        private const string PrivacyNotificationVersion = "1.0.0";
        private const string PrivacyVersionKey = "lastNotifiedPrivacyStatement";

        private readonly RegistrationService _registrationService;
        private readonly RestService _restService;

        public AboutPage()
        {
            _registrationService = RegistrationService.Instance;
            _restService = RestService.Instance;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            if (_registrationService.IsRegistered())
                DisplayRegistrationInfo();
            else
                ResetRegistrationInfo();

            ResetLastSentLocationText();
            LocationChangeRegistry.Instance.RegisterListener(this);
            ShowPrivacyNotificationIfNecessary();
        }

        protected override void OnDisappearing()
        {
            LocationChangeRegistry.Instance.UnregisterListener(this);
            ResetLastSentLocationText();
        }

        public void LocationUpdated(Location updatedLocation)
        {
            if (updatedLocation != null)
            {
                ContentSentAt.Text = updatedLocation.Timestamp.LocalDateTime.ToString(GermanCultureInfo);
                ContentAccuracy.Text = updatedLocation.Accuracy?.ToString("F0");
            }
        }

        private void ResetLastSentLocationText()
        {
            ContentSentAt.Text = "N/A";
            ContentAccuracy.Text = "N/A";
        }

        private void OpenUnitUrl_Clicked(object sender, EventArgs e)
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            if (registrationInfo != null)
            {
                Launcher.OpenAsync(new Uri(registrationInfo.GetMapViewUrl()));
            }
        }

        private void OpenInfoSheetUrl_Clicked(object sender, EventArgs e)
        {
            var registrationInfo = _registrationService.GetRegistrationInfo();
            if (registrationInfo != null)
            {
                Launcher.OpenAsync(new Uri(registrationInfo.BaseUrl + "/about.html"));
            }
        }

        private async void RegisterDevice_Clicked(object sender, EventArgs e)
        {
            var scanPage = CreateScannerPage();
            // Navigate to our scanner page

            if (IsRunningOnAndroid())
            {
                await Navigation.PushModalAsync(scanPage);
            }
            else
            {
                await Navigation.PushAsync(scanPage);
            }
        }

        private static bool IsRunningOnAndroid()
        {
            return Device.RuntimePlatform == Device.Android;
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
                // Try to load scope the first time here to show the unit name afterwards.
                _restService.GetScope();
                await DisplayAlert(
                    "Registrierung erfolgreich",
                    "Dieses Gerät ist nun erfolgreich registriert und sendet den Standort.",
                    "OK");
                DisplayRegistrationInfo();
            }
            else if (unregisteredOnPurpose)
            {
                ResetRegistrationInfo();
                ResetLastSentLocationText();
                await DisplayAlert(
                    "Registrierung gelöscht",
                    "Die Registrierung wurde vom Gerät entfernt.",
                    "OK");
            }
            else
            {
                ResetRegistrationInfo();
                ResetLastSentLocationText();
                await DisplayAlert(
                    "Registrierung fehlgeschlagen",
                    "Es wurde keine gültige Registrierungs URL gefunden.",
                    "OK");
            }
        }

        private void ResetRegistrationInfo()
        {
            ContentUnitRegistered.Text = "Nein";
            ContentUnitId.Text = "";
            ContentUnitName.Text = "";

            LabelUnitUrl.IsVisible = false;
            LabelInfoSheetUrl.IsVisible = false;

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

            LabelUnitUrl.IsVisible = true;
            LabelInfoSheetUrl.IsVisible = true;

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
                if (IsRunningOnAndroid())
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await Navigation.PopAsync();
                }

                _registrationService.SetRegistrationInfo(result.Text);
                await UpdateRegistrationInformation(false);
            });
        }

        private void ShowPrivacyNotificationIfNecessary()
        {
            SecureStorage.GetAsync(PrivacyVersionKey).ContinueWith(async taskResult =>
            {
                var lastVersion = taskResult.Result;
                if (lastVersion == null || !lastVersion.Equals(PrivacyNotificationVersion))
                {
                    await SecureStorage.SetAsync(PrivacyVersionKey, PrivacyNotificationVersion);
                    await DisplayAlert(
                        "Standortzugriff benötigt",
                        "Diese App dient dazu um die Standortdaten im Hintergrund an einen konfigurierten Server zu schicken.\n\n" +
                        "Das Senden der Daten wird durch einscannen eines QR Codes aktiviert und kann jederzeit beendet werden.\n\n" +
                        "Bitte achte darauf, dass nur QR Codes aus vertrauenswürdigen Quellen gescannt werden. " +
                        "Gemeinsam mit dem QR Code werden vom Betreiber des Servers die Datenschutzbedingungen bekannt gegeben.",
                        "OK");
                }
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