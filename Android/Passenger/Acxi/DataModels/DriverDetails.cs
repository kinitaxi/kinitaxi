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
   public class DriverDetails
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string rating { get; set; }
        public string completed_rides { get; set; }
        public string acxi_age { get; set; }
        public LatLng location { get; set; }
        public string car_color { get; set; }
        public string car_model { get; set; }
        public string car_year { get; set; }
        public string car_make { get; set; }
        public string plate_number { get; set; }
        public string phone_number { get; set; }
        public string photourl { get; set; }
        public string city { get; set; }
        public string token { get; set; }
    }
}