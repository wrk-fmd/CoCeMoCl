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
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (RegistrationService.Instance.isRegistered())
            {
                displayRegistrationInfo();
                getLocation();
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
                    RegistrationService.Instance.setRegistrationInfo(result.Text);
                    if (RegistrationService.Instance.isRegistered())
                    {
                        displayRegistrationInfo();
                    }
                });
            };
        }
        async void displayRegistrationInfo()
        {
            registrationinfo.Text = "Dieses Gerät hat die ID " + RegistrationService.Instance.getId() + " mit dem Token " + RegistrationService.Instance.getToken();
            registrationButton.Text = "Erneut registrieren / Zu anderer Einheit zuordnen";
        }
        async void getLocation()
        {
            try
            {
                var locationService = new LocationService();

                Location location = await locationService.getLocationAsync();
                if (location != null)
                {
                    lblLatitude.Text = "Latitude: " + location.Latitude.ToString();
                    lblLongitude.Text = "Longitude:" + location.Longitude.ToString();
                    lblAccuracy.Text = "Accuracy: " + location.Accuracy.ToString();
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Faild", fnsEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Faild", pEx.Message, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Faild", ex.Message, "OK");
            }
        }
    }
}