using System;
using System.Web;
using GeoClient.Models;

namespace GeoClient.Views.Utils
{
    public class GeoPointUtil
    {
        public static Uri CreateGeoUri(GeoPoint geoPoint, string geoPointerTag)
        {
            Uri geoUri = null;

            if (GeoPointUtil.IsGeoPointValid(geoPoint))
            {
                var request = string.Format("geo:{0},{1}?q={0},{1}({2})", geoPoint.Latitude, geoPoint.Longitude, HttpUtility.UrlEncode(geoPointerTag));
                geoUri = new Uri(request);
            }
            else
            {
                Console.WriteLine("Cannot create geo URI for geo point: " + geoPoint);
            }

            return geoUri;
        }

        private static bool IsGeoPointValid(GeoPoint geoPoint)
        {
            return geoPoint != null && NotBlank(geoPoint.Latitude) && NotBlank(geoPoint.Longitude);
        }

        private static bool NotBlank(string inputString)
        {
            return inputString != null && inputString.Trim() != "";
        }
    }
}
