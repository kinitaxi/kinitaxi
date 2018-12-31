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

namespace AcxiDriver.Dialogue
{
    public class CollectPaymentDialogue : Android.Support.V4.App.DialogFragment
    {
        TextView txttotal;
        TextView txttime;
        TextView txtdistance;
        TextView txtbase;
        TextView txtstops;
        Button btncollectpayment;

        string total;
        string time;
        string basefare;
        string distance;
        string stops;

        public event EventHandler CollectPaymentClicked;
        HelperFunctions helpers = new HelperFunctions();

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
            txtbase = (TextView)view.FindViewById(Resource.Id.txtbasefare);
            txtstops = (TextView)view.FindViewById(Resource.Id.txtstopfare);

            btncollectpayment = (Button)view.FindViewById(Resource.Id.btnpay);
            btncollectpayment.Click += Btncollectpayment_Click;
            txttotal.Text = helpers.CurrencyConvert(double.Parse(total));
            txttime.Text = helpers.CurrencyConvert(double.Parse(time));
            txtbase.Text = helpers.CurrencyConvert(double.Parse(basefare));
            txtdistance.Text = helpers.CurrencyConvert(double.Parse(distance));
            txtstops.Text = helpers.CurrencyConvert(double.Parse(stops));
            return view;
        }

        private void Btncollectpayment_Click(object sender, EventArgs e)
        {
            CollectPaymentClicked.Invoke(this, new EventArgs());
        }

        public CollectPaymentDialogue(string mtotal, string mbase, string mdistance, string mtime, string mstops)
        {
            total = mtotal;
            basefare = mbase;
            distance = mdistance;
            time = mtime;
            stops = mstops;
        }
    }
}