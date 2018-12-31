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
    public class RideAssignedValueListener : Java.Lang.Object, IValueEventListener
    {
        AppMainActivity main;
      
        public RideAssignedValueListener(AppMainActivity mm)
        {
            main = mm;
        }
        
        public void OnCancelled(DatabaseError error)
        {
           // throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                if (snapshot.Child("ride_id").Value != null)
                {
                    string ride_id = snapshot.Child("ride_id").Value.ToString();
                    if (ride_id != "waiting" && ride_id != "timeout" && ride_id != "cancelled")
                    {
                        if (!main.DriverMatched)
                        {
                            main.DriverMatched = true;
                            main.FirebaseRideMatched(ride_id);
                            main.rideID = ride_id;
                        }
                    }
                    else if(ride_id == "timeout")
                    {
                        main.NewRideTimeout("timeout");
                    }
                    else if(ride_id == "cancelled")
                    {
                        main.NewRideTimeout("cancelled");

                    }
                }

            }
        }
    }
}