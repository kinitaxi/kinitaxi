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
    public class TicketDetailsValueEventListener : Java.Lang.Object, IValueEventListener
    {
        public event EventHandler<TicketDetailsEventArgs> OnTicketDetails;
        public class TicketDetailsEventArgs : EventArgs
        {
            public string category { get; set; }
            public double created_at { get; set; }
            public string title { get; set; }
            public string status { get; set; }
            public string message { get; set; }
        }
        public void OnCancelled(DatabaseError error)
        {
           
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           if(snapshot.Value != null)
            {
                string created_at = "0";
                string category = "";
                string title = "";
                string status = "";
                string message = "";
               
                if(snapshot.Child("created_at").Value != null)
                {
                    created_at = snapshot.Child("created_at").Value.ToString();
                }
                if(snapshot.Child("category").Value != null)
                {
                    category = snapshot.Child("category").Value.ToString();
                }
                if(snapshot.Child("title").Value != null)
                {
                    title = snapshot.Child("title").Value.ToString();
                }
                if (snapshot.Child("status").Value != null)
                {
                    status =  snapshot.Child("status").Value.ToString();
                }
                if (snapshot.Child("message").Value!= null)
                {
                    message = snapshot.Child("message").Value.ToString();
                }

                OnTicketDetails.Invoke(this, new TicketDetailsEventArgs { category = category, created_at = double.Parse(created_at), message = message, status = status, title = title }); 
            }
           
        }
    }
}