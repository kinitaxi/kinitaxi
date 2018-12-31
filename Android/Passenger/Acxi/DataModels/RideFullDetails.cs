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
   public class RideFullDetails
    {
        public string rating { get; set; }
        public string total_fares { get; set; }
        public string pickup_address { get; set; }
        public string destination_address { get; set; }
        public string payment_method { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string driver_firstname { get; set; }
        public string driver_lastname { get; set; }

        public string driver_pic { get; set; }
        public string driver_carmodel { get; set; }
        public string driver_carcolor { get; set; }
        public string driver_caryear { get; set; }
        public string driver_carmake { get; set; }
        public string driver_platenumber { get; set; }
        public string feedback { get; set; }
    }

}