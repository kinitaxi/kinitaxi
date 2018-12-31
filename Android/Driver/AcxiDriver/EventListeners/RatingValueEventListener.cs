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
    public class RatingValueEventListener : Java.Lang.Object, IValueEventListener
    {

        public event EventHandler<RatingValueEventArgs> OnRatingDetermined;
        public class RatingValueEventArgs : EventArgs
        {
            public int rating { get; set; }
            public string status { get; set; }
            public string feedback { get; set; }
            public string user_id { get; set; }
            public string created_at { get; set; }

        }
        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            string rating = "0";
            string status = "unknown";
            string feedback = "";
            string user_id = "";
            string created_at = "";

            if (snapshot.Child("rating").Value != null)
            {
                rating = snapshot.Child("rating").Value.ToString();
            }

            if(snapshot.Child("status").Value != null)
            {
                status = snapshot.Child("status").Value.ToString();
            }
            if(snapshot.Child("rider_id").Value != null)
            {
                user_id = snapshot.Child("rider_id").Value.ToString();
            }
            if(snapshot.Child("feedback").Value != null)
            {
                feedback = snapshot.Child("feedback").Value.ToString();
            }
            if(snapshot.Child("created_at").Value != null)
            {
                created_at = snapshot.Child("created_at").Value.ToString();
            }
            OnRatingDetermined.Invoke(this, new RatingValueEventArgs { rating = int.Parse(rating), status = status, feedback = feedback, user_id = user_id, created_at = created_at });
        }
    }
}