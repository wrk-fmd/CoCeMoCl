using GeoClient.Services;
using GeoClient.Views;
using System;
using System.Threading.Tasks;
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

            RestService restService = new RestService();
            LocationService.Instance.RegisterListener(restService);

            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                Task.Factory.StartNew(() => { LocationService.Instance.GetLocationAsync(); });

                return ShallPollLocation();
            });
        }

        protected override async void OnStart()
        {
            RegistrationService.Instance.LoadRegistrationInfo();
        }

        protected override async void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override async void OnResume()
        {
            // Handle when your app resumes
        }

        protected bool ShallPollLocation()
        {
            return true;
        }
    }
}