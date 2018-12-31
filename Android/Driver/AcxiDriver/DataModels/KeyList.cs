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
    public class KeyList : Java.Lang.Object
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}