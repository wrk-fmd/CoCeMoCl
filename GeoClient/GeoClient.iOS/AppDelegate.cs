using Foundation;
using GeoClient.Services.Registration;
using UIKit;

namespace GeoClient.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IGeoRegistrationListener
    {
        private readonly LocationManager _locationManager;

        private AppDelegate()
        {
            RegistrationService.Instance.RegisterListener(this);
            _locationManager = new LocationManager();
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            LoadApplication(new App());


            return base.FinishedLaunching(app, options);
        }

        public void GeoServerRegistered()
        {
            _locationManager.StartLocationUpdates();
        }

        public void GeoServerUnregistered()
        {
            _locationManager.StopLocationUpdates();
        }
    }
}
