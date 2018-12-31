using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Firebase.Messaging;
using AcxiDriver.Activities;
using Android.Graphics;
using System.Collections.Generic;
using static Android.App.ActivityManager;

namespace AcxiDriver
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        string appmainstate;
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            string type = "";

            try
            {
                type = message.Data["type"];

                if (!string.IsNullOrEmpty(type))
                {
                    if(type == "ride")
                    {
                        string ride_data = message.Data["ride"];
                        SendRideNotification(ride_data);
                    }
                    else if(type == "message")
                    {
                        string messageTitle = message.Data["title"];
                        string messageBody = message.Data["message"];
                        SendMessageNotification(messageTitle, messageBody); 
                    }
                }
            }
            catch
            {

            }

           
        }

        void SendRideNotification(string data)
        {
            Intent intent;
            PendingIntent pendingIntent = null;
            appmainstate = pref.GetString("appmainstate", "");

            if (appmainstate == "hidden")
            {
                intent = new Intent(this, typeof(NewRideActivity));
                intent.PutExtra("ride_data", data);
                intent.AddFlags(ActivityFlags.NewTask);
                const int pendingIntentId = 0;
                pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

                var path = Android.Net.Uri.Parse("android.resource://com.kinidriver.ng/" + Resource.Raw.alert);

                var notificationBuilder = new Notification.Builder(this)
                    .SetSmallIcon(Resource.Mipmap.ic_directions_car_black_48dp)
                    .SetContentTitle("KiniTaxi")
                    .SetDefaults(NotificationDefaults.Lights | NotificationDefaults.Vibrate)
                    // .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                    .SetSound(path)
                    .SetPriority((int)NotificationPriority.Max)
                    .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.kinid))
                    .SetContentText("You have a new ride request")
                    .SetAutoCancel(true)
                    .SetTicker("You have a new ride request")
                     .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
                    .SetContentIntent(pendingIntent);

                var notificationManager = NotificationManager.FromContext(this);
                notificationManager.Notify(0, notificationBuilder.Build());

            }
            else if (appmainstate == "visible")
            {

            }
            else
            {
                intent = new Intent(this, typeof(NewRideActivity));
                intent.PutExtra("ride_data", data);
                intent.AddFlags(ActivityFlags.NewTask);
                const int pendingIntentId = 0;
                pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

                var path = Android.Net.Uri.Parse("android.resource://com.kinidriver.ng/" + Resource.Raw.alert);

                var notificationBuilder = new Notification.Builder(this)
                    .SetSmallIcon(Resource.Mipmap.ic_directions_car_black_48dp)
                    .SetContentTitle("KiniTaxi")
                    .SetDefaults(NotificationDefaults.Lights | NotificationDefaults.Vibrate)
                    // .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                    .SetSound(path)
                    .SetPriority((int)NotificationPriority.Max)
                    .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.kinid))
                    .SetContentText("You have a new ride request")
                    .SetAutoCancel(true)
                    .SetTicker("You have a new ride request")
                     .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
                    .SetContentIntent(pendingIntent);

                var notificationManager = NotificationManager.FromContext(this);
                notificationManager.Notify(0, notificationBuilder.Build());

            }


        }

        void SendMessageNotification(string title, string message)
        {

            Intent intent = new Intent(this, typeof(AppMainActivity));
            intent.PutExtra("title", title);
            intent.PutExtra("message", message);
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask | ActivityFlags.ClearTop);

            const int pendingIntentId = 0;
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

            var path = Android.Net.Uri.Parse("android.resource://com.kinidriver.ng/" + Resource.Raw.alert);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Mipmap.ic_directions_car_black_48dp)
                .SetContentTitle("KiniTaxi")
                .SetDefaults(NotificationDefaults.Lights | NotificationDefaults.Vibrate)
                // .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                .SetSound(path)
                .SetPriority((int)NotificationPriority.Max)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.acxid))
                .SetContentText("You have a new ride request")
                .SetAutoCancel(true)
                .SetTicker("You have a new ride request")
                 .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());



        }

        static bool shouldShowNotification(Context context)
        {
            RunningAppProcessInfo myProcess = new RunningAppProcessInfo();
            ActivityManager.GetMyMemoryState(myProcess);
            if (myProcess.Importance != RunningAppProcessInfo.ImportanceForeground)
                return true;

            KeyguardManager km = (KeyguardManager)context.GetSystemService(Context.KeyguardService);
            // app is in foreground, but if screen is locked show notification anyway
            return km.InKeyguardRestrictedInputMode();
        }
    }
}