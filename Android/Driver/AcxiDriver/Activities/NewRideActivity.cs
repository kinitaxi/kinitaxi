using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcxiDriver.DataModels;
using AcxiDriver.Dialogue;
using AcxiDriver.EventListeners;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Database;
using Java.Util;
using Newtonsoft.Json;
using UK.CO.Chrisjenx.Calligraphy;

namespace AcxiDriver.Activities
{
    [Activity(Label = "NewRideActivity", Theme ="@style/AcxiTheme1")]
    public class NewRideActivity : AppCompatActivity
    {
        TextView txtlocation;
        TextView txtdestination;
        Button btnaccept;
        Button btndecline;
        FirebaseDatabase database;
        RiderDetails ride_details;
        string ride_data = "";
        string phone_s = "";
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        public System.Timers.Timer RideRequestTimer = new System.Timers.Timer();
        RequestValidityValueEventListener requestvalid_listener;
        DatabaseReference requestvalidRef;
        string ridevalid = "";
        int counter = 0;
            protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
 
                .SetDefaultFontPath("Fonts/Lato-Regular.ttf")

                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());
            SetContentView(Resource.Layout.rideintent);
            txtlocation = (TextView)FindViewById(Resource.Id.txtnewride_notify_location);
            txtdestination = (TextView)FindViewById(Resource.Id.txtnewride_notify_destination);
            btnaccept = (Button)FindViewById(Resource.Id.btnaccept_newride_notify);
            btndecline = (Button)FindViewById(Resource.Id.btndecline_newride_notify);

            btndecline.Click += Btndecline_Click;
            btnaccept.Click += Btnaccept_Click;
            phone_s = pref.GetString("phone", "");
            ride_data = Intent.GetStringExtra("ride_data");
            try
            {
                ride_details = JsonConvert.DeserializeObject<RiderDetails>(ride_data);
                txtlocation.Text = ride_details.pickup_address;
                txtdestination.Text = ride_details.destination_address;
            }
            catch
            {

            }

