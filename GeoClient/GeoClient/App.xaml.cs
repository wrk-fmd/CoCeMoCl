using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GeoClient.Views;
using Xamarin.Essentials;
using GeoClient.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace GeoClient
{

    public partial class App : Application
    {
        private LocationService locationService; 
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            locationService = new LocationService();
            Device.StartTimer(TimeSpan.FromSeconds(30), getLocation); //replace x with required seconds


        }

        protected override async void OnStart()
        {
            // Handle when your app starts
        }

        protected override async void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override async void OnResume()
        {
            // Handle when your app resumes
        }
        async void getLocation()
        {
            Location location = await locationService.getLocationAsync();
            Console.WriteLine("LOCATION ON RESUME: " + location.Timestamp + " - " + location.Latitude + " / " + location.Longitude);
            return true;
        }
    }
}
