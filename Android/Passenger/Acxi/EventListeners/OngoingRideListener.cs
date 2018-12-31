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
    public class OngoingRideListener : Java.Lang.Object, IValueEventListener
    {

        public event EventHandler<rideStatusEventArgs> ongoingRide_status;
        public event EventHandler<rideFaresEventArgs> ongoingRide_ended_fares;
        public class rideStatusEventArgs : EventArgs
        {
            public string status { get; set; }
            public double lat { get; set; }
            public double lng { get; set; } 
        }

        public class rideFaresEventArgs : EventArgs
        {
           public string total_fare { get; set; }
           public string  basefare { get; set; }
           public  string time_fare { get; set; }
           public string distance_fare { get; set; }
           public string stop_fare { get; set; }

         public string status { get; set; }
        }
      
        public void OnCancelled(DatabaseError error)
        {
            //
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                //  WATCH STATUS FOR RIDE STARTED

                // WATCH STATUS FOR RIDE COMPLETION;

                //WATCH STATUS FOR RIDE CANCELLED - NULL;
                //  driverOb.firstname = snapshot.Child("first_name").Value.ToString();

                string basefare = "";
                if (snapshot.Child("fares").Child("base_fare").Value != null)
                {
                    basefare = snapshot.Child("fares").Child("base_fare").Value.ToString();
                }


                string time_fare = "";
                if (snapshot.Child("fares").Child("time_fare").Value != null)
                {
                    time_fare = snapshot.Child("fares").Child("time_fare").Value.ToString();
                }

                string distance_fare = "";
                if (snapshot.Child("fares").Child("distance_fare").Value != null)
                {
                    distance_fare = snapshot.Child("fares").Child("distance_fare").Value.ToString();
                }

                string stop_fare = "";
                if (snapshot.Child("fares").Child("total_fare").Value != null)
                {
                    stop_fare = snapshot.Child("fares").Child("stop_fare").Value.ToString();
                }

                string total_fare = "";
                if (snapshot.Child("fares").Child("total_fare").Value != null)
                {
                    total_fare = snapshot.Child("fares").Child("total_fare").Value.ToString();
                }



                string driverlat = "0", driverlng = "0";
   
                if (snapshot.Child("status").Value != null)
                {
                    string status = snapshot.Child("status").Value.ToString();
                    if (snapshot.Child("driverLocation").Value != null)
                    {
                        driverlat = snapshot.Child("driverLocation").Child("latitude").Value.ToString();
                        driverlng = snapshot.Child("driverLocation").Child("longitude").Value.ToString();
                        if (string.IsNullOrEmpty(driverlat))
                        {
                            driverlat = "0";
                        }

                        if (string.IsNullOrEmpty(driverlng))
                        {
                            driverlng = "0";
                        }
                    }
                    ongoingRide_status.Invoke(this, new rideStatusEventArgs { lat = double.Parse(driverlat), lng = double.Parse(driverlng), status = status });

                    if (status == "ended")
                    {
                        if ((!string.IsNullOrEmpty(basefare)) && (!string.IsNullOrEmpty(total_fare)) && (!string.IsNullOrEmpty(time_fare)) && (!string.IsNullOrEmpty(distance_fare)) && (!string.IsNullOrEmpty(stop_fare)))
                        {
                            //if (main.orderstate == "ONTRIP")
                            //{
                            //    main.TripEnded(total_fare, basefare, time_fare, distance_fare, stop_fare);

                            //}

                            ongoingRide_ended_fares.Invoke(this, new rideFaresEventArgs { status = status, basefare = basefare, distance_fare = distance_fare, stop_fare = stop_fare, time_fare = time_fare, total_fare = total_fare });
                        }
                    }
                }

            

               
            }
        }
    }

}