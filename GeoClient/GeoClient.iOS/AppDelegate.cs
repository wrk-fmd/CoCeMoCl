using System;

using Foundation;
using UIKit;

namespace GeoClient.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public static IOSLocationService Manager = null;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            LoadApplication(new App());

            Manager = new IOSLocationService();
            Manager.StartLocationUpdates();
            return base.FinishedLaunching(app, options);// as soon as the app is done launching, begin generating location updates in the location manager

        }

        public override void WillEnterForeground(UIApplication application)
        {
            Console.WriteLine("App will enter foreground");
        }

        // Runs when the activation transitions from running in the background to
        // being the foreground application.
        // Also gets hit on app startup
        public override void OnActivated(UIApplication application)
        {
            Console.WriteLine("App is becoming active");
        }

        public override void OnResignActivation(UIApplication application)
        {
            Console.WriteLine("App moving to inactive state.");
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Console.WriteLine("App entering background state.");
            Console.WriteLine("Now receiving location updates in the background");
        }
    }
}