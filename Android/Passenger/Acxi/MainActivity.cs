using Android.App;
using Android.Widget;
using Android.OS;
using SupportWidget = Android.Support.V7.Widget;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Android.Support.V7.App;
using Calligraphy;
using Android.Content;
using Android.Gms.Maps;
using System;
using Android.Views;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Acxi.Activities;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using Android.Locations;
using static Android.Gms.Common.Apis.GoogleApiClient;
using Android.Gms.Common;
using Android.Support.V4.App;
using Android;
using Android.Gms.Maps.Model; 
using Android.Util;
using Android.Graphics;
using Android.Gms.Location.Places.UI;
using Android.Gms.Location.Places;
using Android.Runtime;
using Acxi.Helpers;
using Newtonsoft.Json;
using Com.Google.Maps.Android;
using Java.Util;
using yucee.Helpers;
using Acxi.DataModels;
using System.Threading.Tasks;
using System.IO;
using Android.Content.Res;
using Acxi.DialogueFragment;
using System.Globalization;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Refractored.Controls;
using Firebase.Database;
using Firebase;
using static Android.Support.Design.Widget.BottomSheetBehavior;
using System.Diagnostics;
using System.Threading;
using Plugin.Connectivity;
using System.Collections.Generic;
using Acxi.EventListeners;
using Newtonsoft.Json.Linq;
using Android.Media;

