
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatusSendingModalPage : ContentPage
    {
        public StatusSendingModalPage()
        {
            InitializeComponent();
        }

        protected override void OnDisappearing()
        {
            // On disappearing of the page pop also the incident detail view from stack to trigger reload of the incident list.
            Navigation.PopToRootAsync();
        }
    }
}