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
using Android.Support.V7.App;

using Android.Support.V4.Content;
using Com.Ittianyu.Bottomnavigationviewex;
using Android.Graphics;
using Android.Support.V4.Content.Res;
using UK.CO.Chrisjenx.Calligraphy;
using Android.Support.V7.Widget;
using Android.Support.V4.View;
using Android.Support.V4.App;

using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using AcxiDriver.Fragments;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content.Res;
using AcxiDriver.Dialogue;
using Firebase;
using Firebase.Database;
using AcxiDriver.DataModels;
using Java.Util;
using Acxi.Helpers;
using Newtonsoft.Json;
using System.Threading;
using AcxiDriver.EventListeners;
using Android.Locations;
using static Android.Gms.Common.Apis.GoogleApiClient;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;
using Android;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Android.Media;

namespace AcxiDriver.Activities
{
    [Activity(Label = "AppMainActivity", MainLauncher = false, Theme ="@style/AcxiTheme1", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait )]
    public class AppMainActivity : AppCompatActivity, ViewPager.IOnTouchListener
    {
    
        BottomNavigationViewEx bnve;
        Android.Support.V7.Widget.Toolbar mtoolbar;
        SwitchCompat mSwitch;
        bool availability = false;

        ViewPager view_pager;
        HomeFragment homeFrag = new HomeFragment();
        AccountFragment accFrag = new AccountFragment();
        RatingFragment ratingFrag = new RatingFragment();
        EarningsFragment earnFrag = new EarningsFragment();
        NewRideDialogue newridedialogue;

        // SETS WHEN DRIVE IS AVAILABLE ONLINE

        public bool DriverOnline = false;
        //SETS WHEN DRIVER HAS BEEN MATCHED BUT HAS NOT ACCEPTED;

        public bool DriverMatched = false;
        //SETS WHEN DRIVER HAS BEEN MATCHED AND HAS BEEN ACCEPTED;

        public bool DriverRideAccepted = false;

        //APPDATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        string phone_s;

        //FIREBASE VARIABLES
        FirebaseDatabase database;
        DatabaseReference DriverAvailableRef;
        DatabaseReference DriverLocationRef;
        public DatabaseReference RiderFoundRef;
        public DatabaseReference RiderRequestRef;
        public DatabaseReference OrderAcceptedRef;
        public DatabaseReference DriverWorkingLocationRef;
        public DatabaseReference OngoingRideRef;
        DatabaseReference rejectref;
        DatabaseReference acceptCheck;
        RideAssignedValueListener rideassigned_listerner;


        private const int MY_PERMISSION_REQUEST_CODE = 7171;
        private const int PLAY_SERVICES_RESOLUTION_REQUEST = 7172;
        private bool mRequestingLocationUpdates = false;
        private LocationRequest mLocationRequest;
        private GoogleApiClient mGoogleApiClient;
        private Android.Locations.Location mLastLocation;

        private static int UPDATE_INTERVAL = 5000; //SEC
        private static int FASTEST_INTERVAL = 3000; //SEC
        private static int DISPLACEMENT = 10; // METERS

        public string rideID;
        public RiderDetails riderDetails;
        WebRequestHelpers webHelpers = new WebRequestHelpers();
        HelperFunctions helper = new HelperFunctions();
       public static MediaPlayer player;
        private bool isBackground = false;
        bool background_notified = false;


        #region VIEW SETUPS
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            await TryToGetPermissions();

            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());

