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
    public class RequestValidityValueEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<RequestValidityEvenrArgs> IsRequestValid;
        public class RequestValidityEvenrArgs : EventArgs
        {
            public bool validity { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
           
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           if(snapshot.Value != null)
            {
                IsRequestValid.Invoke(this, new RequestValidityEvenrArgs { validity = true });
            }
            else
            {
                IsRequestValid.Invoke(this, new RequestValidityEvenrArgs { validity = false });
            }
        }
    }
}