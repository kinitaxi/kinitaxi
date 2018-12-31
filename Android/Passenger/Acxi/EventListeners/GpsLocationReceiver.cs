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
using Android.Locations;

namespace Acxi.EventListeners
{
    [BroadcastReceiver]
    public class GpsLocationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //if (intent.Action == LocationManager.GpsProvider)
            //{
            //    Toast.MakeText(context, "in android.location.PROVIDERS_CHANGED", ToastLength.Long).Show();
            //    Intent pushIntent = new Intent(context, loca);
            //context.startService(pushIntent);

            if (intent.Action =="android.location.PROVIDERS_CHANGED")
            {
               
              //  Intent pushIntent = new Intent(context, LocalService.class);
           /// context.startService(pushIntent);
        }
}
}

}
