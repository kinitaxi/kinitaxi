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
using Acxi.Helpers;
using Android.Support.Design.Widget;

namespace Acxi.DialogueFragment
{
    public class FaresDialogue : Android.Support.V4.App.DialogFragment
    {
        TextView txttotal;
        TextView txttime;
        TextView txtdistance;
        TextView txtbase;
        TextView txtstops;
        TextInputLayout txtOTP;

        Button btnpay;

        string total;
        string time;
        string basefare;
        string distance;
        string paytype;
        string stops;
        public event EventHandler PaymentClicked;
        HelperFunctions helper = new HelperFunctions();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.pay_dialogue, container, false);
            txttotal = (TextView)view.FindViewById(Resource.Id.txttotalfares);
            txttime = (TextView)view.FindViewById(Resource.Id.txttimefare);
            txtdistance = (TextView)view.FindViewById(Resource.Id.txtdistancefare);
            txtOTP = (TextInputLayout)view.FindViewById(Resource.Id.txtotp_fares);
            txtstops = (TextView)view.FindViewById(Resource.Id.txtstopfare);

            txtbase = (TextView)view.FindViewById(Resource.Id.txtbasefare);
            btnpay = (Button)view.FindViewById(Resource.Id.btnpay);
            btnpay.Click += Btnpay_Click;
            txttotal.Text = helper.CurrencyConvert(double.Parse(total));
            txttime.Text = helper.CurrencyConvert(double.Parse(time));
            txtbase.Text = helper.CurrencyConvert(double.Parse(basefare));
            txtdistance.Text = helper.CurrencyConvert(double.Parse(distance));
            txtstops.Text = helper.CurrencyConvert(double.Parse(stops));
            if (paytype == "cash")
            {
                txtOTP.Visibility = ViewStates.Gone;
                btnpay.Text = "Pay Cash";
            }
            
            return view;
        }

        public FaresDialogue(string mtotal, string mbase, string mdistance, string mtime, string mstops, string mtype)
        {
            total = mtotal;
            basefare = mbase;
            distance = mdistance;
            time = mtime;
            paytype = mtype;
            stops = mstops;
        }

        private void Btnpay_Click(object sender, EventArgs e)
        {
            PaymentClicked.Invoke(this, new EventArgs());
            this.Dismiss();
        }
    }
}