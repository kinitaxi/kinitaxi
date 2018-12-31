using System;
using Android.App;
using Firebase.Iid;
using Android.Util;
using System.Collections.Generic;
using Android.Content;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace Acxi
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);

            SendRegistrationToServer(refreshedToken);
        }
      void SendRegistrationToServer(string token)
        {

            Console.WriteLine("RefreshedToken = " + token);

        }

       
    }
}