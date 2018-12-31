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
using Acxi.DataModels;
using Acxi.Helpers;

namespace Acxi.DialogueFragment
{
    public class RideDetails_Frag : Android.Support.V4.App.DialogFragment
    {
        RideGeoDetails ride_details;
        TextView txtlocation;
        TextView txtdestination;
        TextView txtETA;
        TextView txtdistance;
        TextView txtpayment;
        TextView txtestimatefare;
        Button btnclose;
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

            View view = inflater.Inflate(Resource.Layout.ridedetails_dialogue, container, false);
            txtETA = (TextView)view.FindViewById(Resource.Id.txtarrival_ridedetails);
            txtdistance = (TextView)view.FindViewById(Resource.Id.txtdistance_ridedetails);
            txtdestination = (TextView)view.FindViewById(Resource.Id.txtdestination_ridedetails);
            txtlocation = (TextView)view.FindViewById(Resource.Id.txtlocation_ridedetails);
            txtpayment = (TextView)view.FindViewById(Resource.Id.txtpayment_ridedetails);
            txtdistance = (TextView)view.FindViewById(Resource.Id.txtdistance_ridedetails);
            txtestimatefare = (TextView)view.FindViewById(Resource.Id.txtestimated_ridedetails);
            btnclose = (Button)view.FindViewById(Resource.Id.btnclose_ridedetails);
            btnclose.Click += Btnclose_Click;
            txtETA.Text = ride_details.duration;
            txtdistance.Text = ride_details.distance;
            txtdestination.Text = ride_details.destination1_address;
            txtlocation.Text = ride_details.pickuplocation_address;
            txtpayment.Text = ride_details.payment_method;
            txtestimatefare.Text = helpers.CurrencyConvert(ride_details.estimatefare);
            return view;
        }

        private void Btnclose_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        public RideDetails_Frag(RideGeoDetails rideinfo)
        {
            ride_details = rideinfo;
        }
    }
}