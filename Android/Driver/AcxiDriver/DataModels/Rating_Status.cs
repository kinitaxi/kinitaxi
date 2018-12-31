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
    public class Rating_Status
    {
        public int rating { get; set; }
        public string status { get; set; }
        public string feedback { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string created_at { get; set; }
    }
}