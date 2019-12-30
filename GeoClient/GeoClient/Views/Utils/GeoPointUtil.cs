using GeoClient.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Xamarin.Forms;

namespace GeoClient.Views.Utils
{
    public class GeoPointUtil
    {
        public static bool NotBlank(string inputString)
        {
            return inputString != null && inputString.Trim() != "";
        }

        public static List<Uri> CreateUrisForGeoPoint(GeoPoint geoPoint, string geoPointerTag)
        {
            List<Uri> uriList = null;

            if (GeoPointUtil.IsGeoPointValid(geoPoint))
            {
                uriList = new List<Uri>();
                string latitudeString = geoPoint.Latitude.ToString(CultureInfo.InvariantCulture);
                string longitudeString = geoPoint.Longitude.ToString(CultureInfo.InvariantCulture);
                
                uriList.Add(CreateGeoUri(latitudeString, longitudeString, geoPointerTag));
                if (IsRunningOnIOS())
                {
                    uriList.Add(CreateAppleMapsUri(latitudeString, longitudeString, geoPointerTag));
                    uriList.Add(CreateGoogleMapsUri(latitudeString, longitudeString));
                } else
                {
                    uriList.Add(CreateGoogleMapsUri(latitudeString, longitudeString));
                    uriList.Add(CreateAppleMapsUri(latitudeString, longitudeString, geoPointerTag));
                }
            }
            else
            {
                Console.WriteLine("Cannot create geo URI for geo point: " + geoPoint);
            }

            return uriList;
        }

        private static Uri CreateGeoUri(string latitudeString, string longitudeString, string geoPointerTag)
        {
            var uriString = string.Format(
                      "geo:{0},{1}?q={0},{1}({2})",
                      latitudeString,
                      longitudeString,
                      HttpUtility.UrlEncode(geoPointerTag));
            return new Uri(uriString);
        }

        private static Uri CreateGoogleMapsUri(string latitudeString, string longitudeString)
        {
            var uriString = string.Format(
                      "https://www.google.com/maps/search/?api=1&query={0},{1}",
                      latitudeString,
                      longitudeString);
            return new Uri(uriString);
        }

        private static Uri CreateAppleMapsUri(string latitudeString, string longitudeString, string geoPointerTag)
        {
            var uriString = string.Format(
                      "http://maps.apple.com/?ll={0},{1}&q={2}",
                      latitudeString,
                      longitudeString,
                      HttpUtility.UrlEncode(geoPointerTag));
            return new Uri(uriString);
        }

        private static bool IsRunningOnIOS()
        {
            return Device.RuntimePlatform == Device.iOS;
        }

        private static bool IsGeoPointValid(GeoPoint geoPoint)
        {
            return geoPoint != null;
        }
    }
}
