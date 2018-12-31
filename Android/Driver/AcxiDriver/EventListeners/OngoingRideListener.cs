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
using AcxiDriver.Dialogue;
using Firebase.Database;
using AcxiDriver.Activities;

namespace AcxiDriver.EventListeners
{
    public class OngoingRideListener : Java.Lang.Object, IValueEventListener
    {
        EnrouteActivity main;
        public OngoingRideListener(EnrouteActivity mm)
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
                if (snapshot.Child("payment_method").Value != null)
                {
                    main.Payment_method = snapshot.Child("payment_method").Value.ToString();
                }


                if (snapshot.Child("stops").Value != null)
                {
                    string online_stop = snapshot.Child("stops").Value.ToString();
                    if (main.stops.ToString() != online_stop)
                    {
                        main.stops = double.Parse(online_stop);
                        if (main.myalert != null)
                        {
                            main.myalert.Dismiss();
                        }
                        main.myalert = null;
                        main.myalert = new AppAlertDialogue("Rider made a stop, please endevour to accord the necessary patience");
                       
                        var trans = main.SupportFragmentManager.BeginTransaction();
                        main.myalert.Show(trans, "alert");
                        main.myalert.AlertCancel += Myalert_AlertCancel;
                        main.myalert.AlertOk += Myalert_AlertOk;

                    }
                }

                if(snapshot.Child("status").Value != null)
                {
                    if(snapshot.Child("status").Value.ToString() == "cancelled_p")
                    {
                        main.RideCancelled();
                    }
                }
            }
           
        }

        private void Myalert_AlertOk(object sender, EventArgs e)
        {
            main.myalert.Dismiss();
        }

        private void Myalert_AlertCancel(object sender, EventArgs e)
        {
            main.myalert.Dismiss();
        }
    }

}