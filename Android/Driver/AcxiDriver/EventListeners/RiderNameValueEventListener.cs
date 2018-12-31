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
    public class RiderNameValueEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<RiderNameEventArgs> GetRiderName;
        public class RiderNameEventArgs : EventArgs
        {
            public string first_name { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           if(snapshot.Value != null)
            {
                GetRiderName.Invoke(this, new RiderNameEventArgs { first_name = snapshot.Value.ToString() });
            }
            else
            {
                GetRiderName.Invoke(this, new RiderNameEventArgs { first_name = "Not available" });
            }
        }
    }
}