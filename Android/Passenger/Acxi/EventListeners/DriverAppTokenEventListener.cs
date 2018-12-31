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
    public class DriverAppTokenEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<AppTokenEventArgs> OnTokenFetched;
        public class AppTokenEventArgs : EventArgs
        {
            public string token { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
           // throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           if(snapshot.Value != null)
            {
                string mtoken = snapshot.Value.ToString();
                OnTokenFetched.Invoke(this, new AppTokenEventArgs { token = mtoken });
            }
        }
    }
}