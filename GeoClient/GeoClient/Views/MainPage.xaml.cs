using GeoClient.Services.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            AddDebugPageIfNecessary();
        }

        private void AddDebugPageIfNecessary()
        {
            var isDeveloperModeActive = PrerequisitesChecking.IsDeveloperModeActive();

            if (isDeveloperModeActive)
            {
                TabbedPageInstance.Children.Add(new NavigationPage(new DebugPage())
                {
                    Title = "Debug"
                });
            }
        }
    }
}