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
    public class DriverLocationEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<DriverLocationEventArgs> OnDriverLocationFound;
        public class DriverLocationEventArgs : EventArgs
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
           
        }

        public void OnDataChange(DataSnapshot snapshot)
        {

            if (snapshot != null)
            {
                double lat = 0;
                double lng = 0;
                //WATCH STATUS FOR RIDE CANCELLED - NULL;
                lat = double.Parse(snapshot.Child("latitude").Value.ToString());
                lng = double.Parse(snapshot.Child("longitude").Value.ToString());

                OnDriverLocationFound.Invoke(this, new DriverLocationEventArgs { lat = lat, lng = lng });
            }
        }
    }
}