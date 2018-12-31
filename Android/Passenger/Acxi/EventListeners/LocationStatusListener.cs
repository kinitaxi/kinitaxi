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
  public  class LocationStatusListener : Java.Lang.Object, ILocationListener
    {
        public event EventHandler LocationTurnedOn;
        public event EventHandler LocationTurnedOff;
        public event EventHandler LocationStatusChanged;
        public void OnLocationChanged(Location location)
        {
           //
        }

        public void OnProviderDisabled(string provider)
        {
            //
            LocationTurnedOff.Invoke(this, new EventArgs());
        }

        public void OnProviderEnabled(string provider)
        {
            //
            LocationTurnedOn.Invoke(this, new EventArgs());
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //
            LocationStatusChanged.Invoke(this, new EventArgs());
        }
    }
}