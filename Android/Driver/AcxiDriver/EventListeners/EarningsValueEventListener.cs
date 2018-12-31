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
    public class EarningsValueEventListener : Java.Lang.Object, IValueEventListener
    {

        public event EventHandler<OnEarningValue> EarningChanged;
        public class OnEarningValue : EventArgs
        {
            public double earningOverall { get; set; }
            public double earningBalance { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {

            if (snapshot.Value != null)
            {
                string eo = "0";
                string eb = "0";
                // Toast.MakeText(Application.Context, "changed no null", ToastLength.Short).Show();
                if (snapshot.Child("earning_overall").Value != null)
                {
                    eo = snapshot.Child("earning_overall").Value.ToString();
                }
                if (snapshot.Child("earning_unpaid").Value != null)
                {
                    eb = snapshot.Child("earning_unpaid").Value.ToString();
                }
                EarningChanged.Invoke(this, new OnEarningValue { earningBalance = double.Parse(eb), earningOverall = double.Parse(eo) });

            }
        }
    }

}