using GeoClient.Services.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        private bool _isDebugPageAdded = false;

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
            var needToAddDebugPage = !_isDebugPageAdded && PrerequisitesChecking.IsDeveloperModeActive();

            if (needToAddDebugPage)
            {
                _isDebugPageAdded = true;
                TabbedPageInstance.Children.Add(new NavigationPage(new DebugPage())
                {
                    Title = "Debug"
                });
            }
        }
    }
}