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
using AcxiDriver.DataModels;
using Firebase.Database;
using AcxiDriver.Activities;
using Android.Gms.Maps.Model;

namespace AcxiDriver.EventListeners
{
    public class RideRequestValueListener : Java.Lang.Object, IValueEventListener
    {
     
        AppMainActivity main;
        public RideRequestValueListener(AppMainActivity mm)
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
                try
                {
                    main.riderDetails = new RiderDetails();
                    double pickup_lng = 0;
                    double pickup_lat = 0;

                    double destination_lng = 0;
                    double destination_lat = 0;

                    pickup_lng = double.Parse(snapshot.Child("location").Child("longitude").Value.ToString());
                    pickup_lat = double.Parse(snapshot.Child("location").Child("latitude").Value.ToString());

                    destination_lng = double.Parse(snapshot.Child("destination").Child("longitude").Value.ToString());
                    destination_lat = double.Parse(snapshot.Child("destination").Child("latitude").Value.ToString());

                    main.riderDetails.latloc = pickup_lat;
                    main.riderDetails.lngloc = pickup_lng;
                    main.riderDetails.latdes = destination_lat;
                    main.riderDetails.lngdes = destination_lng;

                    main.riderDetails.latlng_pickup = new LatLng(pickup_lat, pickup_lng);
                    main.riderDetails.latlng_destination = new LatLng(destination_lat, destination_lng);
                    main.riderDetails.pickup_address = snapshot.Child("pickup_address").Value.ToString();
                    main.riderDetails.destination_address = snapshot.Child("destination_address").Value.ToString();
                    main.riderDetails.rider_phone = snapshot.Child("rider_id").Value.ToString();
                    main.DriverMatched = true;
                    main.FirebaseGetRiderInfo();
                }
                catch
                {

                }
              
            }
        }
    }


}