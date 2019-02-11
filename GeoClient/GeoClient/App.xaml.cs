using GeoClient.Services.Boundary;
using GeoClient.Services.Location;
using GeoClient.Services.Registration;
using GeoClient.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

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

            AppCenter.Start("android=95fff8c4-3837-40ac-b0eb-73768b9b08d0;" +
                  "uwp=e0a7552f-c002-434e-9354-a5877373a6c6;" +
                  "ios=baf87dca-150c-4048-bbeb-829d82272a35;",
                  typeof(Analytics), typeof(Crashes));
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