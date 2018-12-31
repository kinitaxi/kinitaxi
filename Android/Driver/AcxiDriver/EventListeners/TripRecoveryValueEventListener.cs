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
    public class TripRecoveryValueEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<OnTripEventArgs> IsTripOngoing;
        public class OnTripEventArgs : EventArgs
        {
            public string ongoing { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
           // throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                string ongoing = snapshot.Value.ToString();
                IsTripOngoing.Invoke(this, new OnTripEventArgs { ongoing = ongoing });
            }
            else
            {
                IsTripOngoing.Invoke(this, new OnTripEventArgs { ongoing = "" });
            }
        }
    }
}