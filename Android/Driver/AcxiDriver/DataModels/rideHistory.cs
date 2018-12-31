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

namespace AcxiDriver.DataModels
{
   public class rideHistory
    {
        public string pickup_address { get; set; }
        public string destination_address { get; set; }
        public string ride_id { get; set; }
        public string status { get; set; }
        public string total_fare { get; set; }
        public string created_at { get; set; }

    }
}