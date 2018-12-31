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
    public class AppSettingsListener : Java.Lang.Object, IValueEventListener
    {
        public class OnBaseFoundEventArgs : EventArgs
        {
            public string basefare { get; set; }
            public string timefare { get; set; }
            public string distancefare { get; set; }
            public string stopfare { get; set; }
            public string timeout { get; set; }
            public string notification_key { get; set; }
            public string version { get; set; }
            public string force_update { get; set; }
            public string registration_bonus { get; set; }
          
        }
        public event EventHandler<OnBaseFoundEventArgs> BaseFareFound;

        public void OnCancelled(DatabaseError error)
        {
          
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                if (snapshot.Child("base_fare").Value != null && snapshot.Child("time_fare").Value != null && snapshot.Child("distance_fare").Value != null && snapshot.Child("stop_fare").Value != null)
                {

                    string facebook_page = "";
                    string twitter_page = "";

                    string appSetting_basefare = snapshot.Child("base_fare").Value.ToString();
                    string appsetting_timefare = snapshot.Child("time_fare").Value.ToString();
                    string appsetting_distance_fare = snapshot.Child("distance_fare").Value.ToString();
                    string appsetting_stop_fare = snapshot.Child("stop_fare").Value.ToString();
                    string appsetting_timeout = snapshot.Child("timeout").Value.ToString();
                    string appsettings_notif_key = snapshot.Child("notification_key").Value.ToString();
                    string appsettings_force_update = snapshot.Child("force_update").Value.ToString();
                    string appsettings_version = snapshot.Child("version").Value.ToString();
                    string appsettings_regbonus = snapshot.Child("registration_bonus").Value.ToString();

                    BaseFareFound.Invoke(this, new OnBaseFoundEventArgs { basefare = appSetting_basefare, timefare = appsetting_timefare, distancefare = appsetting_distance_fare, stopfare = appsetting_stop_fare, timeout = appsetting_timeout, notification_key = appsettings_notif_key, force_update = appsettings_force_update, version = appsettings_version, registration_bonus = appsettings_regbonus });
                }

            }
        }
    }
}