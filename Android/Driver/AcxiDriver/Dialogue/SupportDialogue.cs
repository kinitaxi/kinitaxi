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
    public class SupportDialogue : Android.Support.V4.App.DialogFragment
    {
        LinearLayout btnnewticket;
        LinearLayout btnviewticket;
        public event EventHandler CreateTicket;
        public event EventHandler ViewTickets;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.support_dialogue, container, false);
            btnnewticket = (LinearLayout)view.FindViewById(Resource.Id.lay_newticket);
            btnviewticket = (LinearLayout)view.FindViewById(Resource.Id.lay_viewtickets);

            btnnewticket.Click += Btnnewticket_Click;
            btnviewticket.Click += Btnviewticket_Click;
            return view;
        }

        private void Btnviewticket_Click(object sender, EventArgs e)
        {
            ViewTickets.Invoke(this, new EventArgs());
        }

        private void Btnnewticket_Click(object sender, EventArgs e)
        {
            CreateTicket.Invoke(this, new EventArgs());
        }
    }
}