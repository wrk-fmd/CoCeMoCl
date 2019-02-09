using GeoClient.Services.Boundary;
using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using GeoClient.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace GeoClient
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();

            LocationChangeRegistry.Instance.RegisterListener(RestService.Instance);
        }

        protected override void OnStart()
        {
            Console.WriteLine("Starting GeoClient application.");
            RegistrationService.Instance.LoadRegistrationInfo();
        }

        protected override void OnSleep()
        {
            Console.WriteLine("GeoClient is going to sleep...");
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            Console.WriteLine("Resuming GeoClient.");
            // Handle when your app resumes
        }
    }
}