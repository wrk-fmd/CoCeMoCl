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
        private LocationService locationService;
        private Location _location; 
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
            locationService = new LocationService();
            Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                Task.Factory.StartNew(async () =>
                {
                    var location = await locationService.getLocationAsync();
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
