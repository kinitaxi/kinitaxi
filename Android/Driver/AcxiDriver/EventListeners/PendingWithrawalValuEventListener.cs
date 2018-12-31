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
    public class PendingWithrawalValuEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<PendingEventArgs> PendingChanged;
        public class PendingEventArgs : EventArgs
        {
            public double amount { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                string amount = (snapshot.Child("amount").Value != null ? snapshot.Child("amount").Value.ToString() : "");
                PendingChanged.Invoke(this, new PendingEventArgs { amount = double.Parse(amount) });
            }
            else
            {
                PendingChanged.Invoke(this, new PendingEventArgs { amount = 0 });
            }
        }
    }

}