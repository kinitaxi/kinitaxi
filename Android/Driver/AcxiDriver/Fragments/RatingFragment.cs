using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Database;
using AcxiDriver.EventListeners;
using Firebase;
using AcxiDriver.DataModels;
using System.Globalization;
using Newtonsoft.Json;
using AcxiDriver.Activities;

namespace AcxiDriver.Fragments
{
    public class RatingFragment : Android.Support.V4.App.Fragment
    {

        TextView txtrating;
        TextView txtfivestar;
        TextView txtcompletion;
        TextView txtcancellation;
        LinearLayout layshow_feedback;

        FirebaseDatabase database;
        DatabaseReference tripsref;
        DatabaseReference ridesref;
        TripCountValueEventListener trip_listener;
        RatingValueEventListener rating_listerner;
        RiderNameValueEventListener name_listerner;
        List<string> rideKeys;

        List<Rating_Status> listrating_status = new List<Rating_Status>();
        string phone = "";
        int counter = 0;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            edit = pref.Edit();
            phone = pref.GetString("phone", "");
            database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
            trip_listener = new TripCountValueEventListener();
            trip_listener.OnTripretrieved += Trip_listener_OnTripretrieved;
            tripsref = database.GetReference("drivers/" + phone + "/trips");
            tripsref.AddValueEventListener(trip_listener);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            View view = inflater.Inflate(Resource.Layout.ratings, container, false);
            txtfivestar = (TextView)view.FindViewById(Resource.Id.txtfivestars_rating);
            txtcompletion = (TextView)view.FindViewById(Resource.Id.txtcompletion_rating);
            txtcancellation = (TextView)view.FindViewById(Resource.Id.txtcancellation_rating);
            txtrating = (TextView)view.FindViewById(Resource.Id.txtrating_rating);
            layshow_feedback = (LinearLayout)view.FindViewById(Resource.Id.layshow_feedback);
            layshow_feedback.Click += Layshow_feedback_Click;
            return view;
        }

        private void Layshow_feedback_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(FeedbackActivity)));
        }

        private void Trip_listener_OnTripretrieved(object sender, TripCountValueEventListener.TripCountEventArgs e)
        {
            listrating_status.Clear();
            rideKeys = e.Keys;
            foreach (string ride in rideKeys)
            {
                DatabaseReference thisride = database.GetReference("rideCreated/" + ride);
                rating_listerner = new RatingValueEventListener();
                thisride.AddListenerForSingleValueEvent(rating_listerner);
                rating_listerner.OnRatingDetermined += Rating_listerner_OnRatingDetermined;
            }

            DatabaseReference tripsCount = database.GetReference("drivers/" + phone + "/trip_count");
            tripsCount.SetValue(rideKeys.Count);


        }

        private void CalculateRating()
        {
            double totalrating = 0;
            double fivestar = 0;
            double rating = 0;
            double cancelation = 0;
            double completion = 0;

            foreach(Rating_Status item in listrating_status)
            {
               
                if(item.rating == 5)
                {
                    fivestar += 1;
                }

                if(item.status == "ended")
                {
                    totalrating += item.rating;
                    completion += 1;
                }
                else if (item.status.Contains("cancel"))
                {
                    cancelation += 1;
                }
            }

            rating = Math.Round(totalrating / completion, 2);
            string formatRating = rating.ToString("#0.0", CultureInfo.InvariantCulture);
            DatabaseReference formatRating_ref = database.GetReference("drivers/" + phone + "/driver_rating");
            formatRating_ref.SetValue(formatRating);
            txtrating.Text = formatRating;
            txtfivestar.Text = fivestar.ToString();
            txtcancellation.Text = Math.Round((cancelation / listrating_status.Count) * 100, 0).ToString() + "%";
            txtcompletion.Text = Math.Round((completion / listrating_status.Count) * 100, 0).ToString() + "%";
        }

        private void Rating_listerner_OnRatingDetermined(object sender, RatingValueEventListener.RatingValueEventArgs e)
        {
            listrating_status.Add(new Rating_Status { rating = e.rating, status = e.status, feedback = e.feedback, user_id = e.user_id, created_at = e.created_at });
            CalculateRating();
            Console.WriteLine("listcount = " + listrating_status.Count.ToString());

            if (listrating_status.Count == rideKeys.Count)
            {
                GetRiderName();
            }
        }

        public void GetRiderName()
        {
            foreach( Rating_Status item in listrating_status)
            {
                string user_id = listrating_status[listrating_status.Count - 1].user_id;
                DatabaseReference usernameRef = database.GetReference("users/" + user_id + "/first_name");
                name_listerner = new RiderNameValueEventListener();
                usernameRef.AddListenerForSingleValueEvent(name_listerner);
                name_listerner.GetRiderName += Name_listerner_GetRiderName;
            }
          
        }

        private void Name_listerner_GetRiderName(object sender, RiderNameValueEventListener.RiderNameEventArgs e)
        {

            try
            {
                listrating_status[counter].user_name = e.first_name;
                counter += 1;
            }
            catch
            {

            }          
                // Console.WriteLine(" Users name = " + listrating_status[counter].user_name + " ride id = " + listrating_status[counter].user_id + " rating = " + listrating_status[counter].rating + " status = " + listrating_status[counter].status  + " Feedback = " + listrating_status[counter].feedback);

                if (counter == listrating_status.Count)
                {
                    Console.WriteLine(counter);
                    var list = listrating_status.OrderByDescending(o => o.created_at).ToList();
                    string jstr = JsonConvert.SerializeObject(list);
                    edit.PutString("feedback", jstr);
                    edit.Apply();

                return;
                }
            }
            
          
        }
    }
