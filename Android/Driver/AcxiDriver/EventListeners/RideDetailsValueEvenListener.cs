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
    public class RideDetailsValueEvenListener : Java.Lang.Object, IValueEventListener
    {
        public class TripStatusEventArgs : EventArgs
        {
            public string status { get; set; }
        }
        public event EventHandler<TripStatusEventArgs> isTripStatus;

        public void OnCancelled(DatabaseError error)
        {
           // throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           if(snapshot.Value != null)
            {
                if(snapshot.Child("status").Value != null)
                {
                    string status = snapshot.Child("status").Value.ToString();
                    isTripStatus.Invoke(this, new TripStatusEventArgs { status = status });
                }
                else
                {
                    isTripStatus.Invoke(this, new TripStatusEventArgs { status = "" });

                }
            }
            else
            {
                isTripStatus.Invoke(this, new TripStatusEventArgs { status = "" });
            }
        }
    }
}