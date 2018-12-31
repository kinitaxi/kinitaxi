using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Acxi.DialogueFragment
{
    public class AppUpdate_Dialogue : Android.Support.V4.App.DialogFragment
    {
        Button btnupdate;
        public event EventHandler OnUpdateApp;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.appupdate, container, false);
            btnupdate = (Button)view.FindViewById(Resource.Id.btnupdateapp);
            btnupdate.Click += Btnupdate_Click;
            return view;
        }

        private void Btnupdate_Click(object sender, EventArgs e)
        {
            OnUpdateApp.Invoke(this, new EventArgs());
            this.Dismiss();
        }
    }
}