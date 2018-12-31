using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Firebase.Messaging;


namespace Acxi
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);
            Log.Debug(TAG, "Title: " + message.GetNotification().Title);
            Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);

            SendNotification(message.GetNotification().Body, message.GetNotification().Title);
        }

        void SendNotification(string messageBody, string messageTitle)
        {
           // var intent = new Intent(this, typeof(MainActivity));
         //   intent.AddFlags(ActivityFlags.ClearTop);
         //   var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
			Intent intent = new Intent(this, typeof(MainActivity));
            // intent.SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            //  intent.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask);
            // Create a PendingIntent; we're only using one PendingIntent (ID = 0):
            intent.AddFlags(ActivityFlags.ClearTop);
			const int pendingIntentId = 0;
			PendingIntent pendingIntent =
				PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);


            var notificationBuilder = new Notification.Builder(this)
                 .SetSmallIcon(Resource.Mipmap.ic_launcher)
                 .SetContentTitle(messageTitle)
                 .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                 .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                 .SetContentText(messageBody)
                 .SetAutoCancel(true)
                  .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
                 .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}