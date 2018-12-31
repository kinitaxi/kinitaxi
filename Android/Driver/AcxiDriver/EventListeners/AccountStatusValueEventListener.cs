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
    public class AccountStatusValueEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<AccountStatusEventArgs> AccountStatus;
        public event EventHandler DriverAccountDeleted;

        public class AccountStatusEventArgs : EventArgs
        {
            public string account_status { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                AccountStatus.Invoke(this, new AccountStatusEventArgs { account_status = snapshot.Value.ToString() });
            }
            else
            {
                DriverAccountDeleted.Invoke(this, new EventArgs());
            }
        }
    }
}