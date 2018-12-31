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
    public class TicketDetails
    {
        public string title { get; set; }
        public string category { get; set; }

        public double timestamp { get; set; }
        public string body { get; set; }
    }
}