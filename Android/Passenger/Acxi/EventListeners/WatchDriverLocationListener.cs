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
using Android.Gms.Maps.Model;

namespace Acxi.EventListeners
{
    public class WatchDriverLocationListener : Java.Lang.Object, IValueEventListener
    {

        MainActivity main;
        public WatchDriverLocationListener(MainActivity mm)
        {
            main = mm;
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
                lat = double.Parse(snapshot.Child("location").Child("latitude").Value.ToString());
                lng = double.Parse(snapshot.Child("location").Child("longitude").Value.ToString());
                main.DriverLocation = new LatLng(lat, lng);

                //TODO-CALL MAP UPDATE ON STATUS - WAITING;

            }
        }
    }
}