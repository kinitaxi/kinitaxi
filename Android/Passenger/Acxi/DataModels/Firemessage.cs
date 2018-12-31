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
   public class Firemessage
    {
        public Data data { get; set; }
        public string to { get; set; }


        public class Data
        {
            public string type { get; set; }
            public string ride { get; set; }
        }

    }


}