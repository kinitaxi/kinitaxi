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
    class HistoryItem
    {
        public string location { get; set; }
        public string destination { get; set; }
        public double timestamp { get; set; }
        public string status { get; set; }
        public double amount { get; set; }
        public string payment_method { get; set; }
        public double ride_rating { get; set; }
        public string driver { get; set; }
        public string car_color { get; set; }
        public string car_model { get; set; }
        public string plate_number { get; set; }
        public string driver_rating { get; set; }
        public string feedback { get; set; }
    }
}