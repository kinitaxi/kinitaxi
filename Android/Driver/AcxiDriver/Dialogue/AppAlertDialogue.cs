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

namespace AcxiDriver.Dialogue
{
    public class AppAlertDialogue : Android.Support.V4.App.DialogFragment
    {
        TextView txtok;
        TextView txtcancel;
        TextView txtbody;
        string body;
        public event EventHandler AlertOk;
        public event EventHandler AlertCancel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.alert_dialogue, container, false);
            txtbody = (TextView)view.FindViewById(Resource.Id.txtalertbody);
            txtcancel = (TextView)view.FindViewById(Resource.Id.txtalert_cancel);
            txtok = (TextView)view.FindViewById(Resource.Id.txtalert_ok);

            txtcancel.Click += Txtcancel_Click;
            txtok.Click += Txtok_Click;
            txtbody.Text = body;
            return view;
        }

        private void Txtok_Click(object sender, EventArgs e)
        {
            AlertOk.Invoke(this, new EventArgs());
        }

        private void Txtcancel_Click(object sender, EventArgs e)
        {
            AlertCancel.Invoke(this, new EventArgs());
           
        }

        public AppAlertDialogue(string mbody)
        {
            body = mbody;
        }
    }
}