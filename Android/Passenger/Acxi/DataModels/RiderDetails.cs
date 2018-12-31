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
    public class RiderDetails
    {
        public string pickup_address { get; set; }
        public string destination_address { get; set; }
        public LatLng latlng_pickup { get; set; }
        public LatLng latlng_destination { get; set; }
        public string rider_name { get; set; }
        public string rider_phone { get; set; }
        public double latloc { get; set; }
        public double lngloc { get; set; }
        public double latdes { get; set; }
        public double lngdes { get; set; }
        public string ride_id { get; set; }
    }
}