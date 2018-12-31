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
   public class AssignDriver
    {
        public string ride_id { get; set; }
        public List<string> declined { get; set; }
    }
}