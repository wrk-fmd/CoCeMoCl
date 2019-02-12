using System;
using System.Collections.Generic;
using System.Text;

namespace GeoClient.Models
{
    public class GeoPoint
    {
        public long Latitude { get; }
        public long Longitude { get; }

        public GeoPoint(long latitude, long longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
