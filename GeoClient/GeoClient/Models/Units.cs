using System;
using System.Collections.Generic;
using GeoClient.Services.Utils;
using Xamarin.Forms;
using System.Linq;
using GeoClient.Services.Registration;

namespace GeoClient.Models
{
    public class Unit
    {
        public string Id { get; }
        public string Name { get; set; }
        public GeoPoint LastPoint { get; set; }
        public string State { get; set; }
        public Unit(string id)
        {
            Id = id;
        }        
    }
}