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
    public class AppUpdateEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<appUpdateNew> AppUpdateFound;
        public class appUpdateNew : EventArgs
        {
            public string appVersion { get; set; }
            public string forceUpdate { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
            //throw new NotImplementedException();
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            string force_d = "", version_d = "";
           
            if(snapshot.Child("force_update_d").Value != null)
            {
                force_d = snapshot.Child("force_update_d").Value.ToString();
            }

            if(snapshot.Child("version_d").Value != null)
            {
                version_d = snapshot.Child("version_d").Value.ToString();
            }

            if(!string.IsNullOrEmpty(force_d) && !string.IsNullOrEmpty(version_d))
            {
                AppUpdateFound.Invoke(this, new appUpdateNew { appVersion = version_d, forceUpdate = force_d });
            }
        }
    }
}