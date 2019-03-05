using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace GeoClient.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "Einstellungen";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://intranet.wrk.at/confluence/display/KHD/Infoblatt+Geoclient")));
        }

        public ICommand OpenWebCommand { get; }

        public ICommand OpenMapViewCommand { get;  }
    }
}