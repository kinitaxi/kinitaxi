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
    public class TicketKeysValueEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<TicketKeysEventArgs> OnTickeyKeys;
        public event EventHandler TicketEmpty;
        public class TicketKeysEventArgs : EventArgs
        {
            public List<string> listKey { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
           
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                var childd = snapshot.Children.ToEnumerable<DataSnapshot>();
                List<string> slist = new List<string>();
                foreach (DataSnapshot s in childd)
                {
                    string ss = s.Key;
                    slist.Add(ss);
                }

                OnTickeyKeys.Invoke(this, new TicketKeysEventArgs { listKey = slist });
            }
            else
            {
                TicketEmpty.Invoke(this, new EventArgs());
            }
        }
    }
}