            SetContentView(Resource.Layout.appmain);
            this.Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.KeepScreenOn);

            bnve = FindViewById<BottomNavigationViewEx>(Resource.Id.bnve);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.appmainToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Go online";

            edit = pref.Edit();
            phone_s = pref.GetString("phone", "");
            SetUpFirebase();

           
            mSwitch = (SwitchCompat)FindViewById(Resource.Id.compatSwitch);
            mSwitch.Checked = false;
            mSwitch.Click += MSwitch_Click;

            view_pager = (ViewPager)FindViewById(Resource.Id.viewpager);
            // view_pager.SetOnTouchListener(this);

            view_pager.OffscreenPageLimit = 3;
            view_pager.BeginFakeDrag();

            SetupViewPager(view_pager);
          // bnve.EnableAnimation(false);
            bnve.EnableItemShiftingMode(false);
            bnve.EnableShiftingMode(false);
            bnve.NavigationItemSelected += Bnve_NavigationItemSelected;

            var img = bnve.GetIconAt(0);
            var txt = bnve.GetLargeLabelAt(0);
            img.SetColorFilter(Color.Rgb(252, 140, 30));
            txt.SetTextColor(Color.Rgb(252, 140, 30));

          
            homeFrag.MOnLocationUpdate += HomeFrag_MOnLocationUpdate;
            homeFrag.MFirstLocation += HomeFrag_MFirstLocation;

            // CheckDocuments();

            TurnOnLOcationService();
            DatabaseReference tripRecoverRef = database.GetReference("drivers/" + phone_s + "/ongoing");
            TripRecoveryValueEventListener tripRecovery_listener = new TripRecoveryValueEventListener();
            tripRecoverRef.AddListenerForSingleValueEvent(tripRecovery_listener);
            tripRecovery_listener.IsTripOngoing += TripRecovery_listener_IsTripOngoing;
            UpdateApp();
        }


        public void UpdateApp()
        {
            DatabaseReference appUpdateRef = database.GetReference("appSettings");
            AppUpdateEventListener appupdate_listener = new AppUpdateEventListener();
            appUpdateRef.AddValueEventListener(appupdate_listener);
            appupdate_listener.AppUpdateFound += Appupdate_listener_AppUpdateFound;
        }

        private void Appupdate_listener_AppUpdateFound(object sender, AppUpdateEventListener.appUpdateNew e)
        {
            string appPackageName = PackageManager.GetPackageInfo(PackageName, 0).PackageName.ToString();
            string main_version = PackageManager.GetPackageInfo(PackageName, 0).VersionCode.ToString();

            if (e.forceUpdate == "yes")
            {
                if (!string.IsNullOrEmpty(e.appVersion) && !string.IsNullOrEmpty(main_version))
                {
                    if (int.Parse(e.appVersion) > int.Parse(main_version))
                    {
                        AppUpdateDialogue appUpdate_dialogue = new AppUpdateDialogue();
                        var trans = SupportFragmentManager.BeginTransaction();
                        appUpdate_dialogue.Cancelable = false;

                        try
                        {
                            appUpdate_dialogue.Show(trans, "appupdate");
                        }
                        catch
                        {
                            return;
                        }

                        appUpdate_dialogue.OnUpdateApp += (p, n) =>
                        {
                            //ToDo lauch intent to playstore;
                            Android.Net.Uri market_uri = Android.Net.Uri.Parse("market://details?id=" + appPackageName);
                            Android.Net.Uri store_uri = Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName);

                            try
                            {
                                Intent market = new Intent(Intent.ActionView, market_uri);
                                StartActivity(market);
                                FinishAffinity();
                            }
                            catch
                            {
                                Intent store = new Intent(Intent.ActionView, store_uri);
                                StartActivity(store);
                                FinishAffinity();
                            }
                        };
                    }
                }

            }
        }

        public override void OnBackPressed()
        {
            AppAlertDialogue appAlert = new AppAlertDialogue("You are about to QUIT app, minimize instead to keep app active");
            appAlert.Cancelable = true;
            var trans1 = SupportFragmentManager.BeginTransaction();
            appAlert.Show(trans1, "appalert");

            appAlert.AlertCancel += (i, h) =>
            {
                appAlert.Dismiss();
            };

            appAlert.AlertOk += (y, t) =>
            {
                GoOffline();
                base.OnBackPressed();
            };
           
        }
        private void TripRecovery_listener_IsTripOngoing(object sender, TripRecoveryValueEventListener.OnTripEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.ongoing))
            {
                if(e.ongoing.Length > 15)
                {
                    if (!DriverMatched && !DriverRideAccepted)
                    {
                        RecoverTrip(e.ongoing);
                    }
                }
            }
        }

        public void RecoverTripInvalid()
        {
            DatabaseReference tripOngoing = database.GetReference("drivers/" + phone_s + "/ongoing");
            tripOngoing.RemoveValue();
        }
        public void RecoverTrip(string details)
        {
            var ride_details = JsonConvert.DeserializeObject<RiderDetails>(details);

            DatabaseReference ridedetailsRef = database.GetReference("rideCreated/" + ride_details.ride_id);
            RideDetailsValueEvenListener ridedetails_listener = new RideDetailsValueEvenListener();
            ridedetailsRef.AddListenerForSingleValueEvent(ridedetails_listener);
            ridedetails_listener.isTripStatus += (o, w) =>
            {
                
                if (!string.IsNullOrEmpty(w.status))
                {
                    if (w.status.Contains("cancel"))
                    {
                        return;
                    }
                    else
                    {
                        AppAlertDialogue appalert = new AppAlertDialogue("Continue your trip to " + ride_details.destination_address.ToUpper());
                        appalert.Cancelable = false;
                        var trans = SupportFragmentManager.BeginTransaction();
                        appalert.Show(trans, "alert");

                        appalert.AlertCancel += (i, q) =>
                        {
                            appalert.Dismiss();
                        };

                        appalert.AlertOk += (q, a) =>
                        {
                            appalert.Dismiss();
                            Intent intent = new Intent(this, typeof(EnrouteActivity));
                            intent.PutExtra("ride", details);
                            intent.PutExtra("resume_status", w.status);
                            Finish();
                            StartActivity(intent);
                        };
                    }
                }
                else
                {
                    RecoverTripInvalid();
                    Console.WriteLine("Nullified");
                }
              
            };
        }

        public void TurnOnLOcationService()
        {
            // Get Location Manager and check for GPS & Network location services
            LocationManager lm = (LocationManager)this.GetSystemService(Context.LocationService);
            if (!lm.IsProviderEnabled(LocationManager.GpsProvider) ||
                  !lm.IsProviderEnabled(LocationManager.NetworkProvider))
            {
                AppAlertDialogue appalert = new AppAlertDialogue("Please enable your Location Service");
                appalert.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction(); ;
                appalert.Show(trans, "alert");
                appalert.AlertCancel += (o, y) =>
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    StartActivity(intent);
                    appalert.Dismiss();
                };
                appalert.AlertOk += (o, i) =>
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    StartActivity(intent);
                    appalert.Dismiss();
                };
                //              // Build the alert dialog
                //              Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
                //              builder.SetTitle("Location Services Not Active");
                //              builder.SetMessage("Please enable Location Services and GPS");
                //              builder.SetPositiveButton("OK", new dialo
                ////              {
                //public void onClick(DialogInterface dialogInterface, int i)
                //      {
                //          // Show location settings when the user acknowledges the alert dialog
                //          Intent intent = new Intent(Settings.ACTION_LOCATION_SOURCE_SETTINGS);
                //          startActivity(intent);
                //      }
                //  });
                //Dialog alertDialog = builder.create();
                //  alertDialog.setCanceledOnTouchOutside(false);
                //alertDialog.show();
            }
        }

        public void CheckDocuments()
        {
           

            string worthiness_string = pref.GetString("worthiness", "");
            if (string.IsNullOrEmpty(worthiness_string))
            {
                string worthiness_url = pref.GetString("worthiness_url", "");
                if (string.IsNullOrEmpty(worthiness_url))
                {
                    Thread t2 = new Thread(delegate ()
                    {
                        string imagestring = webHelpers.downloadImage(worthiness_url);
                        edit.PutString("worthiness", imagestring);
                        edit.Apply();
                    });
                    t2.Start();
                }
            }
        }

        private void SetupViewPager(ViewPager view_pager)
        {
            ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);

            adapter.AddFragment(homeFrag, "Home");
            adapter.AddFragment(earnFrag, "Earnings");
            adapter.AddFragment(ratingFrag, "Rating");
            adapter.AddFragment(accFrag, "Account");
            view_pager.Adapter = adapter;
        }
       
        public void GoOffline()
        {
            SupportActionBar.Title = "Go online";
            Toast.MakeText(this, "You are now OFFLINE", ToastLength.Short).Show();
            availability = false;
            homeFrag.GoOffline();

            if (DriverAvailableRef != null)
            {
                DriverAvailableRef.RemoveValueAsync();
                if(rideassigned_listerner!= null)
                {
                    DriverAvailableRef.RemoveEventListener(rideassigned_listerner);
                }
            }
        }

        public void GoOnline()
        {
            bool response = false;
            response = homeFrag.GoOnline();
            if (response)
            {
                SupportActionBar.Title = "You are online";
                Toast.MakeText(this, "You are now ONLINE", ToastLength.Short).Show();
                availability = true;
            }
            else
            {
                Toast.MakeText(this, "TurnOn your location service and try again", ToastLength.Short).Show();
                mSwitch.Toggle();
            }
        }

        private void MSwitch_Click(object sender, EventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    if (availability)
                    {
                        GoOffline();
                    }
                    else
                    {
                        GoOnline();
                    }
                }
                catch
                {
                    Toast.MakeText(this, "Something went wrong, please try again", ToastLength.Short).Show();
                }
               
            }
            else
            {
                mSwitch.Toggle();
                Toast.MakeText(this, "No internet connectivity", ToastLength.Short).Show();
            }
        }

        private void Bnve_NavigationItemSelected(object sender, Android.Support.Design.Widget.BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.action_earning)
            {
                var img = bnve.GetIconAt(1);
                var txt = bnve.GetLargeLabelAt(1);

                img.SetColorFilter(Color.Rgb(252, 140, 30));
                txt.SetTextColor(Color.Rgb(252, 140, 30));

                var img0 = bnve.GetIconAt(0);
                var txt0 = bnve.GetLargeLabelAt(0);
                img0.SetColorFilter(Color.Rgb(255, 255, 255));
                txt0.SetTextColor(Color.Rgb(255, 255, 255));

                var img2 = bnve.GetIconAt(2);
                var txt2 = bnve.GetLargeLabelAt(2);
                img2.SetColorFilter(Color.Rgb(255, 255, 255));
                txt2.SetTextColor(Color.Rgb(255, 255, 255));

                var img3 = bnve.GetIconAt(3);
                var txt3 = bnve.GetLargeLabelAt(3);
                img3.SetColorFilter(Color.Rgb(255, 255, 255));
                txt3.SetTextColor(Color.Rgb(255, 255, 255));
                view_pager.SetCurrentItem(1, true);
            }
            else if( e.Item.ItemId == Resource.Id.action_home)
            {
                var img = bnve.GetIconAt(0);
                var txt = bnve.GetLargeLabelAt(0);
                img.SetColorFilter(Color.Rgb(252, 140, 30));
                txt.SetTextColor(Color.Rgb(252, 140, 30));

                var img1 = bnve.GetIconAt(1);
                var txt1 = bnve.GetLargeLabelAt(1);
                img1.SetColorFilter(Color.Rgb(255, 255, 255));
                txt1.SetTextColor(Color.Rgb(255, 255, 255));

                var img2 = bnve.GetIconAt(2);
                var txt2 = bnve.GetLargeLabelAt(2);
                img2.SetColorFilter(Color.Rgb(255, 255, 255));
                txt2.SetTextColor(Color.Rgb(255, 255, 255));

                var img3 = bnve.GetIconAt(3);
                var txt3 = bnve.GetLargeLabelAt(3);
                img3.SetColorFilter(Color.Rgb(255, 255, 255));
                txt3.SetTextColor(Color.Rgb(255, 255, 255));
                view_pager.SetCurrentItem(0, true);

            }
            else if(e.Item.ItemId == Resource.Id.action_rating)
            {
                var img = bnve.GetIconAt(2);
                var txt = bnve.GetLargeLabelAt(2);
                img.SetColorFilter(Color.Rgb(252, 140, 30));
                txt.SetTextColor(Color.Rgb(252, 140, 30));

                var img0 = bnve.GetIconAt(0);
                var txt0 = bnve.GetLargeLabelAt(0);
                img0.SetColorFilter(Color.Rgb(255, 255, 255));
                txt0.SetTextColor(Color.Rgb(255, 255, 255));

                var img1 = bnve.GetIconAt(1);
                var txt1 = bnve.GetLargeLabelAt(1);
                img1.SetColorFilter(Color.Rgb(255, 255, 255));
                txt1.SetTextColor(Color.Rgb(255, 255, 255));

                var img3 = bnve.GetIconAt(3);
                var txt3 = bnve.GetLargeLabelAt(3);
                img3.SetColorFilter(Color.Rgb(255, 255, 255));
                txt3.SetTextColor(Color.Rgb(255, 255, 255));
                view_pager.SetCurrentItem(2, true);

            }
            else if (e.Item.ItemId == Resource.Id.action_account)
            {
                var img = bnve.GetIconAt(3);
                var txt = bnve.GetLargeLabelAt(3);
                img.SetColorFilter(Color.Rgb(252, 140, 30));
                txt.SetTextColor(Color.Rgb(252, 140, 30));

                var img0 = bnve.GetIconAt(0);
                var txt0 = bnve.GetLargeLabelAt(0);
                img0.SetColorFilter(Color.Rgb(255, 255, 255));
                txt0.SetTextColor(Color.Rgb(255, 255, 255));

                var img2 = bnve.GetIconAt(2);
                var txt2 = bnve.GetLargeLabelAt(2);
                img2.SetColorFilter(Color.Rgb(255, 255, 255));
                txt2.SetTextColor(Color.Rgb(255, 255, 255));

                var img1 = bnve.GetIconAt(1);
                var txt1 = bnve.GetLargeLabelAt(1);
                img1.SetColorFilter(Color.Rgb(255, 255, 255));
                txt1.SetTextColor(Color.Rgb(255, 255, 255));
                view_pager.SetCurrentItem(3, true);

            }
        }
        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        bool View.IOnTouchListener.OnTouch(View v, MotionEvent e)
        {
            return false;
        }

        public class ViewPagerAdapter : FragmentPagerAdapter
        {
            public List<SupportFragment> Fragments { get; set; }
            public List<string> FragmentNames { get; set; }

            public ViewPagerAdapter(SupportFragmentManager sfm) : base(sfm)
            {
                Fragments = new List<SupportFragment>();
                FragmentNames = new List<string>();
            }

            public void AddFragment(SupportFragment fragment, string name)
            {
                Fragments.Add(fragment);
                FragmentNames.Add(name);
            }


            public override int Count
            {
                get
                {
                    return Fragments.Count;
                }
            }

            public override SupportFragment GetItem(int position)
            {
                return Fragments[position];
            }
        }

        #endregion

        #region FIREBASE FUNCTION

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


        private void HomeFrag_MFirstLocation(object sender, OnLocationUpdatedEventArgs e)
        {
            DriverAvailableRef = database.GetReference("driversAvailable/" + phone_s);

            HashMap location = new HashMap();
            location.Put("longitude", e.Longitude.ToString());
            location.Put("latitude", e.Latitude.ToString());
            edit.PutString("longitude", e.Longitude.ToString());
            edit.PutString("latitude", e.Latitude.ToString());
            edit.Apply();

            mLastLocation = e.LastLocation;
            HashMap map = new HashMap();
            map.Put("location", location);
            map.Put("ride_id", "waiting");

            rideassigned_listerner = new RideAssignedValueListener(this);
            DriverAvailableRef.AddValueEventListener(rideassigned_listerner);
            DriverAvailableRef.SetValue(map);
            DriverLocationRef = database.GetReference("driversAvailable/" + phone_s + "/location");
        }

        private void HomeFrag_MOnLocationUpdate(object sender, OnLocationUpdatedEventArgs e)
        {
            if (availability)
            {
                HashMap map = new HashMap();
                map.Put("longitude", e.Longitude.ToString());
                map.Put("latitude", e.Latitude.ToString());
                mLastLocation = e.LastLocation;
               if(DriverLocationRef != null)
                {
                    try
                    {
                        DriverLocationRef.SetValue(map);
                    }
                    catch
                    {

                    }
                }
                try
                {
                    edit.PutString("longitude", e.Longitude.ToString());
                    edit.PutString("latitude", e.Latitude.ToString());
                    edit.Apply();
                }
                catch
                {

                }
            }
           
        }
        public void FirebaseGoOffline()
        {
            if(DriverAvailableRef != null)
            {
                DriverAvailableRef.RemoveValueAsync();
                if(rideassigned_listerner != null)
                {
                    DriverAvailableRef.RemoveEventListener(rideassigned_listerner);
                }
            }
           
        }

        public void FirebaseRideMatched(string ride_id)
        {
            RiderRequestRef = database.GetReference("rideRequest/" + ride_id);
            RiderRequestRef.AddListenerForSingleValueEvent(new RideRequestValueListener(this));
        }

        public void FirebaseAccepted()
        {
            DriverRideAccepted = true;
            if (DriverMatched)
            {
                DriverAvailableRef.RemoveValueAsync();
                if(rideassigned_listerner != null)
                {
                    DriverAvailableRef.RemoveEventListener(rideassigned_listerner);
                }
               
                OrderAcceptedRef = database.GetReference("rideRequest/" + rideID + "/driver_id");
                OrderAcceptedRef.SetValue(phone_s);
                FirebaseUpdateWorkLocation();

                if (DriverRideAccepted)
                {

                    OngoingRideRef = database.GetReference("rideCreated/" + rideID );
                    //  OngoingRideRef.AddValueEventListener(new OngoingRideListener(this));
                   
                }
               
            }
        }

        public void FirebaseUpdateWorkLocation()
        {
            DriverWorkingLocationRef = database.GetReference("driversWorking/" + phone_s);
            HashMap map = new HashMap();
            map.Put("longitude", mLastLocation.Longitude.ToString());
            map.Put("latitude", mLastLocation.Latitude.ToString());
            DriverWorkingLocationRef.SetValue(map);
        }
       
        public void FirebaseTripStart()
        {
            HelperFunctions helper = new HelperFunctions();
            //string RideHistoryID = 
        }

        public void FirebaseGetRiderInfo()
        {
            RiderFoundRef = database.GetReference("users/" + riderDetails.rider_phone);
            RiderFoundRef.AddListenerForSingleValueEvent(new RiderFoundValueListener(this));
            background_notified = false;
        }

        public void ShowRide()
        {
            DriverMatched = true;
            if (!DriverRideAccepted)
            {
                if (!isBackground)
                {
                    newridedialogue = new NewRideDialogue(riderDetails.pickup_address, riderDetails.destination_address);
                    var trans = SupportFragmentManager.BeginTransaction();
                    newridedialogue.Cancelable = false;
                    newridedialogue.Show(trans, "NewRide");

                    player = MediaPlayer.Create(Application.Context, Resource.Raw.alert);
                    player.Looping = true;
                    player.Start();

                    newridedialogue.OrderAccepted += Newridedialogue_OrderAccepted;
                    newridedialogue.OrderRejected += Newridedialogue_OrderRejected;
                }
                else
                {
                    DriverMatched = false;

                    //if (!background_notified)
                    //{
                    //    background_notified = true;
                    //    Intent intent = new Intent(this, typeof(NewRideActivity));
                    //    riderDetails.ride_id = rideID;

                    //    string jstr = JsonConvert.SerializeObject(riderDetails);
                    //    intent.PutExtra("ride_data", jstr);
                    //    intent.AddFlags(ActivityFlags.NewTask);
                    //    PendingIntent pendingIntent = null;

                    //    const int pendingIntentId = 0;
                    //    pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);

                    //    var path = Android.Net.Uri.Parse("android.resource://com.kinidriver.ng/" + Resource.Raw.alert);

                    //    var notificationBuilder = new Notification.Builder(this)
                    //        .SetSmallIcon(Resource.Mipmap.ic_directions_car_black_48dp)
                    //        .SetContentTitle("KiniTaxi")
                    //        .SetDefaults(NotificationDefaults.Lights | NotificationDefaults.Vibrate)
                    //        // .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                    //        .SetSound(path)
                    //        .SetPriority((int)NotificationPriority.Max)
                    //        .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.kinid))
                    //        .SetContentText("You have a new ride request")
                    //        .SetAutoCancel(true)
                    //        .SetTicker("You have a new ride request")
                    //         .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis())
                    //        .SetContentIntent(pendingIntent);

                    //    var notificationManager = NotificationManager.FromContext(this);
                    //    notificationManager.Notify(0, notificationBuilder.Build());
                    //}
                   
                   

                }

            }
          
        }

        public void NewRideTimeout( string type)
        {
            if (newridedialogue != null)
            {
                try
                {
                    player.Stop();
                    player.Looping = false;
                    player.Stop();
                    player.Looping = false;
                    player.Dispose();
                    player = null;
                }
                catch
                {
                    if(player != null)
                    {
                        player.Stop();
                        player.Looping = false;
                        player.Stop();
                        player.Dispose(); ;
                        player = null;
                    }
                }
               

                newridedialogue.Dismiss();

                acceptCheck = null;
                acceptCheck = database.GetReference("driversAvailable/" + phone_s + "/ride_id");
                acceptCheck.SetValue("waiting");
                DriverMatched = false;

                if(type == "timeout")
                {
                    Toast.MakeText(this, "Ride request has expired", ToastLength.Short).Show();
                }
                else if(type == "cancelled")
                {
                    Toast.MakeText(this, "Rider has cancelled this request", ToastLength.Short).Show();
                }
            }
        }
        private void Newridedialogue_OrderRejected(object sender, EventArgs e)
        {
            player.Looping = false;
            player.Stop();
            player.Looping = false;
            player.Dispose();
            player = null;

            if (newridedialogue != null)
            {
                newridedialogue.Dismiss();
            }
            DriverMatched = false;
            rejectref = null;
            rejectref = database.GetReference("rideRequest/" + rideID + "/driver_id");
            rejectref.SetValue("order_rejected");

            acceptCheck = null;
            acceptCheck = database.GetReference("driversAvailable/" + phone_s + "/ride_id");
            acceptCheck.SetValue("waiting");
            DriverMatched = false;
            //Perform a REST REQUEST to find another driver for the order;
        }

        private void Newridedialogue_OrderAccepted(object sender, EventArgs e)
        {
            DriverRideAccepted = true;
            player.Looping = false;
            player.Stop();
            player.Looping = false;

            if (DriverMatched)
            {

                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Please wait...");
                progress.SetCancelable(false);
                progress.Show();

                DatabaseReference requestvalidRef = database.GetReference("rideRequest/" + rideID);
                RequestValidityValueEventListener requestvalid_listener = new RequestValidityValueEventListener();
                requestvalidRef.AddListenerForSingleValueEvent(requestvalid_listener);
                requestvalid_listener.IsRequestValid += (i, f) =>
                {
                    progress.Dismiss();
                    if (f.validity)
                    {
                        if(newridedialogue != null)
                        {
                            newridedialogue.Dismiss();
                        }
                        DriverAvailableRef.RemoveValueAsync();
                        DriverAvailableRef.RemoveEventListener(rideassigned_listerner);

                        riderDetails.ride_id = rideID;
                        OrderAcceptedRef = database.GetReference("rideRequest/" + rideID + "/driver_id");
                        OrderAcceptedRef.SetValue(phone_s);

                        DatabaseReference thisride = database.GetReference("drivers/" + phone_s + "/trips/" + rideID);
                        thisride.SetValue(true);

                        DriverWorkingLocationRef = database.GetReference("driversWorking/" + phone_s);
                        HashMap map = new HashMap();
                        map.Put("longitude", mLastLocation.Longitude.ToString());
                        map.Put("latitude", mLastLocation.Latitude.ToString());
                        DriverWorkingLocationRef.SetValue(map);

                        DatabaseReference cLocationRef = database.GetReference("rideCreated/" + rideID + "/driverLocation");
                        cLocationRef.SetValue(map);
                        mSwitch.Toggle();
                        GoOffline();

                        string jstr = JsonConvert.SerializeObject(riderDetails);
                        DatabaseReference ongoing = database.GetReference("drivers/" + phone_s + "/ongoing");
                        ongoing.SetValue(jstr);

                        Intent intent = new Intent(this, typeof(EnrouteActivity));
                        intent.PutExtra("ride", jstr);
                        Finish();
                        StartActivity(intent);
                      
                    }
                    else
                    {
                        AppAlertDialogue appalert = new AppAlertDialogue("This request has been cancelled by rider");
                        appalert.Cancelable = true;
                        var trans = SupportFragmentManager.BeginTransaction();
                        appalert.Show(trans, "appalert");
                        DriverAvailableRef.Child("ride_id").SetValue("waiting");
                        appalert.AlertCancel += (a, s) =>
                        {
                            appalert.Dismiss();
                        };

                        appalert.AlertOk += (d, g) =>
                        {
                            appalert.Dismiss();
                        };
                    }
                };

              

               
            }
        }

       
       
        #endregion



        #region RuntimePermissions

        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                await GetPermissionsAsync();
                return;
            }


        }
        const int RequestLocationId = 0;

        readonly string[] PermissionsGroupLocation =
            {
                             //TODO add more permissions
                            Manifest.Permission.AccessCoarseLocation,
                            Manifest.Permission.AccessFineLocation,
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.Camera,
             };
        async Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.AccessFineLocation;

            if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
            {
                //TODO change the message to show the permissions name
                //  Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                //set alert for executing the task
                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("Permissions Needed");
                alert.SetMessage("The application need special permissions to continue");
                alert.SetPositiveButton("Request Permissions", (senderAlert, args) =>
                {
                    RequestPermissions(PermissionsGroupLocation, RequestLocationId);
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
                });

                Dialog dialog = alert.Create();
                dialog.Show();


                return;
            }

            RequestPermissions(PermissionsGroupLocation, RequestLocationId);

        }
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults[0] == (int)Android.Content.PM.Permission.Granted)
                        {
                            Toast.MakeText(this, "Permissions granted", ToastLength.Short).Show();

                        }
                        else
                        {
                            //Permission Denied :(
                            Toast.MakeText(this, "Permissions denied", ToastLength.Short).Show();

                        }
                    }
                    break;
            }
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion


        #region OTHERS

        protected override void OnStart()
        {
            base.OnStart();
            edit.PutString("appmainstate", "visible");
            edit.Apply();
        }

        protected override void OnStop()
        {
            base.OnStop();

            //if (availability)
            //{
            //    GoOffline();
            //    availability = true;
            //}
            try
            {
                edit.PutString("appmainstate", "hidden");
                edit.Apply();
            }
            catch
            {

            }
           
        }

        protected override void OnResume()
        {
            base.OnResume();
            isBackground = false;
            edit.PutString("appmainstate", "visible");
            edit.Apply();
            //if (availability)
            //{
            //    GoOnline();
            //}
        }

        protected override void OnPause()
        {
            base.OnPause();
            isBackground = true;
            edit.PutString("appmainstate", "hidden");
            edit.Apply();

        }

        private bool CheckPlayServices()
        {
            int resultCode = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(Application.Context);
            if (resultCode != ConnectionResult.Success)
            {
                if (GooglePlayServicesUtil.IsUserRecoverableError(resultCode))
                {
                    GooglePlayServicesUtil.GetErrorDialog(resultCode, this, PLAY_SERVICES_RESOLUTION_REQUEST).Show();
                }
                else
                {
                    Toast.MakeText(Application.Context, "This device is not support Google Play Services", ToastLength.Long).Show();
                    this.Finish();
                }

                return false;
            }
            return true;
        }

        



        #endregion


    }
}