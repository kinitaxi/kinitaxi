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

namespace AcxiDriver.EventListeners
{
    public class RideValidityValueEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<RideValidityEvenrArgs> IsRideValid;
        public class RideValidityEvenrArgs : EventArgs
        {
            public bool validity { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                if(snapshot.Child("rider_id").Value != null)
                {
                    IsRideValid.Invoke(this, new RideValidityEvenrArgs { validity = true });
                }
                else
                {
                    IsRideValid.Invoke(this, new RideValidityEvenrArgs { validity = false });
                }

            }
        }
    }
}