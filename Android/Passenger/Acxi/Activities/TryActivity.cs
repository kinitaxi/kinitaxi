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
using Android.Support.V7.App;

namespace Acxi.Activities
{
    [Activity(Label = "TryActivity", MainLauncher = false, Theme ="@style/AcxiTheme1")]
    public class TryActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.driverassigned_frag);
        }
    }
}