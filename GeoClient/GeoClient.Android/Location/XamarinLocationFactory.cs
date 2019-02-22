using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GeoClient.Droid.Location
{
    public static class XamarinLocationFactory
    {
        public static Xamarin.Essentials.Location CreateXamarinLocation(Android.Locations.Location location)
        {
            return new Xamarin.Essentials.Location
            {
                Accuracy = location.Accuracy,
                Altitude = location.Altitude,
                Course = location.Bearing,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Speed = location.Speed,
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(location.Time)
            };
        }
    }
}