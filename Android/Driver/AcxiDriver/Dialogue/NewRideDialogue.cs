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
using Android.Media;

namespace AcxiDriver.Dialogue
{
    public class NewRideDialogue : Android.Support.V4.App.DialogFragment
    {
        public TextView txtdestination;
        public TextView txtlocation;

        TextView txtaccept;
        TextView txtreject;
        string mlocation = "";
        string mdestination = "";
       // MediaPlayer player;

        public event EventHandler OrderRejected;
        public event EventHandler OrderAccepted;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //player = MediaPlayer.Create(Application.Context, Resource.Raw.alert);
            //player.Looping = true;
            //player.Start();
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.newrequest_dialogue, container, false);
            txtlocation = (TextView)view.FindViewById(Resource.Id.txtnewride_location);
            txtdestination = (TextView)view.FindViewById(Resource.Id.txtnewride_destination);
            txtaccept = (TextView)view.FindViewById(Resource.Id.txtaccept_ride);
            txtreject = (TextView)view.FindViewById(Resource.Id.txtreject_ride);
            txtlocation.Text = mlocation;
            txtdestination.Text = mdestination;
            txtaccept.Click += Txtaccept_Click;
            txtreject.Click += Txtreject_Click;
            return view;
        }

        private void Txtreject_Click(object sender, EventArgs e)
        {
            //player.Looping = false;
            //player.Stop();
            //player.Looping = false;
            OrderRejected.Invoke(this, new EventArgs());
            this.Dismiss();
        }

        private void Txtaccept_Click(object sender, EventArgs e)
        {
            //player.Looping = false;
            //player.Stop();
            //player.Looping = false;

            OrderAccepted.Invoke(this, new EventArgs());
            this.Dismiss();
        }

        public NewRideDialogue(string location, string destination)
        {
            mlocation = location;
            mdestination = destination;
        }
    }
}