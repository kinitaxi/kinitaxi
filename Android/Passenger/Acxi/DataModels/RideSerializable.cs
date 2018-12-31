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

namespace Acxi.DataModels
{

    public class RideSerializable
    {
        public string latloc { get; set; }
        public string lngloc { get; set; }
        public string latdes { get; set; }
        public string lngdes { get; set; }       
        public string pickuplocation_address { get; set; }
        public string destination1_address { get; set; }
        public double distancevalue { get; set; }
        public string distance { get; set; }
        public string duration { get; set; }
        public double durationvalue { get; set; }
        public string payment_method { get; set; }
        public double timestamp { get; set; }
        public string ride_id { get; set; }
        public double estimatefare { get; set; }
        public string driver_id { get; set; }
    }
}