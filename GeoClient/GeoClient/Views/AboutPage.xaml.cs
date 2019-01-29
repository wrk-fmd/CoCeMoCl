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
        private string _url;
        private string _id;
        private string _token;

        public AboutPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            try
            {
                var _url = await SecureStorage.GetAsync("url");
                if (_url != null)
                {
                    getRegistrationInfo(_url);
                }
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
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
                    await SecureStorage.SetAsync("url", result.Text);                    
                    getRegistrationInfo(result.Text);
                });
            };
        }
        async void getRegistrationInfo(string url)
        {
            //hacky way to get ID & Token
            var idpos = url.IndexOf("id=");
            var tokenpos = url.IndexOf("&token=");
            var id = url.Substring(idpos + 3, tokenpos - idpos -3);
            var token = url.Substring(tokenpos + 4);
            await SecureStorage.SetAsync("id", id);
            await SecureStorage.SetAsync("token", token);
            registrationinfo.Text = "Dieses Gerät hat die ID " + id + " mit dem Token " + token;
            registrationButton.Text = "Erneut registrieren / Zu anderer Einheit zuordnen";
        }
    }
}