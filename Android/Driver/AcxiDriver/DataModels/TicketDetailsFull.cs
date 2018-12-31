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
    public class TicketDetailsFull
    {
        public string category { get; set; }
        public double created_at { get; set; }
        public string title { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
}