            SetUpFirebase();
            edit = pref.Edit();
           
        }

        public void SetUpFirebase()
        {

            try
            {
                var options = new FirebaseOptions.Builder()
                 .SetApplicationId("kinitaxi-1007b")
                 .SetApiKey("AIzaSyBGEbUGOVZaP5DLh3UK-cM1kF-bw7e-YMI")
                 .SetDatabaseUrl("https://kinitaxi-1007b.firebaseio.com")
                 .SetStorageBucket("kinitaxi-1007b.appspot.com")
                 .Build();
                var app = Firebase.FirebaseApp.InitializeApp(this, options);
                database = FirebaseDatabase.GetInstance(app);
            }
            catch
            {
                try
                {
                    var app = Firebase.FirebaseApp.InitializeApp(this);
                    database = FirebaseDatabase.GetInstance(app);
                }
                catch
                {
                    database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
                }

            }
        }



        public async void AcceptNewRide()
        {
            //RideRequestTimer.Interval = 1000;
            //RideRequestTimer.Elapsed += RideRequestTimer_Elapsed;
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();

           requestvalidRef = database.GetReference("rideRequest/" + ride_details.ride_id);
           requestvalid_listener = new RequestValidityValueEventListener();
            requestvalidRef.AddListenerForSingleValueEvent(requestvalid_listener);
            requestvalid_listener.IsRequestValid += (i, f) =>
            {
                progress.Dismiss();
                if (f.validity)
                {
                  //DatabaseReference  DriverAvailableRef = database.GetReference("driversAvailable/" + phone_s + "/);
                  //  DriverAvailableRef.RemoveValueAsync();

                    DatabaseReference   OrderAcceptedRef = database.GetReference("rideRequest/" + ride_details.ride_id + "/driver_id");
                    OrderAcceptedRef.SetValue(phone_s);

                    DatabaseReference thisride = database.GetReference("drivers/" + phone_s + "/trips/" + ride_details.ride_id);
                    thisride.SetValue(true);

                    string lng = pref.GetString("longitude", "");
                    string lat = pref.GetString("latitude", "");
                    if(!string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(lat))
                    {

                    }
                    HashMap map = new HashMap();
                    map.Put("longitude", lng);
                    map.Put("latitude", lat);

                    DatabaseReference DriverWorkingLocationRef = database.GetReference("driversWorking/" + phone_s);
                    DriverWorkingLocationRef.SetValue(map);
                    DatabaseReference cLocationRef = database.GetReference("rideCreated/" + ride_details.ride_id + "/driverLocation");
                    cLocationRef.SetValue(map);
                  

                    string jstr = JsonConvert.SerializeObject(ride_details);
                    DatabaseReference ongoing = database.GetReference("drivers/" + phone_s + "/ongoing");
                    ongoing.SetValue(jstr);

                    Intent intent = new Intent(this, typeof(EnrouteActivity));
                    intent.PutExtra("channel", "notification");
                    intent.PutExtra("ride", ride_data);
                    Finish();
                    StartActivity(intent);

                }
                else
                {
                    AppAlertDialogue appalert = new AppAlertDialogue("This request has been cancelled by rider");
                    appalert.Cancelable = true;
                    var trans = SupportFragmentManager.BeginTransaction();
                    appalert.Show(trans, "appalert");

                   // DatabaseReference DriverAvailableRef = database.GetReference("driversAvailable/" + phone_s);
                    //DriverAvailableRef.RemoveValueAsync();

                    appalert.AlertCancel += (a, s) =>
                    {
                        appalert.Dismiss();
                        this.Finish();
                    };

                    appalert.AlertOk += (d, g) =>
                    {
                        appalert.Dismiss();
                        this.Finish();
                    };
                    ridevalid = "false";
                }
               
                
            };

        }

        private void RideRequestTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(counter > 4)
            {

                requestvalidRef.RemoveEventListener(requestvalid_listener);
                //AppAlertDialogue appalert = new AppAlertDialogue("This request has been cancelled by rider");
                //appalert.Cancelable = true;
                //var trans = SupportFragmentManager.BeginTransaction();
                //appalert.Show(trans, "appalert");

                //DatabaseReference DriverAvailableRef = database.GetReference("driversAvailable/" + phone_s);
                //DriverAvailableRef.RemoveValueAsync();

                //appalert.AlertCancel += (a, s) =>
                //{
                //    appalert.Dismiss();
                //    this.Finish();
                //};

                //appalert.AlertOk += (d, g) =>
                //{
                //    appalert.Dismiss();
                //    this.Finish();
                //};

                Toast.MakeText(this, "This request has been cancelled by the rider", ToastLength.Short).Show();
                this.Finish();
            }
            counter++;
        }

        private async void Btnaccept_Click(object sender, EventArgs e)
        {
            AcceptNewRide();
        }

        private void Btndecline_Click(object sender, EventArgs e)
        {
            AppAlertDialogue appAlert = new AppAlertDialogue("You are about to DECLINE this ride request");
            appAlert.Cancelable = true;
            var trans1 = SupportFragmentManager.BeginTransaction();
            appAlert.Show(trans1, "appalert");

            appAlert.AlertCancel += (i, h) =>
            {
                appAlert.Dismiss();

            };

            appAlert.AlertOk += (y, t) =>
            {
                appAlert.Dismiss();
                this.Finish();
            };

        }

        public void AlertInit(string body)
        {
            AppAlertDialogue appAlert = new AppAlertDialogue(body);
            appAlert.Cancelable = true;
            var trans1 = SupportFragmentManager.BeginTransaction();
            appAlert.Show(trans1, "appalert");

            appAlert.AlertCancel += (i, h) =>
            {
                appAlert.Dismiss();
                appAlert = null;
            };

            appAlert.AlertOk += (y, t) =>
            {
                appAlert.Dismiss();
                appAlert = null;
            };

        }


        public override void OnBackPressed()
        {
            AppAlertDialogue appAlert = new AppAlertDialogue("You are about to DECLINE this ride request");
            appAlert.Cancelable = true;
            var trans1 = SupportFragmentManager.BeginTransaction();
            appAlert.Show(trans1, "appalert");

            appAlert.AlertCancel += (i, h) =>
            {
                appAlert.Dismiss();
               
            };

            appAlert.AlertOk += (y, t) =>
            {
                appAlert.Dismiss();
                this.Finish();
            };
           
        }
        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}