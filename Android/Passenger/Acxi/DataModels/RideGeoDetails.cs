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
using Android.Gms.Maps.Model;

namespace Acxi.DataModels
{
   public class RideGeoDetails
    {
       public LatLng latlng_pickuplocation { get; set; }
        public LatLng latlng_destination1 { get; set; }
        public LatLng latlng_destination2 { get; set; }
        public LatLng latlng_destination3 { get; set; }
        public string pickuplocation_address { get; set; }
        public string destination1_address { get; set; }
        public string destination2_address { get; set; }
        public string destination3_address { get; set; }
        public double distancevalue { get; set; }
        public string distance { get; set; }
        public string duration { get; set; }
        public double durationvalue { get; set; }
        public string payment_method { get; set; }
        public LatLngBounds ridebounds { get; set; }
        public double timestamp { get; set; }
        public string ride_id { get; set; }
        public double estimatefare { get; set; }
    }
}