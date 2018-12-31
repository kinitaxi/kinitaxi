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
using Acxi.Helpers;
using Acxi.DataModels;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Acxi.EventListeners
{

    public class DriverFoundValueListener : Java.Lang.Object, IValueEventListener
    {
        string mtype;
        HelperFunctions helper = new HelperFunctions();
        WebRequestHelpers webhelpers = new WebRequestHelpers();

        public event EventHandler<DriverOnTripEventArgs> isDriverOnTrip;
        public event EventHandler<driverDetailsEventArgs> driverDetails_found;
       // public event EventArgs;
        public class DriverOnTripEventArgs : EventArgs
        {
            public string ongoing { get; set; }
        }

        public class driverDetailsEventArgs : EventArgs
        {
            public DriverDetails myDriver { get; set; }
        }
        
      
        public DriverFoundValueListener( string type)
        {
            mtype = type;
        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {

            DriverDetails driverOb = new DriverDetails();
            if (snapshot.Value != null)
            {
                driverOb.photourl = snapshot.Child("documents").Child("profile_pic").Child("url").Value.ToString();

                //NEW THREAD DOWNLOADS DRIVER IMAGE
              
                driverOb.firstname = snapshot.Child("personal_details").Child("first_name").Value.ToString();
                driverOb.lastname = snapshot.Child("personal_details").Child("last_name").Value.ToString();
                string regtime = snapshot.Child("personal_details").Child("created_at").Value.ToString();
                driverOb.acxi_age = helper.DateDifference(double.Parse(regtime));
                driverOb.city = snapshot.Child("personal_details").Child("city").Value.ToString();

                driverOb.phone_number = snapshot.Child("personal_details").Child("phone").Value.ToString();
                driverOb.car_color = snapshot.Child("vehicle_details").Child("color").Value.ToString();
                driverOb.car_model = snapshot.Child("vehicle_details").Child("model").Value.ToString();
                driverOb.car_make = snapshot.Child("vehicle_details").Child("make").Value.ToString();
                driverOb.car_year = snapshot.Child("vehicle_details").Child("year").Value.ToString();
                driverOb.plate_number = snapshot.Child("vehicle_details").Child("plate_number").Value.ToString();
                driverOb.photourl = snapshot.Child("documents").Child("profile_pic").Child("url").Value.ToString();
                
                if (snapshot.Child("driver_rating").Value != null)
                {
                    driverOb.rating = snapshot.Child("driver_rating").Value.ToString();
                }

                if (snapshot.Child("trip_count").Value != null)
                {
                    driverOb.completed_rides = snapshot.Child("trip_count").Value.ToString();
                }

                if(snapshot.Child("token").Value != null)
                {
                    driverOb.token = snapshot.Child("token").Value.ToString();
                }


                if (mtype == "")
                {
                    driverDetails_found.Invoke(this, new driverDetailsEventArgs { myDriver = driverOb });
                }
                else
                {
                    if (snapshot.Child("ongoing").Value != null)
                    {
                        var jongoing = JObject.Parse(snapshot.Child("ongoing").Value.ToString());
                        string rideid = jongoing["ride_id"].ToString();
                        if (!string.IsNullOrEmpty(rideid))
                        {
                            isDriverOnTrip.Invoke(this, new DriverOnTripEventArgs { ongoing = rideid });
                        }
                    }
                    else
                    {
                        isDriverOnTrip.Invoke(this, new DriverOnTripEventArgs { ongoing = "" });
                    }
                }


                // main.foundDriverDetails = driverOb;
                //if (main.createRequestRef != null)
                //{
                //  main.createRequestRef.RemoveValueAsync();
                //}

                //if(mtype != "ongoing")
                //{

                //    main.ShowFoundDriver(driverOb);
                //    main.FirebaseOngoingRide();

                //}
                //if (mtype == "ongoing")
                //{
                //    if(snapshot.Child("ongoing").Value != null)
                //    {
                //        var jongoing = JObject.Parse(snapshot.Child("ongoing").Value.ToString());
                //        string rideid = jongoing["ride_id"].ToString();
                //        if (!string.IsNullOrEmpty(rideid))
                //        {
                //            isDriverOnTrip.Invoke(this, new DriverOnTripEventArgs { ongoing = rideid });
                //        }
                //    }
                //    else
                //    {
                //        isDriverOnTrip.Invoke(this, new DriverOnTripEventArgs { ongoing = "" });
                //    }
                //}


            }
        }
    }
}