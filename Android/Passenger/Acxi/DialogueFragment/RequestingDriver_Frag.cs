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
    public class RequestingDriver_Frag : Android.Support.V4.App.DialogFragment
    {
        private Button btncancel;
        private TextView txtfares;
        double fare;
        public event EventHandler OnRideRequestCancelled;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.requestingAcxi_frag, container, false);
            btncancel = (Button) view.FindViewById(Resource.Id.btnCancel_requestingfrag);
            txtfares = (TextView)view.FindViewById(Resource.Id.txtamount_requestingfrag);
            btncancel.Click += Btncancel_Click;

            Helpers.HelperFunctions helper = new Helpers.HelperFunctions();
            txtfares.Text = helper.CurrencyConvert(fare);
            return view;
        }
        public RequestingDriver_Frag(double fares)
        {
            fare = fares;
        }
        private void Btncancel_Click(object sender, EventArgs e)
        {
            OnRideRequestCancelled.Invoke(this, new EventArgs());
        }
    }
}