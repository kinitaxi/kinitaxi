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
using Firebase.Database;

namespace Acxi.EventListeners
{
    public class CreateRequestValueListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler rideRequest_unfound;
        public event EventHandler<onDriverFoundEventArgs> rideRequest_assigned;
        public event EventHandler rideRequest_rejected;
       
        public class onDriverFoundEventArgs : EventArgs
        {
            public  string driver_id { get; set; }
        }

        public void OnCancelled(DatabaseError error)
        {
           
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            string driverassigned = "";
            if (snapshot.Value != null)
            {
                if (snapshot.Child("driver_id").Value != null)
                {
                    driverassigned = snapshot.Child("driver_id").Value.ToString();
                }
            }

            if (driverassigned != "waiting" && driverassigned != "order_rejected" && driverassigned != "")
            {

                if (snapshot.Child("driver_id").Value != null)
                {
                    string driver_id = snapshot.Child("driver_id").Value.ToString();
                    rideRequest_assigned.Invoke(this, new onDriverFoundEventArgs { driver_id = driver_id });
                }
            }
            // FINDA A NEW DRIVER IF ORDER WAS REJECTED
            else if (driverassigned == "order_rejected")
            {
                rideRequest_rejected.Invoke(this, new EventArgs());
            }
            else
            {
                rideRequest_unfound.Invoke(this, new EventArgs());
            }

        }
    }
}