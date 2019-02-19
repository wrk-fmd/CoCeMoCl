﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GeoClient.Models
{
    public class GeoPoint
    {
        public string Latitude { get; }
        public string Longitude { get; }

        public GeoPoint(string latitude, string longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