namespace Acxi
{
    [Activity(Label = "Acxi", MainLauncher = false, Theme = "@style/AcxiTheme", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, IOnMapReadyCallback, IConnectionCallbacks, IOnConnectionFailedListener, Android.Gms.Location.ILocationListener, IPlaceSelectionListener
    {
        //BOTTOMSHEET RESOURCES
        private BottomSheetBehavior paymentbottomsheet_behaviour;
        private BottomSheetBehavior requestbottomsheet_behaviour;
        private BottomSheetBehavior driverassignedbottomsheet_behaviour;

        //PAYMENT BOTTOMSHEET RESOURCES
        Button btnproceed_paymentbottom;
        RadioButton radiocash_paymentsheet;
        RadioButton radiocard_paymentsheet;
        RadioButton radiowallet_paymentsheet;

        //REQUEST BOTTOMSHEET RESOURCES
        TextView txtfaresEstimate_requestsheet;
        TextView txtarrival_requestsheet;
        TextView txtpaymentMethod_requestsheet;
        Button btnrequest_requestsheet;

        //DRIVERASSINED BOTTOMSHEET RESOURCES
        TextView txtname_driverassigned;
        TextView txtcarmodel_driverassigned;
        TextView txtplatenumber_driverassigned;
        TextView txtarrival_driverassigned;
        TextView txtprofile_driverassigned;
        public CircleImageView img_driverassigned;
        LinearLayout btncall_driverassigned;
        LinearLayout btndetails_driverassigned;
        LinearLayout btncancel_driverassigned;
        LinearLayout btn_hometerms;

        //SHARED PREFRENCES
        public string phone_s;
        string firstname_s;
        string lastname_s;
        string email_s;
        string photourl_s;
        string firsttime_s;
        string just_registered;
        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        //NAV TITLE
        TextView txtnav_title;
        CircleImageView imgProfile;

        const string TAG = "MainActivity";
        int PLACE_AUTOCOMPLETE_REQUEST_CODE = 1;

        SupportWidget.Toolbar mToolbar;
        DrawerLayout mdrawer;
        TextView txtlocation_main;
        TextView txtdestination_main;
        Button btn_addmoredestination_main, btn_doneonetrip_main;
        ImageView btn_favlocation_main;
        Button btn_favlocation_round;
        public FrameLayout fragamentContainer_main;
        RadioButton radio_location;
        RadioButton radio_destination;
        ImageView imgcenter_marker;
        RelativeLayout btn_mylocation;
        RelativeLayout layout_location_main, layout_destination_main;
        LatLng latlng_pickuplocation, latlng_destination1, latlng_destination2, latlng_destination3;
        string location_address, destination_address1, destination_address2, destination_address3;

        //TRACKING SITUATIONS
        string whichplace_definition = "";
        string whichplace_marker = "location";

        //VARIABLE ENSURES THAT ADDRESS FROM PLACE SDK IS USED, HENCE DISABLE THE EFFECT OF CAMERAMOVE EVENT AND CAMERAIDLE EVENT WHICH REQUESTS FOR A NEW LOCATION;
        bool TakeAddressFromSearch = false;
        public string orderstate = "REST";
        public string tripstate = "";


        //GOOGLE API CLIENT
        private GoogleMap mainMap;
        private const int MY_PERMISSION_REQUEST_CODE = 7171;
        private const int PLAY_SERVICES_RESOLUTION_REQUEST = 7172;
        private bool mRequestingLocationUpdates = true;
        private LocationRequest mLocationRequest;
        private GoogleApiClient mGoogleApiClient;
        private Android.Locations.Location mLastLocation;
        public bool location_displayed = false;

        private static int UPDATE_INTERVAL = 5; //SEC
        private static int FASTEST_INTERVAL = 5; //SEC
        private static int DISPLACEMENT = 2; // METERS
        private Android.Gms.Maps.Model.Polyline mPolyLine;
       
        string DirectionKey = "AIzaSyBGEbUGOVZaP5DLh3UK-cM1kF-bw7e-YMI";
        public RideGeoDetails tripgeodetails;

        Marker locationMarker;
        Marker destinationMarker;
        Marker driverMarker;

        HelperFunctions helper = new HelperFunctions();
        WebRequestHelpers webhelpers = new WebRequestHelpers();

        //FRAGMENTS
        public RequestingDriver_Frag requestingDriver_frag;
        RateTrip_Frag rateTrip_frag;
        RideDetails_Frag rideDetails_frag;
        AppAlertDialogue appalert;
        DriverDetails_Frag driverDetails_frag;
        FaresDialogue faresdialogue_frag;
        WhereTo_Dialogue whereto_frag;

        //FIREBASE INSTANCES
        FirebaseDatabase database;
        DatabaseReference mRef;
        DatabaseReference mRef2;
        public DatabaseReference createRequestRef;
        public DatabaseReference driverRef;
        public DatabaseReference driverFoundRef;
        public DatabaseReference ongoingRideRef;
        public DatabaseReference driverLocationRef;
        public DatabaseReference faresRef;
        public DatabaseReference thisDriver;

        CreateRequestValueListener CreateRequest_listener;
        OngoingRideListener ongoingRide_listener;
        DriverFoundValueListener driverfound_listener;

        double App_basefare = 0;
        double App_timefare = 0;
        double App_distancefare = 0;
        double App_stopfare = 0;
        double stops = 0;
        int App_requesttimeout;
        string App_notification_key = "";
        string force_update = "";
        string App_version;
        string App_registration_bonus;

        double promo_wallet = 0;
        double ride_wallet = 0;

        public List<string> rejectedDrivers = new List<string>();
        public bool locationwatcher = false;
        public string driverFoundId = ""; // DRIVER FOUND AND HAS ACCEPTED THE ORDER;
        public string driverFound_yetToAccept = ""; // DRIVER FOUND BUT HAS NOT ACCEPTED THE ORDER;
        public bool DriverAssigned = false;
        public LatLng DriverLocation;
        public string rideStatus = "";
        public string FirstDuration = "";
        public bool isRequestingUpdate = false;
        public bool driverFoundResponse = false;
        public string driverImageString = "";
        public DriverDetails foundDriverDetails;
        public Stopwatch DurationCount;
        public System.Timers.Timer RideRequestTimer = new System.Timers.Timer();
        int RequestTimeoutCount = 0;

        string preferred_card = "";

        public DatabaseReference driverAvailRef;
        public RequestTimeoutValueEventListener requesttimeout_listener;
        public static Context currentContext;
        int updatelocation_animecount = 0;
        RiderDetails rideNotifyDetails;

        bool token_update = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {

           // AppCenter.Start("722382e0-864c-4c8e-a10b-31a3c619179c", typeof(Analytics), typeof(Crashes));

          //  await TryToGetPermissions();

            base.OnCreate(savedInstanceState);

            //SETTING DEFAULT FONT
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());
            SetContentView(Resource.Layout.Main);

            edit = pref.Edit();
            mToolbar = (SupportWidget.Toolbar)FindViewById(Resource.Id.mainToolbar);
            layout_location_main = (RelativeLayout)FindViewById(Resource.Id.layout_location_main);
            layout_destination_main = (RelativeLayout)FindViewById(Resource.Id.layout_destination_main);
            fragamentContainer_main = (FrameLayout)FindViewById(Resource.Id.fragmentContainer_main);
            btn_hometerms = (LinearLayout)FindViewById(Resource.Id.btnhome_terms);
            btn_hometerms.Click += Btn_hometerms_Click;

            txtlocation_main = (TextView)FindViewById(Resource.Id.txtlocation_main);
            txtdestination_main = (TextView)FindViewById(Resource.Id.txtdestination_main);

            radio_location = (RadioButton)FindViewById(Resource.Id.radio_pickup);
            radio_destination = (RadioButton)FindViewById(Resource.Id.radio_destination);
            imgcenter_marker = (ImageView)FindViewById(Resource.Id.imgcenter_marker);
            imgcenter_marker.Click += Imgcenter_marker_Click;

            radio_location.Click += Radio_location_Click;
            radio_destination.Click += Radio_destination_Click;

            btn_mylocation = (RelativeLayout)FindViewById(Resource.Id.btn_mylocation);
            btn_doneonetrip_main = (Button)FindViewById(Resource.Id.btndone_onetrip_main);
            btn_addmoredestination_main = (Button)FindViewById(Resource.Id.btn_addmoredestination_main);
            btn_addmoredestination_main.Visibility = ViewStates.Invisible;
            btn_addmoredestination_main.Click += Btn_addmoredestination_main_Click;
            btn_mylocation.Click += Btn_mylocation_Click;

            btn_favlocation_main = (ImageView)FindViewById(Resource.Id.btn_favlocation_main);
            btn_favlocation_round = (Button)FindViewById(Resource.Id.btn_favlocation_round);

            btn_favlocation_main.Click += Btn_favlocation_main_Click;
            btn_favlocation_round.Click += Btn_favlocation_round_Click;

            LinearLayout paymentsheetview = (LinearLayout)FindViewById(Resource.Id.paymentbottomsheet_view);
            LinearLayout requestsheetview = (LinearLayout)FindViewById(Resource.Id.requestacxi_bottomsheet_view);
            LinearLayout driverassignedsheetview = (LinearLayout)FindViewById(Resource.Id.driverassigned_bottomsheet_view);

            paymentbottomsheet_behaviour = BottomSheetBehavior.From(paymentsheetview);
            driverassignedbottomsheet_behaviour = BottomSheetBehavior.From(driverassignedsheetview);
            requestbottomsheet_behaviour = BottomSheetBehavior.From(requestsheetview);

            //PAYMENTSHEET RESOURCES
            btnproceed_paymentbottom = (Button)FindViewById(Resource.Id.btnproceed_paymentfrag);
            radiocard_paymentsheet = (RadioButton)FindViewById(Resource.Id.radio_card);
            radiocash_paymentsheet = (RadioButton)FindViewById(Resource.Id.radio_cash);
            radiowallet_paymentsheet = (RadioButton)FindViewById(Resource.Id.radio_wallet);
            btnproceed_paymentbottom.Click += Btnproceed_paymentbottom_Click;
            radiocard_paymentsheet.Click += Radiocard_paymentsheet_Click;
            radiocash_paymentsheet.Click += Radiocash_paymentsheet_Click;
            radiowallet_paymentsheet.Click += Radiowallet_paymentsheet_Click;

            //REQUESTSHEET RESOURCES
            txtfaresEstimate_requestsheet = (TextView)FindViewById(Resource.Id.txt_Estimatedfares_RequestAcxi);
            txtarrival_requestsheet = (TextView)FindViewById(Resource.Id.txt_Arrivaltime_RequestAcxi);
            txtpaymentMethod_requestsheet = (TextView)FindViewById(Resource.Id.txt_Paymentmethod_RequestAcxi);
            btnrequest_requestsheet = (Button)FindViewById(Resource.Id.btn_requestacxi_requestfrag);
            btnrequest_requestsheet.Click += Btnrequest_requestsheet_Click;

            //DRIVERASSIGNEDSHEET RESOURCES
            txtarrival_driverassigned = (TextView)FindViewById(Resource.Id.txtarrival_driverassigned);
            txtcarmodel_driverassigned = (TextView)FindViewById(Resource.Id.txtcardetails_driverassigned);
            txtname_driverassigned = (TextView)FindViewById(Resource.Id.txtfirstname_driverassigned);
            txtplatenumber_driverassigned = (TextView)FindViewById(Resource.Id.txtplatenumber_driverassigned);
            txtprofile_driverassigned = (TextView)FindViewById(Resource.Id.txtprofile_driverassigned);
            btncall_driverassigned = (LinearLayout)FindViewById(Resource.Id.btncall_driverassigned);
            img_driverassigned = (CircleImageView)FindViewById(Resource.Id.img_driverassigned_frag);
            // btnmessage_driverassigned = (LinearLayout)FindViewById(Resource.Id.btnmessage_driverassigned);
            btndetails_driverassigned = (LinearLayout)FindViewById(Resource.Id.btnridedetails_driverassigned);
            btncancel_driverassigned = (LinearLayout)FindViewById(Resource.Id.btncancel_driverassigned);
            btncall_driverassigned.Click += Btncall_driverassigned_Click;
            btncancel_driverassigned.Click += Btncancel_driverassigned_Click;
            btndetails_driverassigned.Click += Btndetails_driverassigned_Click;
            txtprofile_driverassigned.Click += Txtprofile_driverassigned_Click;
            //LAYOUT CLICKABLE
            layout_location_main.Click += Layout_location_main_Click;
            layout_destination_main.Click += Layout_destination_main_Click;
            btn_doneonetrip_main.Click += Btn_doneonetrip_main_Click;



            ////FOR SETTING STATUSBAR TO TRANSPARENT
            //if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            //{
            //    this.Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);
            //    Display display = WindowManager.DefaultDisplay;
            //    Point size = new Point();
            //    display.GetSize(size);
            //    int width = size.X;
            //    int height = size.Y;
            //    Console.WriteLine("Screen height = " + height.ToString());
            //    //CALCULATES THE DEVICE SCREEN SIZE AND ADJUSTS ACTIONBAR ACCORDINGLY
            //    if (height > 1280)
            //    {
            //        mToolbar.SetPadding(0, 25, 0, 0);
            //    }
            //    else
            //    {
            //        mToolbar.SetPadding(0, 45, 0, 0);
            //    }

            //}
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "";
            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_menu_action);
            ab.SetDisplayHomeAsUpEnabled(true);
            SupportMapFragment mapfragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapfragment.GetMapAsync(this);
            NavigationView nav_view = FindViewById<NavigationView>(Resource.Id.nav_view);
            mdrawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            if (nav_view != null)
            {
                SetupDraweContent(nav_view);
                View headerLayout = nav_view.GetHeaderView(0);
                txtnav_title = headerLayout.FindViewById<TextView>(Resource.Id.txtnav_title);
                imgProfile = headerLayout.FindViewById<CircleImageView>(Resource.Id.imgViewHeader);
            }

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] {
                    Manifest.Permission.AccessFineLocation,
                    Manifest.Permission.AccessCoarseLocation
                }, MY_PERMISSION_REQUEST_CODE);
            }
            else
            {
                if (CheckPlayServices())
                {
                    BuildGoogleApiClient();
                    CreateLocationRequest();
                }
            }

            SetSharedPrefrences();
            if (firsttime_s == "true")
            {
                Thread t2 = new Thread(delegate ()
                {
                    downloadImageImage();
                });
                t2.Start();
            }
            else
            {
                SetProfileImage();
            }

            SetUpFirebase();
            FirebaseGetAppSettings();
            TurnOnLOcationService();

            RideRequestTimer.Interval = 1000;
            RideRequestTimer.Elapsed += RideRequestTimeOut_Elapsed;
            currentContext = this;
        }

        private void Btn_hometerms_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com/home/terms");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Btn_mylocation_Click(object sender, EventArgs e)
        {
            DisplayLocation();
        }

        private void Btn_favlocation_round_Click(object sender, EventArgs e)
        {
            btn_favlocation_round.Enabled = false;
            whereto_frag = new WhereTo_Dialogue();
            whereto_frag.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            whereto_frag.Show(trans, "Where to");
            whereto_frag.SelectPlace += Whereto_frag_SelectPlace;
            whereto_frag.CloseDialogue += Whereto_frag_CloseDialogue;
        }

        private void Imgcenter_marker_Click(object sender, EventArgs e)
        {

        }

        private void Btn_addmoredestination_main_Click(object sender, EventArgs e)
        {
            if (appalert != null)
            {
                appalert.Dismiss();
                appalert = null;
            }
            else
            {
                appalert = null;
            }
            appalert = new AppAlertDialogue("You are about to make a stop. Each stop attracts a ₦" + App_stopfare.ToString() + " charge");
            var trans = SupportFragmentManager.BeginTransaction();
            appalert.Cancelable = false;
            appalert.Show(trans, "alert");
            appalert.AlertCancel += (o, p) =>
            {
                appalert.Dismiss();
            };
            appalert.AlertOk += (o, r) =>
            {
                appalert.Dismiss();
                stops += 1;
                database.GetReference("rideCreated/" + tripgeodetails.ride_id + "/stops").SetValue(stops.ToString());
            };

        }

        private void Btn_favlocation_main_Click(object sender, EventArgs e)
        {
            btn_favlocation_main.Enabled = false;
            whereto_frag = new WhereTo_Dialogue();
            whereto_frag.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            whereto_frag.Show(trans, "Where to");
            whereto_frag.SelectPlace += Whereto_frag_SelectPlace;
            whereto_frag.CloseDialogue += Whereto_frag_CloseDialogue;
        }

        private void Whereto_frag_CloseDialogue(object sender, EventArgs e)
        {
            if (whereto_frag != null)
            {
                whereto_frag.Dismiss();
            }
            btn_favlocation_round.Enabled = true;

        }

        private void Whereto_frag_SelectPlace(object sender, OnSavePlaceEventArgs e)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                Radio_destination_Click(sender, e);
                DestinationSelected();
                txtdestination_main.Text = e.address;
                destination_address1 = e.address;
                LatLng where = new LatLng(e.latitude, e.longitude);
                latlng_destination1 = where;
                TakeAddressFromSearch = true;
                mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(where, 15));

            }
            else
            {
                Toast.MakeText(this, "Internet connectivity is not available", ToastLength.Long).Show();
            }

            if (whereto_frag != null)
            {
                whereto_frag.Dismiss();
            }
            btn_favlocation_round.Enabled = true;
        }

        private void Radio_destination_Click(object sender, EventArgs e)
        {
            //TODO FIX OUT OF MEMORY ERROR;
            //DONE
            try
            {
                imgcenter_marker.SetImageResource(Resource.Drawable.marker_red1);
            }
            catch
            {
                imgcenter_marker = null;
                imgcenter_marker = (ImageView)FindViewById(Resource.Id.imgcenter_marker);
                imgcenter_marker.SetImageResource(Resource.Drawable.marker_red1);
            }
            radio_location.Checked = false;
            radio_destination.Checked = true;
            whichplace_marker = "destination";
        }

        private void Radio_location_Click(object sender, EventArgs e)
        {
            radio_destination.Checked = false;
            radio_location.Checked = true;
            try
            {
                imgcenter_marker.SetImageResource(Resource.Drawable.marker_green4);
            }
            catch
            {
                imgcenter_marker = null;
                imgcenter_marker = (ImageView)FindViewById(Resource.Id.imgcenter_marker);
                imgcenter_marker.SetImageResource(Resource.Drawable.marker_green4);
            }
            whichplace_marker = "location";
        }

        public class BottomCallBacksReverse : BottomSheetCallback
        {
            BottomSheetBehavior behaviour;
            public BottomCallBacksReverse(BottomSheetBehavior Behaviour)
            {
                behaviour = Behaviour;
            }
            public override void OnSlide(View bottomSheet, float slideOffset)
            {

            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                if (newState == BottomSheetBehavior.StateDragging)
                {
                    behaviour.State = BottomSheetBehavior.StateDragging;
                }
            }
        }

        public class BottomCallBacks : BottomSheetCallback
        {
            BottomSheetBehavior behaviour;

            public BottomCallBacks(BottomSheetBehavior Behaviour)
            {
                behaviour = Behaviour;
            }
            public override void OnSlide(View bottomSheet, float slideOffset)
            {

            }

            public override void OnStateChanged(View bottomSheet, int newState)
            {
                if (newState == BottomSheetBehavior.StateDragging)
                {
                    behaviour.State = BottomSheetBehavior.StateExpanded;
                }
            }
        }

        private void SetupDraweContent(NavigationView nav_view)
        {
            nav_view.NavigationItemSelected += (sender, e) =>
            {

                if (e.MenuItem.ItemId == Resource.Id.nav_wallet)
                {
                    Intent intent = new Intent(this, typeof(PaymentsActivity));
                    this.StartActivity(intent);
                }
                else if (e.MenuItem.ItemId == Resource.Id.nav_trips)
                {
                    Intent intent = new Intent(this, typeof(TripsActivity));
                    this.StartActivity(intent);
                }
                else if (e.MenuItem.ItemId == Resource.Id.nav_profile)
                {
                    Intent intent = new Intent(this, typeof(ProfileActivity));
                    this.StartActivity(intent);
                }
                else if (e.MenuItem.ItemId == Resource.Id.nav_support)
                {
                    Intent intent = new Intent(this, typeof(HelpActivity));
                    this.StartActivity(intent);
                }
                else if (e.MenuItem.ItemId == Resource.Id.nav_promo)
                {
                    Intent intent = new Intent(this, typeof(PromoActivity));
                    intent.PutExtra("promo", promo_wallet.ToString());
                    this.StartActivity(intent);
                }
                else if (e.MenuItem.ItemId == Resource.Id.nav_about)
                {
                    Intent intent = new Intent(this, typeof(AboutActivity));
                    this.StartActivity(intent);
                }
                mdrawer.CloseDrawers();
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    mdrawer.OpenDrawer((int)GravityFlags.Left);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }

        }



        #region FIREBASE FUNCTIONS

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

        public void UpdateToken()
        {
            string token = pref.GetString("apptoken", "");
            if (!string.IsNullOrEmpty(token))
            {
                DatabaseReference tokenRef = database.GetReference("users/" + phone_s + "/token");
                tokenRef.SetValue(token);
            }
        }

        void TurnOnLOcationService()
        {
            // Get Location Manager and check for GPS & Network location services
            LocationManager lm = (LocationManager)this.GetSystemService(Context.LocationService);
            if (!lm.IsProviderEnabled(LocationManager.GpsProvider) ||
                  !lm.IsProviderEnabled(LocationManager.NetworkProvider))
            {

                appalert = new AppAlertDialogue("Please enable your Location Service");
                appalert.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction(); 
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
            else
            {
                return;
            }

        }

        public void FirebaseCreateRequest()
        {
            createRequestRef = database.GetReference("rideRequest/" + tripgeodetails.ride_id);
            HashMap map = new HashMap();

            rideNotifyDetails = new RiderDetails();
            rideNotifyDetails.destination_address = tripgeodetails.destination1_address;
            rideNotifyDetails.pickup_address = tripgeodetails.pickuplocation_address;
            rideNotifyDetails.ride_id = tripgeodetails.ride_id;
            rideNotifyDetails.rider_phone = phone_s;
            rideNotifyDetails.rider_name = firstname_s;
            rideNotifyDetails.latloc = tripgeodetails.latlng_pickuplocation.Latitude;
            rideNotifyDetails.lngloc = tripgeodetails.latlng_pickuplocation.Longitude;
            rideNotifyDetails.latdes = tripgeodetails.latlng_destination1.Latitude;
            rideNotifyDetails.lngdes = tripgeodetails.latlng_destination1.Longitude;

            HashMap mapdestination = new HashMap();
            mapdestination.Put("longitude", tripgeodetails.latlng_destination1.Longitude.ToString());
            mapdestination.Put("latitude", tripgeodetails.latlng_destination1.Latitude.ToString());

            HashMap maplocation = new HashMap();
            maplocation.Put("longitude", tripgeodetails.latlng_pickuplocation.Longitude.ToString());
            maplocation.Put("latitude", tripgeodetails.latlng_pickuplocation.Latitude.ToString());

            map.Put("destination", mapdestination);
            map.Put("location", maplocation);
            map.Put("destination_address", tripgeodetails.destination1_address);
            map.Put("pickup_address", tripgeodetails.pickuplocation_address);
            map.Put("rider_id", phone_s);
            map.Put("payment_method", tripgeodetails.payment_method);
            map.Put("created_at", helper.GetTimeStampNow().ToString());
            map.Put("driver_id", "waiting");

            CreateRequest_listener = null;
            CreateRequest_listener = new CreateRequestValueListener();
            createRequestRef.AddValueEventListener(CreateRequest_listener);
            createRequestRef.SetValue(map);

            CreateRequest_listener.rideRequest_assigned += CreateRequest_listener_rideRequest_assigned;
            CreateRequest_listener.rideRequest_rejected += CreateRequest_listener_rideRequest_rejected;
            CreateRequest_listener.rideRequest_unfound += CreateRequest_listener_rideRequest_unfound;
        }

        private void CreateRequest_listener_rideRequest_unfound(object sender, EventArgs e)
        {
            FindDriverNearest();
        }

        private void CreateRequest_listener_rideRequest_rejected(object sender, EventArgs e)
        {
            FindDriverAfter_Rejectection();
        }

        private void CreateRequest_listener_rideRequest_assigned(object sender, CreateRequestValueListener.onDriverFoundEventArgs e)
        {
            requestingDriver_frag.Dismiss();
            driverFoundId = e.driver_id;
            FirebaseDriverFound(e.driver_id);
            DriverAssigned = true;
            createRequestRef.RemoveEventListener(CreateRequest_listener);
            createRequestRef.RemoveValue();
            createRequestRef.Dispose();
            createRequestRef = null;
        }


        public void AfterRide()
        {
            ongoingRideRef.RemoveEventListener(ongoingRide_listener);
            CancelAssignedRide_view();
            AfterAssigned();
            btn_addmoredestination_main.Visibility = ViewStates.Invisible;
            try
            {
                img_driverassigned.SetImageResource(Resource.Drawable.account);
            }
            catch
            {

            }

            locationwatcher = false;
            driverFoundId = ""; // DRIVER FOUND AND HAS ACCEPTED THE ORDER;
            driverFound_yetToAccept = ""; // DRIVER FOUND BUT HAS NOT ACCEPTED THE ORDER;
            DriverAssigned = false;
            rideStatus = "";
            FirstDuration = "";
            updatelocation_animecount = 0;

            isRequestingUpdate = false;
            driverFoundResponse = false;
            TakeAddressFromSearch = false;

            radiocard_paymentsheet.Checked = false;
            radiocash_paymentsheet.Checked = false;
            txtarrival_driverassigned.Text = "Coming";
            RecoverTripInvalid();
            edit.Apply();
        }

        public void AfterAssigned()
        {
            DriverAssigned = false;
            driverFoundResponse = false;
        }

        public void FindDriverAfter_Rejectection()
        {
            rejectedDrivers.Add(driverFound_yetToAccept);
            if (createRequestRef != null)
            {
                createRequestRef.Child("driver_id").SetValue("waiting");
            }
            else
            {
                //DatabaseReference currentRequest = database.GetReference("rideRequest/" + tripgeodetails.ride_id + "/driver_id");
            }

            Toast.MakeText(this, "A driver did not accept your request, please wait we are matching you with another driver", ToastLength.Long).Show();
            FindDriverNearest();
        }

        public void FindDriverAfter_Timeout()
        {
            rejectedDrivers.Add(driverFound_yetToAccept);
            if (createRequestRef != null)
            {
                createRequestRef.Child("driver_id").SetValue("waiting");
            }
            else
            {
                // DatabaseReference currentRequest = database.GetReference("rideRequest/" + tripgeodetails.ride_id + "/driver_id");
            }

            Toast.MakeText(this, "A driver did not accept your request in Time, please wait we are matching you with another driver", ToastLength.Long).Show();
            FindDriverNearest();
        }

        public async void FindDriverNearest()
        {

            AssignDriver assign = new AssignDriver();
            assign.ride_id = tripgeodetails.ride_id;
            assign.declined = rejectedDrivers;
            string jstr = JsonConvert.SerializeObject(assign);
            Thread.Sleep(100);
            string response = "";
            await Task.Run(() =>
            {
                response = webhelpers.AssignDriver2(jstr);
                if (!response.Contains("no") && response.Length < 200)
                {
                    string notify_str = JsonConvert.SerializeObject(rideNotifyDetails);
                }
            });

            if (string.IsNullOrEmpty(response) || response.Length > 200)
            {
                DriverNotFoundResponse("Oops!!, something went wrong while finding a driver, Please try again");
                return;
            }

            if (response.Contains("no"))
            {
                Thread.Sleep(100);
                string response1 = "";
                await Task.Run(() =>
                {
                    response1 = webhelpers.AssignDriver2(jstr);

                });

                if (string.IsNullOrEmpty(response1) || response.Length > 200)
                {
                    DriverNotFoundResponse("Oops!!, something went wrong while finding a driver, Please try again");
                    return;
                }

                if (response1.Contains("no"))
                {
                    DriverNotFoundResponse("No available driver within your location, please try again after few moments");
                }
                else
                {
                    driverFound_yetToAccept = response1;
                    GetTokenAndAlertify(response1);
                    GetFoundDriverFirstLocation(response1);
                    thisDriver = database.GetReference("driversAvailable/" + response1 + "/ride_id");
                    thisDriver.SetValue(tripgeodetails.ride_id);
                    driverFoundResponse = true;
                    RideRequestTimer.Enabled = true;

                }
            }
            else
            {
                driverFound_yetToAccept = response;
                GetTokenAndAlertify(response);
                GetFoundDriverFirstLocation(response);
                DatabaseReference thisdriver = database.GetReference("driversAvailable/" + response + "/ride_id");
                thisdriver.SetValue(tripgeodetails.ride_id);
                driverFoundResponse = true;
                RideRequestTimer.Enabled = true;

            }

        }

        private void GetTokenAndAlertify(string response)
        {
            DatabaseReference driverToken = database.GetReference("drivers/" + response + "/token");
            DriverAppTokenEventListener token_listener = new DriverAppTokenEventListener();
            driverToken.AddListenerForSingleValueEvent(token_listener);
            token_listener.OnTokenFetched += Token_listener_OnTokenFetched;
        }

        private async void Token_listener_OnTokenFetched(object sender, DriverAppTokenEventListener.AppTokenEventArgs e)
        {
            if (e.token.Length > 20)
            {
                Task.Run(() =>
                {
                    string ride_data = JsonConvert.SerializeObject(rideNotifyDetails);
                    try
                    {
                        string repo = webhelpers.NotifyDriver(ride_data, e.token, App_notification_key);
                    }
                    catch
                    {

                    }
                });
            }
        }

        public void DriverNotFoundResponse(string body)
        {
            FirebaseCancelRequest();
            if (requestingDriver_frag != null)
            {
                requestingDriver_frag.Dismiss();
            }

            if (appalert != null)
            {
                appalert.Dismiss();
            }

            appalert = new AppAlertDialogue(body);
            var trans = SupportFragmentManager.BeginTransaction();
            appalert.Cancelable = false;
            appalert.Show(trans, "alert");
            appalert.AlertCancel += (o, r) =>
            {
                appalert.Dismiss();
            };

            appalert.AlertOk += (o, w) =>
            {

                appalert.Dismiss();
            };
        }

        private void RideRequestTimeOut_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RequestTimeoutCount++;

            if (RequestTimeoutCount == App_requesttimeout)
            {
                RideRequestTimer.Enabled = false;
                RequestTimeoutCount = 0;

                if (!DriverAssigned)
                {
                    RunOnUiThread(() =>
                    {
                        FindDriverAfter_Timeout();
                        driverAvailRef = database.GetReference("driversAvailable/" + driverFound_yetToAccept + "/ride_id");
                        requesttimeout_listener = new RequestTimeoutValueEventListener(tripgeodetails.ride_id);
                        driverAvailRef.AddListenerForSingleValueEvent(requesttimeout_listener);
                        requesttimeout_listener.isDriverAvailabeValid += Requesttimeout_listener_isDriverAvailabeValid;
                    });

                }
            }

        }

        private void Requesttimeout_listener_isDriverAvailabeValid(object sender, RequestTimeoutValueEventListener.DriverAvailableValidityEventArgs e)
        {
            if (e.valid)
            {
                driverAvailRef.SetValue("timeout");
            }

            Console.WriteLine("the valid");
        }

        private void Requestcancel_listener_isDriverAvailabeValid(object sender, RequestTimeoutValueEventListener.DriverAvailableValidityEventArgs e)
        {
            if (e.valid)
            {
                driverAvailRef.SetValue("cancelled");
            }

            Console.WriteLine("the valid");
        }

        public void GetFoundDriverFirstLocation(string phoneid)
        {
            DatabaseReference locationRef = database.GetReference("driversAvailable/" + phoneid + "/location");
            DriverLocationEventListener driverLocation_listener = new DriverLocationEventListener();
            locationRef.AddListenerForSingleValueEvent(driverLocation_listener);
            driverLocation_listener.OnDriverLocationFound += DriverLocation_listener_OnDriverLocationFound;
        }

        private void DriverLocation_listener_OnDriverLocationFound(object sender, DriverLocationEventListener.DriverLocationEventArgs e)
        {
            DriverLocation = new LatLng(e.lat, e.lng);
        }

        public void FirebaseGetAppSettings()
        {
            DatabaseReference appsettingRef = database.GetReference("appSettings");
            AppSettingsListener appreflistener = new AppSettingsListener();
            appsettingRef.AddValueEventListener(appreflistener);

            appreflistener.BaseFareFound += (o, k) =>
            {
                App_basefare = double.Parse(k.basefare);
                App_distancefare = double.Parse(k.distancefare);
                App_timefare = double.Parse(k.timefare);
                App_stopfare = double.Parse(k.stopfare);
                App_requesttimeout = int.Parse(k.timeout);
                App_notification_key = k.notification_key;
                force_update = k.force_update;
                App_version = k.version;
                App_registration_bonus = k.registration_bonus;
               
                string main_version = PackageManager.GetPackageInfo(PackageName, 0).VersionCode.ToString();

                Console.WriteLine(main_version);
                if (string.IsNullOrEmpty(main_version))
                {
                    edit.PutString("version", App_version);
                    edit.Apply();
                }
                else
                {
                    if (main_version != App_version)
                    {
                        if (double.Parse(App_version) > double.Parse(main_version))
                        {
                            if (force_update == "yes")
                            {
                                string appPackageName = PackageManager.GetPackageInfo(PackageName, 0).PackageName.ToString();

                                orderstate = "UPDATE";
                                AppUpdate_Dialogue appUpdate_dialogue = new AppUpdate_Dialogue();
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
                                    // Toast.MakeText(this, "Update to take place", ToastLength.Short).Show();
                                    edit.PutString("version", App_version);
                                    edit.Apply();
                                    orderstate = "REST";
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

                                // Todo Update
                                return;

                            }
                        }
                    }
                }

                if (just_registered == "true")
                {
                    DatabaseReference promoinfo = database.GetReference("users/" + phone_s + "/wallet/promo_wallet");
                    promoinfo.SetValue(App_registration_bonus);
                    edit.PutString("justregistered", "false");
                    edit.Apply();
                    Console.WriteLine("here");
                }
                else
                {
                    Console.WriteLine("here");
                }
            };

            DatabaseReference profileinfo = database.GetReference("users/" + phone_s);
            ProfileValueEventListener profilelistener = new ProfileValueEventListener();
            profileinfo.AddValueEventListener(profilelistener);

            profilelistener.OnWalletBalance += (o, j) =>
            {
                if (!string.IsNullOrEmpty(j.promo_wallet))
                {
                    promo_wallet = double.Parse(j.promo_wallet);
                }

                if (!string.IsNullOrEmpty(j.ride_wallet))
                {
                    ride_wallet = double.Parse(j.ride_wallet);
                }
            };

           

            profilelistener.SavedPlacesAddress += Profilelistener_SavedPlacesAddress;
            profilelistener.SavedCards += Profilelistener_SavedCards;
            profilelistener.isTripOngoing += Profilelistener_isTripOngoing;
            profilelistener.UserAccountDeleted += Profilelistener_UserAccountDeleted;
            profilelistener.UserAccountVerified += Profilelistener_UserAccountVerified;
        }

        private void Profilelistener_UserAccountVerified(object sender, EventArgs e)
        {
            if (!token_update)
            {
                UpdateToken();
                token_update = true;
            }

        }

        private void Profilelistener_UserAccountDeleted(object sender, EventArgs e)
        {
            Toast.MakeText(this, "This account no longer exist, Please register with acxi", ToastLength.Short).Show();
            edit = pref.Edit();
            edit.Clear().Apply();
            FinishAffinity();
            Intent intent = new Intent(this, typeof(GetStartedActivity));
            StartActivity(intent);
        }

        private void Profilelistener_isTripOngoing(object sender, ProfileValueEventListener.OnTripEventArgs e)
        {
            if (orderstate == "REST")
            {
                if (!string.IsNullOrEmpty(e.ongoing))
                {
                    if (e.ongoing.Length > 10)
                    {
                        RecoverTrip(e.ongoing);
                    }
                }
            }
        }

        private void Profilelistener_SavedCards(object sender, ProfileValueEventListener.CardsEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.card1))
            {
                edit.PutString("card1", e.card1);
            }

            if (!string.IsNullOrEmpty(e.preferred_card))
            {
                edit.PutString("preferred_card", e.preferred_card);
            }

            if (!string.IsNullOrEmpty(e.card2))
            {
                edit.PutString("card2", e.card2);
            }

            if (!string.IsNullOrEmpty(e.card3))
            {
                edit.PutString("card3", e.card3);
            }

            edit.Apply();
        }

        private void Profilelistener_SavedPlacesAddress(object sender, ProfileValueEventListener.AddressesEventArgs e)
        {
            edit.PutString("homeaddress", e.homeaddress);
            edit.PutString("homelat", e.homelat);
            edit.PutString("homelng", e.homelng);
            edit.PutString("workaddress", e.workaddress);
            edit.PutString("worklat", e.worklat);
            edit.PutString("worklng", e.worklng);
            edit.PutString("savedplaces", e.others);
            edit.Apply();

        }

        public void FirebaseCancelRequest()
        {
            driverFoundResponse = false;
            if (createRequestRef != null)
            {
                createRequestRef.RemoveValueAsync();
                if (CreateRequest_listener != null)
                {
                    createRequestRef.RemoveEventListener(CreateRequest_listener);

                    driverAvailRef = null;
                    requesttimeout_listener = null;

                    RideRequestTimer.Enabled = false;
                    RequestTimeoutCount = 0;

                    driverAvailRef = database.GetReference("driversAvailable/" + driverFound_yetToAccept + "/ride_id");
                    requesttimeout_listener = new RequestTimeoutValueEventListener(tripgeodetails.ride_id);
                    driverAvailRef.AddListenerForSingleValueEvent(requesttimeout_listener);
                    requesttimeout_listener.isDriverAvailabeValid += Requestcancel_listener_isDriverAvailabeValid;
                }
            }
        }

        public void FirebaseDriverFound(string driverID)
        {
            driverfound_listener = new DriverFoundValueListener("");
            driverFoundRef = database.GetReference("drivers/" + driverID);
            driverFoundRef.AddListenerForSingleValueEvent(driverfound_listener);
            driverfound_listener.driverDetails_found += Driverfound_listener_driverDetails_found;
        }

        private void Driverfound_listener_driverDetails_found(object sender, DriverFoundValueListener.driverDetailsEventArgs e)
        {
            foundDriverDetails = e.myDriver;
            ShowFoundDriver(e.myDriver);
            FirebaseOngoingRide();
        }

        public void FirebaseOngoingRide()
        {
            if (orderstate != "ONTRIP")
            {
                rejectedDrivers.Clear();
                ongoingRideRef = database.GetReference("rideCreated/" + tripgeodetails.ride_id);
                ongoingRide_listener = new OngoingRideListener();
                ongoingRideRef.AddValueEventListener(ongoingRide_listener);
                ongoingRide_listener.ongoingRide_status += OngoingRide_listener_ongoingRide_status;
                ongoingRide_listener.ongoingRide_ended_fares += OngoingRide_listener_ongoingRide_ended_fares;

                HashMap map = new HashMap();
                HashMap mapdestination = new HashMap();
                mapdestination.Put("longitude", tripgeodetails.latlng_destination1.Longitude.ToString());
                mapdestination.Put("latitude", tripgeodetails.latlng_destination1.Latitude.ToString());

                HashMap maplocation = new HashMap();
                maplocation.Put("longitude", tripgeodetails.latlng_pickuplocation.Longitude.ToString());
                maplocation.Put("latitude", tripgeodetails.latlng_pickuplocation.Latitude.ToString());

                map.Put("destination", mapdestination);
                map.Put("location", maplocation);
                map.Put("destination_address", tripgeodetails.destination1_address);
                map.Put("pickup_address", tripgeodetails.pickuplocation_address);
                map.Put("rider_id", phone_s);
                map.Put("driver_id", driverFoundId);
                map.Put("payment_method", tripgeodetails.payment_method);
                map.Put("created_at", helper.GetTimeStampNow().ToString());
                map.Put("status", "accepted");
                RideSerializable rideInfo = new RideSerializable
                {
                    destination1_address = tripgeodetails.destination1_address,
                    distance = tripgeodetails.distance,
                    distancevalue = tripgeodetails.distancevalue,
                    duration = tripgeodetails.duration,
                    durationvalue = tripgeodetails.durationvalue,
                    estimatefare = tripgeodetails.estimatefare,
                    latdes = tripgeodetails.latlng_destination1.Latitude.ToString(),
                    lngdes = tripgeodetails.latlng_destination1.Longitude.ToString(),
                    latloc = tripgeodetails.latlng_pickuplocation.Latitude.ToString(),
                    lngloc = tripgeodetails.latlng_pickuplocation.Longitude.ToString(),
                    payment_method = tripgeodetails.payment_method,
                    pickuplocation_address = tripgeodetails.pickuplocation_address,
                    ride_id = tripgeodetails.ride_id,
                    timestamp = helper.GetTimeStampNow(),
                    driver_id = driverFoundId,
                };
                string ride_json = JsonConvert.SerializeObject(rideInfo);
                edit.PutString("ongoing", ride_json);
                edit.Apply();
                ongoingRideRef.SetValue(map);
                DatabaseReference thisrideref = database.GetReference("users/" + phone_s + "/rides/" + tripgeodetails.ride_id);
                thisrideref.SetValue(true);

                DatabaseReference onTripRef = database.GetReference("users/" + phone_s + "/ongoing");
                onTripRef.SetValue(ride_json);
            }

        }

        private void OngoingRide_listener_ongoingRide_ended_fares(object sender, OngoingRideListener.rideFaresEventArgs e)
        {
            if (e.status == "ended")
            {
                TripEnded(e.total_fare, e.basefare, e.time_fare, e.distance_fare, e.stop_fare);
            }
        }

        private void OngoingRide_listener_ongoingRide_status(object sender, OngoingRideListener.rideStatusEventArgs e)
        {
            FirebaseDriverStatus(e.status, e.lat, e.lng);
        }

        public void FirebaseDriverStatus(string status, double lat, double lng)
        {

            if (status == "accepted")
            {
                if (lat != 0 && lng != 0)
                {
                    if (orderstate != "ONTRIP")
                    {
                        UpdateDriverLocation(lat, lng);
                    }
                }
            }

            if (status == "arrived")
            {
                TripDriverArrived();
            }
            else if (status == "ongoing")
            {
                txtarrival_driverassigned.Text = "On Trip";
                if (orderstate != "ONTRIP")
                {
                    TripStart();
                }
            }
            else if (status == "ended")
            {
                //TODO - Put a caveat to execute only one

                InitializeTripEnd();
            }
            else if (status == "cancelled_d")
            {

                CancelAssignedRide_view();
                AfterAssigned();
                AppAlertDialogue ap = new AppAlertDialogue("This ride has been cancelled by the driver");
                var trans = SupportFragmentManager.BeginTransaction();
                ap.Show(trans, "alert");
                ap.AlertOk += (o, y) =>
                {
                    ap.Dismiss();
                };
                ap.AlertCancel += (o, m) =>
                {
                    ap.Dismiss();
                };
            }
            else if (status == "cancelled_p")
            {
                AfterRide();
                CancelAssignedRide_view();
                AfterAssigned();
            }
        }

        public void RecoverTrip(string ride_json)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);

            if (!string.IsNullOrEmpty(ride_json))
            {
                var ride_details = JsonConvert.DeserializeObject<RideSerializable>(ride_json);
                driverFoundId = ride_details.driver_id;
                // progress.Show();
                string jstr = pref.GetString("last_driver", "");
                if (!string.IsNullOrEmpty(jstr))
                {
                    // progress.Dismiss();
                    var driver = JsonConvert.DeserializeObject<DriverDetails>(jstr);
                    foundDriverDetails = driver;
                    DatabaseReference driverRef = database.GetReference("drivers/" + driverFoundId);
                    DriverFoundValueListener mylistener = new DriverFoundValueListener("ongoing");
                    driverRef.AddListenerForSingleValueEvent(mylistener);
                    mylistener.isDriverOnTrip += (p, g) =>
                    {
                        if (!string.IsNullOrEmpty(g.ongoing))
                        {

                            if (g.ongoing == ride_details.ride_id)
                            {
                                appalert = new AppAlertDialogue("Continue your ride to " + ride_details.destination1_address.ToUpper());
                                appalert.Cancelable = false;
                                var trans = SupportFragmentManager.BeginTransaction();
                                try
                                {

                                    appalert.Show(trans, "alert");
                                }
                                catch
                                {

                                }

                                appalert.AlertOk += (w, q) =>
                                {
                                    appalert.Dismiss();
                                    InvalidateCenterMarker();
                                    DestinationSelected();

                                    LatLng pickup = new LatLng(double.Parse(ride_details.latloc), double.Parse(ride_details.lngdes));
                                    LatLng destin = new LatLng(double.Parse(ride_details.latdes), double.Parse(ride_details.lngdes));
                                    tripgeodetails = new RideGeoDetails
                                    {
                                        destination1_address = ride_details.destination1_address,
                                        distance = ride_details.distance,
                                        distancevalue = ride_details.distancevalue,
                                        estimatefare = ride_details.estimatefare,
                                        duration = ride_details.duration,
                                        durationvalue = ride_details.durationvalue,
                                        latlng_pickuplocation = pickup,
                                        latlng_destination1 = destin,
                                        payment_method = ride_details.payment_method,
                                        ride_id = ride_details.ride_id,
                                        timestamp = ride_details.timestamp,
                                        pickuplocation_address = ride_details.pickuplocation_address,
                                    };
                                    ShowFoundDriver(driver);
                                    FirebaseOngoingRide();
                                    txtarrival_driverassigned.Text = "On Trip";
                                    txtlocation_main.Text = ride_details.pickuplocation_address;
                                    txtdestination_main.Text = ride_details.destination1_address;
                                    if (orderstate != "ONTRIP")
                                    {
                                        TripStart();
                                    }
                                };

                                appalert.AlertCancel += (t, y) =>
                                {
                                    appalert.Dismiss();
                                    RecoverTripInvalid();
                                };
                            }
                            else
                            {
                                RecoverTripInvalid();
                            }

                        }
                        else
                        {
                            RecoverTripInvalid();
                        }
                    };
                }
            }
        }

        public void RecoverTripInvalid()
        {

            DatabaseReference recoverRef = database.GetReference("users/" + phone_s + "/ongoing");
            recoverRef.RemoveValue();
            edit.PutString("ongoing", "");
            edit.PutString("last_driver", "");
            edit.Apply();

        }

        public void FirebaseWatchDriverLocation()
        {
            driverLocationRef = database.GetReference("driversWorking/" + driverFoundId);
        }


        async Task UpdateDriverLocation(double lat, double lng)
        {
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
            LatLng driverposition = new LatLng(lat, lng);
            LatLng myposition = new LatLng(tripgeodetails.latlng_pickuplocation.Latitude, tripgeodetails.latlng_pickuplocation.Longitude);
            MapFunctionHelper mapfunction = new MapFunctionHelper();

            string durl = "";
            string json = "";


            durl = mapfunction.getMapsApiDirectionsUrl(driverposition, myposition, DirectionKey);

            if (orderstate == "ONTRIP")
            {
                return;
            }

            await Task.Run(() =>
            {
                json = webhelpers.GetDirectionJson(durl);
            });

            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            DirectionParser deser = null;
            try
            {
                deser = JsonConvert.DeserializeObject<DirectionParser>(json);
            }
            catch
            {
                return;
            }
            string duration = deser.routes[0].legs[0].duration.text;
            txtarrival_driverassigned.Text = duration;
            var pointcode = deser.routes[0].overview_polyline.points;
            var line = PolyUtil.Decode(pointcode);
            ArrayList routeList = new ArrayList();
            foreach (LatLng item in line)
            {
                routeList.Add(item);
            }

            try
            {
                mPolyLine.Remove();
            }
            catch
            {

            }

            if (orderstate == "ONTRIP")
            {
                return;
            }

            mainMap.Clear();

            PolylineOptions pointConnect = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(6)
                .InvokeColor(Color.DarkGray)
                .Geodesic(true);

            mPolyLine = mainMap.AddPolyline(pointConnect);
            double southlng = deser.routes[0].bounds.southwest.lng;
            double southlat = deser.routes[0].bounds.southwest.lat;
            double northlat = deser.routes[0].bounds.northeast.lat;
            double northlng = deser.routes[0].bounds.northeast.lng;
            LatLng southwest = new LatLng(southlat, southlng);
            LatLng northeast = new LatLng(northlat, northlng);
            LatLngBounds tripbound = new LatLngBounds(southwest, northeast);

            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count - 1];

            MarkerOptions markerOptions1 = new MarkerOptions();
            markerOptions1.SetPosition(firstpoint);
            markerOptions1.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.car));
            markerOptions1.Anchor(0.5f, 0.5f);
            markerOptions1.SetRotation(helper.bearingBetweenLocations(firstpoint, lastpoint));

            MarkerOptions markerOptions2 = new MarkerOptions();
            markerOptions2.SetPosition(lastpoint);
            markerOptions2.SetTitle("Pickup Location");
            markerOptions2.SetSnippet("Driver is " + duration + " Away");
            markerOptions2.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));

            mainMap.AddMarker(markerOptions1);
            Marker pickupstatus_marker = mainMap.AddMarker(markerOptions2);
            pickupstatus_marker.ShowInfoWindow();

            if (updatelocation_animecount == 0)
            {
                mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(tripbound, 150));
            }
            else
            {
                mainMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(tripbound, 150));
            }
            updatelocation_animecount += 1;
        }



        public void TripDriverArrived()
        {
            //trigger alert
            txtarrival_driverassigned.Text = "Arrived";
            MediaPlayer player = MediaPlayer.Create(Application.Context, Resource.Raw.alert);
            player.Start();

            mainMap.Clear();
            MarkerOptions markerOptions2 = new MarkerOptions();
            markerOptions2.SetPosition(tripgeodetails.latlng_pickuplocation);
            markerOptions2.SetTitle("Pickup Location");
            markerOptions2.SetSnippet("Your driver has arrived");
            markerOptions2.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
            Marker pickupstatus_marker = mainMap.AddMarker(markerOptions2);
            pickupstatus_marker.ShowInfoWindow();
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(tripgeodetails.latlng_pickuplocation, 16));

        }

        public void TripStart()
        {
            //show stop sign,
            // trigger alert,
            //send notification,

            // TRIGGERS ONCE WITH AND SETS tripstate TO STARTED;
            btn_addmoredestination_main.Visibility = ViewStates.Visible;
            orderstate = "ONTRIP";
            mainMap.Clear();
            LatLng destination = new LatLng(tripgeodetails.latlng_destination1.Latitude, tripgeodetails.latlng_destination1.Longitude);
            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(destination);
            markerOptions.SetTitle("Destination");
            markerOptions.SetSnippet(FirstDuration);
            markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
            destinationMarker = mainMap.AddMarker(markerOptions);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(destination, 16));

            StartLocationUpdates();
            destinationMarker.ShowInfoWindow();
            //DurationCount = new Stopwatch();
            //DurationCount.Start();
        }

        public async void UpdateOnTrip()
        {

            LatLng destination = new LatLng(tripgeodetails.latlng_destination1.Latitude, tripgeodetails.latlng_destination1.Longitude);
            LatLng myposition = null;

            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
            myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
            MapFunctionHelper mapfunction = new MapFunctionHelper();
            WebRequestHelpers webrequests = new WebRequestHelpers();

            string durl = "";
            string json = "";
            if (mLastLocation != null)
            {
                durl = mapfunction.getMapsApiDirectionsUrl(myposition, destination, DirectionKey);
            }

            if (!isRequestingUpdate)
            {
                isRequestingUpdate = true;

                await Task.Run(() =>
                {
                    json = webrequests.GetDirectionJson(durl);
                });

                if (string.IsNullOrEmpty(json))
                {
                    return;
                }

                DirectionParser deser = null;
                try
                {
                    deser = JsonConvert.DeserializeObject<DirectionParser>(json);
                }
                catch
                {
                    return;
                }
                string duration = deser.routes[0].legs[0].duration.text;

                if (mLastLocation != null)
                {
                    mainMap.Clear();
                    //Toast.MakeText(this, "location contained = " + duration, ToastLength.Short).Show();
                    myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
                    MarkerOptions markerOptions = new MarkerOptions();
                    markerOptions.SetPosition(destination);
                    markerOptions.SetTitle("Destination");
                    markerOptions.SetSnippet(duration);
                    markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
                    Marker mmaker = mainMap.AddMarker(markerOptions);
                    mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(destination, 16));
                    mmaker.ShowInfoWindow();
                    isRequestingUpdate = false;
                }
            }
            else
            {
                // Toast.MakeText(this, "Is Request Ongoing", ToastLength.Short).Show();
            }
        }

        public void InitializeTripEnd()
        {
            //faresRef = database.GetReference("rideCreated/" + tripgeodetails.ride_id);
            //FaresValueEventListener fares_listener = new FaresValueEventListener(this);
            //faresRef.AddValueEventListener(fares_listener);
        }

        public void TripEnded(string total, string basefare, string timefare, string distancefares, string stopfare)
        {
            // Toast.MakeText(this, "Time Count = " + (DurationCount.ElapsedMilliseconds / 60000).ToString(), ToastLength.Long).Show();
            AfterRide();
            total = (Math.Floor(double.Parse(total) / 10) * 10).ToString();
            faresdialogue_frag = new FaresDialogue(total, basefare, distancefares, timefare, stopfare, tripgeodetails.payment_method);
            if (tripgeodetails.payment_method == "card")
            {
                PerformTripCharge(total);

                rateTrip_frag = new RateTrip_Frag(driverImageString, foundDriverDetails.firstname, foundDriverDetails.lastname);
                rateTrip_frag.Cancelable = false;
                var trans1 = SupportFragmentManager.BeginTransaction();
                rateTrip_frag.Show(trans1, "ratetrip");
                rateTrip_frag.OnRateDone += RateTrip_frag_OnRateDone;
                driverImageString = "";
                mainMap.Clear();
                DisplayLocation();
            }
            else
            {
                faresdialogue_frag.PaymentClicked += Faresdialogue_frag_PaymentClicked;
                var trans = SupportFragmentManager.BeginTransaction();
                faresdialogue_frag.Cancelable = false;
                faresdialogue_frag.Show(trans, "fares");
            }



        }

        public async void PerformTripCharge(string amount)
        {
            string cardjson = "";
            if (!string.IsNullOrEmpty(preferred_card))
            {

                if (preferred_card == "card1")
                {
                    cardjson = pref.GetString("card1", "");
                }
                else if (preferred_card == "card2")
                {
                    cardjson = pref.GetString("card2", "");
                }
                else if (preferred_card == "card3")
                {
                    cardjson = pref.GetString("card3", "");
                }


            }
            else
            {
                string card1 = pref.GetString("card1", "");
                string card2 = pref.GetString("card2", "");
                string card3 = pref.GetString("card3", "");

                if (!string.IsNullOrEmpty(card1))
                {
                    cardjson = card1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(card2))
                    {
                        cardjson = card2;
                    }
                    else
                    {
                        cardjson = card3;
                    }
                }
            }

            if (!string.IsNullOrEmpty(cardjson))
            {
                var jobj = JObject.Parse(cardjson);
                string auth_code = jobj["auth_code"].ToString();
                string pin = jobj["pin"].ToString();
                string email = email_s;
                string ride_id = tripgeodetails.ride_id;
                string charge_response = "";
                double newamount = 0;

                if (promo_wallet > 1 && promo_wallet > double.Parse(amount))
                {
                    promo_wallet -= double.Parse(amount);
                    DatabaseReference promoref = database.GetReference("users/" + phone_s + "/wallet/promo_wallet");
                    promoref.SetValue(promo_wallet.ToString());
                }
                else if (promo_wallet > 1 && promo_wallet < double.Parse(amount))
                {
                    newamount = double.Parse(amount) - promo_wallet;
                    DatabaseReference promoref = database.GetReference("users/" + phone_s + "/wallet/promo_wallet");
                    promoref.SetValue("0");

                    await Task.Run(() =>
                    {

                        charge_response = webhelpers.BillCard(email, pin, amount, ride_id, auth_code);
                        Console.WriteLine("card_charge = :" + charge_response);
                    });


                    if (charge_response.Contains("success"))
                    {
                        DatabaseReference ridewalletRef = database.GetReference("users/" + phone_s + "/wallet/ride_wallet");
                        ridewalletRef.SetValue("0");
                    }
                }
                else
                {
                    await Task.Run(() =>
                    {
                        charge_response = webhelpers.BillCard(email, pin, amount, ride_id, auth_code);
                        Console.WriteLine("card_charge = :" + charge_response);
                    });

                    if (charge_response.Contains("success"))
                    {
                        DatabaseReference ridewalletRef = database.GetReference("users/" + phone_s + "/wallet/ride_wallet");
                        ridewalletRef.SetValue("0");
                    }
                }


            }
        }

        public void PerformTripCharge1(string amount)
        {
            string cardjson = "";
            if (!string.IsNullOrEmpty(preferred_card))
            {

                if (preferred_card == "card1")
                {
                    cardjson = pref.GetString("card1", "");
                }
                else if (preferred_card == "card2")
                {
                    cardjson = pref.GetString("card2", "");
                }
                else if (preferred_card == "card3")
                {
                    cardjson = pref.GetString("card3", "");
                }


            }
            else
            {
                string card1 = pref.GetString("card1", "");
                string card2 = pref.GetString("card2", "");
                string card3 = pref.GetString("card3", "");

                if (!string.IsNullOrEmpty(card1))
                {
                    cardjson = card1;
                }
                else
                {
                    if (!string.IsNullOrEmpty(card2))
                    {
                        cardjson = card2;
                    }
                    else
                    {
                        cardjson = card3;
                    }
                }
            }

            if (!string.IsNullOrEmpty(cardjson))
            {
                var jobj = JObject.Parse(cardjson);
                string auth_code = jobj["auth_code"].ToString();
                string pin = jobj["pin"].ToString();
                string email = email_s;
                string ride_id = tripgeodetails.ride_id;
                string charge_response = "";
                double newamount = 0;

                if (promo_wallet > 1 && promo_wallet > double.Parse(amount))
                {
                    promo_wallet -= double.Parse(amount);
                    DatabaseReference promoref = database.GetReference("users/" + phone_s + "/wallet/promo_wallet");
                    promoref.SetValue(promo_wallet.ToString());
                }
                else if (promo_wallet > 1 && promo_wallet < double.Parse(amount))
                {
                    newamount = double.Parse(amount) - promo_wallet;
                    DatabaseReference promoref = database.GetReference("users/" + phone_s + "/wallet/promo_wallet");
                    promoref.SetValue("0");


                    charge_response = webhelpers.BillCard(email, pin, amount, ride_id, auth_code);
                    Console.WriteLine("card_charge = :" + charge_response);

                    if (string.IsNullOrEmpty(charge_response) || charge_response.Length > 100)
                    {
                        return;
                    }

                    if (charge_response.Contains("success"))
                    {
                        DatabaseReference ridewalletRef = database.GetReference("users/" + phone_s + "/wallet/ride_wallet");
                        ridewalletRef.SetValue("0");
                    }
                }
                else
                {

                    charge_response = webhelpers.BillCard(email, pin, amount, ride_id, auth_code);
                    Console.WriteLine("card_charge = :" + charge_response);
                    if (string.IsNullOrEmpty(charge_response) || charge_response.Length > 100)
                    {
                        return;
                    }

                    if (charge_response.Contains("success"))
                    {
                        DatabaseReference ridewalletRef = database.GetReference("users/" + phone_s + "/wallet/ride_wallet");
                        ridewalletRef.SetValue("0");
                    }
                }


            }
        }


        private void Faresdialogue_frag_PaymentClicked(object sender, EventArgs e)
        {

            faresdialogue_frag.Dismiss();
            if (tripgeodetails.payment_method == "cash")
            {
                rateTrip_frag = new RateTrip_Frag(driverImageString, foundDriverDetails.firstname, foundDriverDetails.lastname);
                rateTrip_frag.Cancelable = false;
                var trans1 = SupportFragmentManager.BeginTransaction();
                rateTrip_frag.Show(trans1, "ratetrip");
                rateTrip_frag.OnRateDone += RateTrip_frag_OnRateDone;
            }
            else if (tripgeodetails.payment_method == "card")
            {

                //INITIALIZE CARD CHARGE,

                rateTrip_frag = new RateTrip_Frag(driverImageString, foundDriverDetails.firstname, foundDriverDetails.lastname);
                rateTrip_frag.Cancelable = false;
                rateTrip_frag.OnRateDone += RateTrip_frag_OnRateDone;
                var trans1 = SupportFragmentManager.BeginTransaction();
                rateTrip_frag.Show(trans1, "ratetrip");
            }
            driverImageString = "";
            mainMap.Clear();
            DisplayLocation();
        }

        private void RateTrip_frag_OnRateDone(object sender, RatingEventArgs e)
        {
            DatabaseReference ratingref = database.GetReference("rideCreated/" + tripgeodetails.ride_id + "/rating");
            ratingref.SetValue(e.rating);

            DatabaseReference feedbackRef = database.GetReference("rideCreated/" + tripgeodetails.ride_id + "/feedback");
            feedbackRef.SetValue(e.feedback);

            //if (ongoingRideRef != null)
            //{
            //    ongoingRideRef.RemoveEventListener(ongoingRide_listener);
            //    ongoingRideRef.OnDisconnect();
            //    ongoingRideRef.Dispose();
            //}     
            rateTrip_frag.Dismiss();
            mainMap.Clear();
            DisplayLocation();
            orderstate = "REST";
        }



        public void CancelAssignedRide_view()
        {
            if (ongoingRide_listener != null)
            {
                if (ongoingRideRef != null)
                {
                    ongoingRideRef.RemoveEventListener(ongoingRide_listener);
                    ongoingRideRef = null;
                }
                ongoingRide_listener = null;
            }

            driverassignedbottomsheet_behaviour.State = BottomSheetBehavior.StateHidden;
            ReverseSelected();
        }




        #endregion


        #region CLICK EVENTS

        private void Txtprofile_driverassigned_Click(object sender, EventArgs e)
        {
            if (foundDriverDetails != null)
            {
                driverDetails_frag = new DriverDetails_Frag(driverImageString, foundDriverDetails);
                var trans = SupportFragmentManager.BeginTransaction();
                driverDetails_frag.Show(trans, "driverdetails");
            }
            else
            {
                Toast.MakeText(this, "Downloading profile, please wait", ToastLength.Short).Show();
            }
        }

        private void Btndetails_driverassigned_Click(object sender, EventArgs e)
        {
            //DriverDetails is assigned when all driver profile properties is downloaded as ride is created

            rideDetails_frag = new RideDetails_Frag(tripgeodetails);
            var trans = SupportFragmentManager.BeginTransaction();
            rideDetails_frag.Show(trans, "ridedetails");
        }

        private void Btncancel_driverassigned_Click(object sender, EventArgs e)
        {
            if (orderstate == "ONTRIP")
            {
                appalert = new AppAlertDialogue("Please inform your driver to drop you off");
                appalert.Cancelable = false;
                var trans1 = SupportFragmentManager.BeginTransaction();
                appalert.Show(trans1, "alert");
                appalert.AlertCancel += (o, p) =>
                {
                    appalert.Dismiss();
                };

                appalert.AlertOk += (o, i) =>
                {
                    appalert.Dismiss();
                };

            }
            else
            {
                appalert = new AppAlertDialogue("Are you sure");
                appalert.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction();
                appalert.Show(trans, "alert");
                appalert.AlertOk += (o, w) =>
                {
                    appalert.Dismiss();

                    ongoingRideRef.Child("status").SetValue("cancelled_p");
                    ongoingRideRef.RemoveEventListener(ongoingRide_listener);
                    ongoingRideRef.Dispose();
                    ongoingRideRef = null;
                    RecoverTripInvalid();

                    //PERFORM BASE CHARGE TODO
                    //Done
                    PerformTripCharge(App_basefare.ToString());
                    CancelAssignedRide_view();
                    orderstate = "REST";
                };

                appalert.AlertCancel += (o, r) =>
                {
                    appalert.Dismiss();
                };
            }

        }

        private void Btncall_driverassigned_Click(object sender, EventArgs e)
        {
            string phonenumber = "0" + driverFoundId.Substring(3);
            var uri = Android.Net.Uri.Parse("tel:" + phonenumber);
            var intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }

        private void Radiowallet_paymentsheet_Click(object sender, EventArgs e)
        {
            if (radiocash_paymentsheet.Checked)
            {
                radiocash_paymentsheet.Checked = false;
            }
            else if (radiocard_paymentsheet.Checked)
            {
                radiocard_paymentsheet.Checked = false;
            }
            tripgeodetails.payment_method = "wallet";
        }

        private void Radiocash_paymentsheet_Click(object sender, EventArgs e)
        {
            if (radiocard_paymentsheet.Checked)
            {
                radiocard_paymentsheet.Checked = false;
            }
            else if (radiowallet_paymentsheet.Checked)
            {
                radiowallet_paymentsheet.Checked = false;
            }
            tripgeodetails.payment_method = "cash";
        }

        private void Radiocard_paymentsheet_Click(object sender, EventArgs e)
        {
            preferred_card = helper.PreferedCard();
            if (!string.IsNullOrEmpty(preferred_card))
            {
                if (radiocash_paymentsheet.Checked)
                {
                    radiocash_paymentsheet.Checked = false;
                }

                else if (radiowallet_paymentsheet.Checked)
                {
                    radiowallet_paymentsheet.Checked = false;
                }

                tripgeodetails.payment_method = "card";
            }
            else
            {
                radiocard_paymentsheet.Checked = false;
                StartActivity(new Intent(Application.Context, typeof(PaymentsActivity)));
            }

        }

        private void Btnrequest_requestsheet_Click(object sender, EventArgs e)
        {
            var trans = SupportFragmentManager.BeginTransaction();
            var estfare = helper.mEstimateFares(tripgeodetails.distancevalue, tripgeodetails.durationvalue, App_basefare, App_distancefare, App_timefare);
            tripgeodetails.estimatefare = estfare;

            if (ride_wallet > 0)
            {
                DeptDialogue dbt = new DeptDialogue(ride_wallet);
                var trans1 = SupportFragmentManager.BeginTransaction();
                dbt.Show(trans1, "debt");
                dbt.OnDebtDeclined += Dbt_OnDebtDeclined;
            }
            else
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    requestingDriver_frag = null;
                    requestingDriver_frag = new RequestingDriver_Frag(helper.mEstimateFares(tripgeodetails.distancevalue, tripgeodetails.durationvalue, App_basefare, App_distancefare, App_timefare));
                    requestingDriver_frag.Cancelable = false;
                    requestingDriver_frag.Show(trans, "Requesting");
                    FirebaseCreateRequest();
                    requestingDriver_frag.OnRideRequestCancelled += RequestingDriver_frag_OnRideRequestCancelled;

                }
                else
                {
                    if (appalert != null)
                    {
                        appalert.Dismiss();
                        appalert = null;
                    }

                    appalert = new AppAlertDialogue("Internet connectivity is not available");
                    appalert.Cancelable = true;
                    var trans2 = SupportFragmentManager.BeginTransaction();
                    appalert.Show(trans2, "appalert");

                    appalert.AlertCancel += (o, u) =>
                    {
                        appalert.Dismiss();
                        appalert = null;
                    };

                    appalert.AlertOk += (i, g) =>
                    {
                        appalert.Dismiss();
                        appalert = null;
                    };
                }

            }

        }

        private async void Dbt_OnDebtDeclined(object sender, EventArgs e)
        {
            //Initialize payment;
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetCancelable(false);
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait.....");
            progress.Show();
            await Task.Run(() =>
            {
                PerformTripCharge1(ride_wallet.ToString());
            });
            progress.Dismiss();
        }

        private void RequestingDriver_frag_OnRideRequestCancelled(object sender, EventArgs e)
        {
            FirebaseCancelRequest();
            if (requestingDriver_frag != null)
            {
                requestingDriver_frag.Dismiss();
            }
        }

        private async void Btnproceed_paymentbottom_Click(object sender, EventArgs e)
        {
            if (radiocard_paymentsheet.Checked || radiocash_paymentsheet.Checked)
            {
                layout_destination_main.Clickable = false;
                layout_location_main.Clickable = false;


                btnproceed_paymentbottom.Text = "Please wait";
                btnproceed_paymentbottom.Enabled = false;

                MapFunctionHelper mapfunction = new MapFunctionHelper();
                WebRequestHelpers webrequests = new WebRequestHelpers();

                if (latlng_destination1 == null)
                {
                    Toast.MakeText(this, "Please set your Destination", ToastLength.Short).Show();
                    return;
                }

                if (latlng_pickuplocation == null)
                {
                    Toast.MakeText(this, "Please set your Pick-up location", ToastLength.Short).Show();
                    return;
                }

                string durl = mapfunction.getMapsApiDirectionsUrl(latlng_pickuplocation, latlng_destination1, DirectionKey);
                string json = "";

                try
                {
                    await Task.Run(() =>
                    {
                        json = webrequests.GetDirectionJson(durl);

                    });

                    if (string.IsNullOrEmpty(json))
                    {
                        Toast.MakeText(this, "Something went wrong, please try again", ToastLength.Short).Show();
                        btnproceed_paymentbottom.Enabled = true;
                        btnproceed_paymentbottom.Text = "Proceed";
                        tripgeodetails.payment_method = "card";
                        return;
                    }
                    InvalidateCenterMarker();
                    PerformOneTrip(json);

                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Something went wrong, please try again", ToastLength.Short).Show();
                    btnproceed_paymentbottom.Enabled = true;
                    btnproceed_paymentbottom.Text = "Proceed";
                    tripgeodetails.payment_method = "card";
                    return;
                }

                ShowRequestAcxi();
                btnproceed_paymentbottom.Enabled = true;
                btnproceed_paymentbottom.Text = "Proceed";
            }
            else
            {
                Toast.MakeText(this, "Please select a payment method", ToastLength.Long).Show();
            }

        }

        private void Btn_doneonetrip_main_Click(object sender, EventArgs e)
        {
            // string resop = webhelpers.NotifyDriver("You have a new ride request api", "eZYMifI_gcQ:APA91bF4cxTL1SoGqwwUgGUf34NdinBkMSG6b6A2fuOls_L2FkNlLVeuE31HYeNge9CgYr4OY0Bt0LcV4XqPDtu2pNj5uEMg2ordiI-Bem8-rvbllPDjqUyzbm3lncmHWTqBwFBlwR4h");
            ///Console.WriteLine(resop);

            //DISPLAYS PAYMENT METHOD BOTTOMSHEET;
            paymentbottomsheet_behaviour.SetBottomSheetCallback(new BottomCallBacks(paymentbottomsheet_behaviour));
            paymentbottomsheet_behaviour.State = BottomSheetBehavior.StateExpanded;
            tripgeodetails = new RideGeoDetails();
            tripgeodetails.payment_method = "cash";
            tripgeodetails.ride_id = helper.GenerateRandomString(10);
            orderstate = "PAYMENTSTATE";

        }

        private void Layout_destination_main_Click(object sender, EventArgs e)
        {
            Radio_destination_Click(sender, e);
            whichplace_definition = "destination";
            try
            {
                AutocompleteFilter typeFilter = new AutocompleteFilter.Builder()
                    .SetCountry("NG")
                    .Build();
                Intent intent = new PlaceAutocomplete.IntentBuilder(PlaceAutocomplete.ModeOverlay)
                    .SetFilter(typeFilter)
                    .Build(this);
                StartActivityForResult(intent, PLACE_AUTOCOMPLETE_REQUEST_CODE);
            }
            catch (GooglePlayServicesRepairableException ex)
            {
                // TODO: Handle the error.
            }
            catch (GooglePlayServicesNotAvailableException ex)
            {
                // TODO: Handle the error.
            }
        }

        private void Layout_location_main_Click(object sender, EventArgs e)
        {
            Radio_location_Click(sender, e);
            whichplace_definition = "location";
            try
            {
                AutocompleteFilter typeFilter = new AutocompleteFilter.Builder()
                    .SetCountry("NG")
                    .Build();
                Intent intent = new PlaceAutocomplete.IntentBuilder(PlaceAutocomplete.ModeOverlay)
                    .SetFilter(typeFilter)
                    .Build(this);
                StartActivityForResult(intent, PLACE_AUTOCOMPLETE_REQUEST_CODE);
            }
            catch (GooglePlayServicesRepairableException ex)
            {

            }
            catch (GooglePlayServicesNotAvailableException ex)
            {
                Toast.MakeText(this, "Google play service is not available", ToastLength.Short).Show();
            }
        }

        #endregion

        #region APPLICATION SETUPS, NAVIGATION, MENUS, APPDATA

        public void SetSharedPrefrences()
        {
            edit = pref.Edit();
            phone_s = pref.GetString("phone", "");
            firstname_s = pref.GetString("firstname", "");
            lastname_s = pref.GetString("lastname", "");
            email_s = pref.GetString("email", "");
            photourl_s = pref.GetString("photourl", "");
            firsttime_s = pref.GetString("firsttime", "");
            txtnav_title.Text = firstname_s + " " + lastname_s;
            just_registered = pref.GetString("justregistered", "");
        }

        public void SetProfileImage()
        {
            string imagebase64 = pref.GetString("imagestring", "");
            if (!string.IsNullOrEmpty(imagebase64))
            {
                helper.SetProfileImage(imgProfile, imagebase64);
            }

        }

        public void downloadImageImage()
        {
            string imageurl = pref.GetString("photourl", "");
            if (imageurl != "")
            {
                string imagestring = webhelpers.downloadImage1(imageurl);


                if (!string.IsNullOrEmpty(imagestring))
                {
                    edit.PutString("imagestring", imagestring);
                    edit.PutString("firsttime", "false");
                    edit.Apply();
                    RunOnUiThread(() =>
                    {
                        helper.SetProfileImage(imgProfile, imagestring);
                    });
                }
                else
                {
                    string imagestring1 = webhelpers.downloadImage1(imageurl);
                    if (!string.IsNullOrEmpty(imagestring1))
                    {
                        edit.PutString("imagestring", imagestring1);
                        edit.PutString("firsttime", "false");
                        edit.Apply();
                        RunOnUiThread(() =>
                        {
                            helper.SetProfileImage(imgProfile, imagestring1);
                        });
                    }
                }
            }
        }



        #endregion

        #region LOCATION FUNCTIONS
        private void CreateLocationRequest()
        {
            mLocationRequest = new LocationRequest();
            mLocationRequest.SetInterval(UPDATE_INTERVAL);
            mLocationRequest.SetFastestInterval(FASTEST_INTERVAL);
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            mLocationRequest.SetSmallestDisplacement(DISPLACEMENT);
        }

        private void BuildGoogleApiClient()
        {
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(LocationServices.API).Build();
            mGoogleApiClient.Connect();
        }

        private bool CheckPlayServices()
        {
            int resultCode = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GooglePlayServicesUtil.IsUserRecoverableError(resultCode))
                {
                    GooglePlayServicesUtil.GetErrorDialog(resultCode, this, PLAY_SERVICES_RESOLUTION_REQUEST).Show();
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "This device is not support Google Play Services", ToastLength.Long).Show();
                    Finish();
                }

                return false;
            }
            return true;
        }
        private void StartLocationUpdates()
        {
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }
            LocationServices.FusedLocationApi.RequestLocationUpdates(mGoogleApiClient, mLocationRequest, this);
        }

        private void StopLocationUpdates()
        {
            LocationServices.FusedLocationApi.RemoveLocationUpdates(mGoogleApiClient, this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            //try
            //{
            //    // Customise the styling of the base map using a JSON object defined
            //    // in a raw resource file.
            //    bool success = googleMap.SetMapStyle(
            //            MapStyleOptions.LoadRawResourceStyle(this, Resource.Raw.uber_style_map));

            //    if (!success)
            //    {
            //        Console.WriteLine("Error");
            //    }
            //}
            //catch (Resources.NotFoundException e)
            //{
            //    Console.WriteLine("Error");
            //}

            mainMap = googleMap;
            mainMap.CameraIdle += MainMap_CameraIdle;
            mainMap.CameraMoveStarted += MainMap_CameraMoveStarted;

        }

        private void MainMap_CameraMoveStarted(object sender, GoogleMap.CameraMoveStartedEventArgs e)
        {

            if (whichplace_marker == "destination")
            {
                txtdestination_main.Text = "Seting new destination....";
                DestinationSelected();
            }
            else if (whichplace_marker == "location")
            {
                txtlocation_main.Text = "Setting new location.....";
            }
        }

        private void MainMap_CameraIdle(object sender, EventArgs e)
        {
            LatLng position = mainMap.CameraPosition.Target;
            if (whichplace_marker == "destination")
            {
                if (TakeAddressFromSearch)
                {
                    txtdestination_main.Text = destination_address1;
                }
                else
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        txtdestination_main.Text = "Fetching address...";
                        TakeAddressFromSearch = false;
                        FindCordinateAddressForMarker(position, txtdestination_main, "destination");

                    }
                    else
                    {
                        Toast.MakeText(this, "Internet connectivity is not available", ToastLength.Long).Show();
                    }

                }

            }
            else if (whichplace_marker == "location")
            {

                if (TakeAddressFromSearch)
                {
                    txtlocation_main.Text = location_address;
                }
                else
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        txtlocation_main.Text = "Fetching address...";
                        TakeAddressFromSearch = false;
                        FindCordinateAddressForMarker(position, txtlocation_main, "location");
                    }
                    else
                    {
                        Toast.MakeText(this, "Internet connectivity is not available", ToastLength.Long).Show();
                    }

                }
            }
        }



        public async void FindCordinateAddressForMarker(LatLng thisposition, TextView whichtext, string whichplace)
        {
            MapFunctionHelper mapfunction = new MapFunctionHelper();
            WebRequestHelpers webrequests = new WebRequestHelpers();

            string durl = mapfunction.GetGeocodeUrl(thisposition.Latitude, thisposition.Longitude, DirectionKey);
            string json = "";

            await Task.Run(() =>
            {
                json = webrequests.GetDirectionJson(durl);
            });

            Console.WriteLine(json);
            if (string.IsNullOrEmpty(json))
            {
                if (whichplace == "destination")
                {
                    whichtext.Text = "Set destination";
                }
                else if (whichplace == "location")
                {
                    whichtext.Text = "Pickup location";
                }
                return;
            }

            var deser = JsonConvert.DeserializeObject<GeoLocationParser>(json);
            if (!deser.status.Contains("ZERO"))
            {
                if (deser.results[0] != null)
                {
                    string place = deser.results[0].formatted_address;
                    if (whichplace == "destination")
                    {
                        latlng_destination1 = thisposition;
                        destination_address1 = place;
                        whichtext.Text = place;
                    }
                    else if (whichplace == "location")
                    {
                        latlng_pickuplocation = thisposition;
                        location_address = place;
                        whichtext.Text = place;
                    }
                }
            }
            else
            {
                if (whichplace == "destination")
                {
                    whichtext.Text = "Set destination";
                }
                else if (whichplace == "location")
                {
                    whichtext.Text = "Pickup location";
                }
            }


            // e
        }

        public void InvalidateCenterMarker()
        {
            imgcenter_marker.Visibility = ViewStates.Invisible;
            whichplace_marker = "none";
            radio_location.Enabled = false;
            radio_destination.Enabled = false;
        }

        public void ValidateCenterMarker()
        {
            imgcenter_marker.Visibility = ViewStates.Visible;
            whichplace_marker = "location";
            radio_location.Enabled = true;
            radio_destination.Enabled = true;

            radio_destination.Checked = false;
            radio_location.Checked = true;
            imgcenter_marker.SetImageResource(Resource.Drawable.marker_green4);

        }
        #endregion

        #region RIDER REQUEST FUNCTION, MAPS AND REQUESTS

        private void DisplayLocation()
        {
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }


            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);

            if (mLastLocation != null)
            {
                Console.WriteLine("displayLocation: Latitude = " + mLastLocation.Latitude.ToString() + "   longitude = " + mLastLocation.Longitude.ToString());
                location_displayed = true;
                LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
                MarkerOptions markerOptions = new MarkerOptions();
                markerOptions.SetPosition(new LatLng(mLastLocation.Latitude, mLastLocation.Longitude));
                markerOptions.SetTitle("Pickup Location");
                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.destination_marker));
                //  locationMarker = mainMap.AddMarker(markerOptions);
                mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 15));

                Console.WriteLine("LocationCode = " + mLastLocation.Latitude.ToString() + "  ," + mLastLocation.Longitude);
                //  locationMarker.ShowInfoWindow();
                txtlocation_main.Text = "Fetching address...";
                try
                {
                    FindCordinateAddress(myposition);
                }
                catch
                {
                    Toast.MakeText(this, "Internet connectivity is not available 111", ToastLength.Long).Show();

                }
            }
            //else
            //{
            //    LatLng myposition = new LatLng(0, 0);
            //    MarkerOptions markerOptions = new MarkerOptions();
            //    markerOptions.SetPosition(new LatLng(0, 0));
            //    markerOptions.SetTitle("Pickup Location");
            //    markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.destination_marker));
            //    locationMarker = mainMap.AddMarker(markerOptions);
            //    locationMarker.ShowInfoWindow();
            //}
        }

        public void DestinationSelected()
        {
            //THIS FUNCTION IS CALLED WHEN LOCATION AND DESTINATION HAS BEEN SELECTED

            btn_favlocation_round.Visibility = ViewStates.Invisible;
            // btn_addmoredestination_main.Visibility = ViewStates.Visible;
            btn_doneonetrip_main.Visibility = ViewStates.Visible;
            layout_destination_main.Clickable = true;
            layout_location_main.Clickable = true;
            orderstate = "LOCALIZEDSTATE";
        }

        public void ReverseSelected()
        {
            btn_favlocation_round.Visibility = ViewStates.Visible;
            // btn_addmoredestination_main.Visibility = ViewStates.Invisible;
            btn_doneonetrip_main.Visibility = ViewStates.Invisible;

            layout_destination_main.Clickable = true;
            layout_location_main.Clickable = true;

            txtlocation_main.Text = "Pickup location";
            txtdestination_main.Text = "Set destination";
            orderstate = "REST";
            rejectedDrivers.Clear();
            driverFound_yetToAccept = "";
            ValidateCenterMarker();
            mainMap.Clear();
            DisplayLocation();
        }

        public void ReverseRequestState()
        {
            //requestbottomsheet_behaviour.SetBottomSheetCallback(new BottomCallBacksReverse(requestbottomsheet_behaviour));
            //paymentbottomsheet_behaviour.SetBottomSheetCallback(new BottomCallBacksReverse(paymentbottomsheet_behaviour));

            //requestbottomsheet_behaviour.Hideable = true;
            //paymentbottomsheet_behaviour.Hideable = true;
            requestbottomsheet_behaviour.State = BottomSheetBehavior.StateHidden;
            paymentbottomsheet_behaviour.State = BottomSheetBehavior.StateHidden;
            ReverseSelected();
            orderstate = "PAYMENTSTATE";
        }

        public void ReversePaymentState()
        {
            paymentbottomsheet_behaviour.State = BottomSheetBehavior.StateHidden;
            ReverseSelected();
        }

        public void PerformOneTrip(string json)
        {

            var deser = JsonConvert.DeserializeObject<DirectionParser>(json);
            // e.Marker.Snippet = deser.routes[0].legs[0].start_address;
            //ENCODED ROUTE
            string duration = deser.routes[0].legs[0].duration.text;
            FirstDuration = duration;

            var pointcode = deser.routes[0].overview_polyline.points;
            var line = PolyUtil.Decode(pointcode);
            ArrayList routeList = new ArrayList();
            foreach (LatLng item in line)
            {
                routeList.Add(item);
            }

            try
            {
                mPolyLine.Remove();
            }
            catch
            {

            }

            mainMap.Clear();

            PolylineOptions pointConnect = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(6)
                .InvokeColor(Color.DarkGray)
                .Geodesic(true);

            mPolyLine = mainMap.AddPolyline(pointConnect);
            double southlng = deser.routes[0].bounds.southwest.lng;
            double southlat = deser.routes[0].bounds.southwest.lat;
            double northlat = deser.routes[0].bounds.northeast.lat;
            double northlng = deser.routes[0].bounds.northeast.lng;
            LatLng southwest = new LatLng(southlat, southlng);
            LatLng northeast = new LatLng(northlat, northlng);
            LatLngBounds tripbound = new LatLngBounds(southwest, northeast);

            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count - 1];

            MarkerOptions markerOptions1 = new MarkerOptions();
            markerOptions1.SetPosition(firstpoint);
            markerOptions1.SetTitle("Pickup Location");
            markerOptions1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
            MarkerOptions markerOptions2 = new MarkerOptions();
            markerOptions2.SetPosition(lastpoint);
            markerOptions2.SetTitle("Your destination");
            markerOptions2.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));

            mainMap.AddMarker(markerOptions1);
            mainMap.AddMarker(markerOptions2);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(tripbound, 200));

            tripgeodetails.durationvalue = deser.routes[0].legs[0].duration.value;
            tripgeodetails.distance = deser.routes[0].legs[0].distance.text;
            tripgeodetails.distancevalue = deser.routes[0].legs[0].distance.value;
            tripgeodetails.duration = deser.routes[0].legs[0].duration.text;
            tripgeodetails.destination1_address = destination_address1;
            tripgeodetails.pickuplocation_address = location_address;

            tripgeodetails.latlng_destination1 = latlng_destination1;
            tripgeodetails.latlng_pickuplocation = latlng_pickuplocation;

            // ShowPaymentMethod();
            //ShowRequestAcxi(tripgeodetails);
        }

        public void ShowRequestAcxi()
        {
            orderstate = "REQUESTSTATE";
            paymentbottomsheet_behaviour.State = BottomSheetBehavior.StateHidden;
            HelperFunctions helper = new HelperFunctions();
            txtfaresEstimate_requestsheet.Text = (tripgeodetails.distancevalue > 1 && tripgeodetails.durationvalue > 1) ? helper.CurrencyConvert(helper.mEstimateFares(tripgeodetails.distancevalue, tripgeodetails.durationvalue, App_basefare, App_distancefare, App_timefare)) : "Undecided";
            txtarrival_requestsheet.Text = tripgeodetails.duration;
            txtpaymentMethod_requestsheet.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tripgeodetails.payment_method);

            requestbottomsheet_behaviour.SetBottomSheetCallback(new BottomCallBacks(requestbottomsheet_behaviour));
            requestbottomsheet_behaviour.State = BottomSheetBehavior.StateExpanded;
        }

        public void ShowFoundDriver(DriverDetails mydriver)
        {
            Thread t2 = new Thread(delegate ()
            {
                downloadImage(mydriver.photourl);

            });
            t2.Start();

            txtname_driverassigned.Text = mydriver.firstname;
            txtcarmodel_driverassigned.Text = mydriver.car_color + " " + mydriver.car_make + " " + mydriver.car_model + " " + mydriver.car_year;
            txtplatenumber_driverassigned.Text = mydriver.plate_number;
            requestbottomsheet_behaviour.State = BottomSheetBehavior.StateHidden;
            driverassignedbottomsheet_behaviour.SetBottomSheetCallback(new BottomCallBacks(driverassignedbottomsheet_behaviour));
            driverassignedbottomsheet_behaviour.State = BottomSheetBehavior.StateExpanded;
            string jstr = JsonConvert.SerializeObject(mydriver);
            edit.PutString("last_driver", jstr);
            edit.Apply();

            if (DriverLocation != null)
            {
                UpdateDriverLocation(DriverLocation.Latitude, DriverLocation.Longitude);

            }
        }

        public void downloadImage(string url)
        {
            driverImageString = webhelpers.downloadImage1(url);
            if (!string.IsNullOrEmpty(driverImageString))
            {
                RunOnUiThread(() =>
                {
                    helper.SetProfileImage(img_driverassigned, driverImageString);
                });

            }
            else
            {
                driverImageString = webhelpers.downloadImage1(url);
                if (!string.IsNullOrEmpty(driverImageString))
                {
                    RunOnUiThread(() =>
                    {
                        helper.SetProfileImage(img_driverassigned, driverImageString);
                    });

                }
            }
        }
        public async void FindCordinateAddress(LatLng thisposition)
        {
            try
            {
                MapFunctionHelper mapfunction = new MapFunctionHelper();
                WebRequestHelpers webrequests = new WebRequestHelpers();
                string durl = mapfunction.GetGeocodeUrl(thisposition.Latitude, thisposition.Longitude, DirectionKey);
                string json = "";

                await Task.Run(() =>
                {
                    json = webrequests.GetDirectionJson(durl);
                });


                if (!string.IsNullOrEmpty(json))
                {
                    var deser = JsonConvert.DeserializeObject<GeoLocationParser>(json);
                    if (deser.results[0] != null)
                    {
                        string place = deser.results[0].formatted_address;
                        txtlocation_main.Text = place;
                        latlng_pickuplocation = thisposition;
                        location_address = place;
                    }
                }
                else
                {
                    return;
                }



            }
            catch
            {
                // Toast.MakeText(this, "Error is here FindCordinate Function", ToastLength.Short).Show();
            }

        }

        #endregion

        #region OVERIDEABLE AND INTERFACES

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Android.App.Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PLACE_AUTOCOMPLETE_REQUEST_CODE)
            {
                if (resultCode == Android.App.Result.Ok)
                {
                    var place = PlaceAutocomplete.GetPlace(this, data);
                    // Console.WriteLine("this Place: " + place.AddressFormatted);
                    if (whichplace_definition == "location")
                    {
                        txtlocation_main.Text = place.NameFormatted.ToString();
                        location_address = place.NameFormatted.ToString();
                        latlng_pickuplocation = place.LatLng;
                        TakeAddressFromSearch = true;
                        mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 15));
                    }
                    else if (whichplace_definition == "destination")
                    {
                        DestinationSelected();
                        txtdestination_main.Text = place.NameFormatted.ToString();
                        destination_address1 = place.NameFormatted.ToString();
                        latlng_destination1 = place.LatLng;
                        TakeAddressFromSearch = true;
                        mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(place.LatLng, 15));

                    }
                }
                else if ((int)(resultCode) == (int)PlaceAutocomplete.ResultError)
                {
                    Console.WriteLine("error");

                }
                else if (resultCode == Android.App.Result.Canceled)
                {
                    // The user canceled the operation.
                }
            }
        }

        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }

        protected override void OnResume()
        {
            currentContext = this;
            base.OnResume();
        }
        public override void OnBackPressed()
        {
            // base.OnBackPressed();
            if (orderstate == "LOCALIZEDSTATE")
            {
                ReverseSelected();
            }
            else if (orderstate == "PAYMENTSTATE")
            {
                ReversePaymentState();
            }
            else if (orderstate == "REQUESTSTATE")
            {
                ReverseRequestState();
            }
            else if (orderstate == "REST")
            {
                base.OnBackPressed();
            }
        }


        public void OnConnected(Bundle connectionHint)
        {
            DisplayLocation();
            if (mRequestingLocationUpdates)
            {
                StartLocationUpdates();
            }
        }

        public void OnConnectionSuspended(int cause)
        {
            mGoogleApiClient.Connect();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            mGoogleApiClient.Connect();
        }

        public void OnError(Statuses status)
        {
            Console.WriteLine("connect error");
        }

        public void OnPlaceSelected(IPlace place)
        {
            //TODO
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
            mLastLocation = location;
            if (orderstate == "ONTRIP")
            {
                UpdateOnTrip();
            }
            if (!location_displayed)
            {
                DisplayLocation();
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
    }

}

