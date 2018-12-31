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

namespace AcxiDriver.EventListeners
{
    public class AppSettingsListener : Java.Lang.Object, IValueEventListener
    {
        public class OnBaseFoundEventArgs : EventArgs
        {
            public string basefare { get; set; }
            public string timefare { get; set; }
            public string distancefare { get; set; }
            public string stopfare { get; set; }
            public string earning_percentage { get; set; }
        }
        public event EventHandler<OnBaseFoundEventArgs> BaseFareFound;

        public void OnCancelled(DatabaseError error)
        {
            //
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot != null)
            {
                string appSetting_basefare = snapshot.Child("base_fare").Value.ToString();
                string appsetting_timefare = snapshot.Child("time_fare").Value.ToString();
                string appsetting_distance_fare = snapshot.Child("distance_fare").Value.ToString();
                string appsetting_stop_fare = snapshot.Child("stop_fare").Value.ToString();
                string apperaning_percentage = snapshot.Child("drivers_percentage").Value.ToString();
                BaseFareFound.Invoke(this, new OnBaseFoundEventArgs { basefare = appSetting_basefare, timefare = appsetting_timefare, distancefare = appsetting_distance_fare, stopfare = appsetting_stop_fare, earning_percentage = apperaning_percentage });
            }
        }
    }

}