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
using AcxiDriver.Activities;

namespace AcxiDriver.EventListeners
{
    public class RiderFoundValueListener : Java.Lang.Object, IValueEventListener
    {
        AppMainActivity main;
        public RiderFoundValueListener(AppMainActivity mm)
        {
            main = mm;
        }
        public void OnCancelled(DatabaseError error)
        {
            //
        }

        public void OnDataChange(DataSnapshot snapshot)
        {

            if (snapshot != null)
            {
                main.riderDetails.rider_name = snapshot.Child("first_name").Value.ToString();
                main.ShowRide();
            }
        }
    }

}