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
using Java.Util;
using AcxiDriver.DataModels;

namespace AcxiDriver.EventListeners
{
    public class TripCountValueEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<TripCountEventArgs> OnTripretrieved;
        public class TripCountEventArgs : EventArgs
        {
            public int Trips { get; set; }
            public List<string> Keys { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
           //
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
               string keycount = snapshot.ChildrenCount.ToString();

                var childd = snapshot.Children.ToEnumerable<DataSnapshot>();
                List<string> slist = new List<string>();
                foreach (DataSnapshot s in childd)
                {
                    string ss = s.Key;
                    slist.Add(ss);
                }

                OnTripretrieved.Invoke(this, new TripCountEventArgs { Trips = int.Parse(keycount), Keys = slist });
            }
        }


    }
}