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
    public class RequestTimeoutValueEventListener : Java.Lang.Object, IValueEventListener
    {
        string ride_id;
        public event EventHandler<DriverAvailableValidityEventArgs> isDriverAvailabeValid;
       public class DriverAvailableValidityEventArgs : EventArgs
        {
            public bool valid { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
           
        }

        public RequestTimeoutValueEventListener(string id)
        {
            ride_id = id;
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           if(snapshot.Value != null)
            {
                string r_ide = snapshot.Value.ToString();

                if(ride_id == r_ide)
                {
                    isDriverAvailabeValid.Invoke(this, new DriverAvailableValidityEventArgs { valid = true });
                }
                else
                {
                    isDriverAvailabeValid.Invoke(this, new DriverAvailableValidityEventArgs { valid = false });
                }
            }
            else
            {
                isDriverAvailabeValid.Invoke(this, new DriverAvailableValidityEventArgs { valid = false });
            }
        }
    }
}