using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GeoClient.Views;
using Xamarin.Essentials;
using GeoClient.Services;
using System.Threading.Tasks;

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

            Device.StartTimer(TimeSpan.FromSeconds(10), () => {
                Task.Factory.StartNew(() =>
                {
                    LocationService.Instance.GetLocationAsync();
                });

                return shallPollLocation();
            });
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
        protected bool shallPollLocation()
        {  
            return true;
        }
    }
}
