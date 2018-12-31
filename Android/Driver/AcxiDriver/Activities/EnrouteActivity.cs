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
using AcxiDriver.DataModels;
using Newtonsoft.Json;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content.Res;
using UK.CO.Chrisjenx.Calligraphy;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using static Android.Gms.Common.Apis.GoogleApiClient;
using Android.Gms.Common;
using Android.Locations;
using Android.Support.V4.App;
using Android;
using Com.Google.Maps.Android;
using Acxi.Helpers;
using Java.Util;
using Android.Graphics;
using AcxiDriver.Dialogue;
using Firebase.Database;
using Firebase;
using System.Diagnostics;
using AcxiDriver.EventListeners;
using System.Threading.Tasks;

namespace AcxiDriver.Activities
{
    [Activity(Label = "EnrouteActivity",MainLauncher = false, Theme ="@style/AcxiTheme1")]
    public class EnrouteActivity : AppCompatActivity, IOnMapReadyCallback, IConnectionCallbacks, IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        Android.Support.V7.Widget.Toolbar mtoolbar;
        TextView txtridername;
        ImageView btncall;
        RiderDetails riderDetails;
        TextView txtlocation_enroute;
        TextView txtdestination_enroute;
        Button btnenroute;
       public AppAlertDialogue myalert;
        CollectPaymentDialogue collectpay_dialogue;

        //STRINGS
        string driverActionState = "accepted";
        string DirectionKey = "AIzaSyBci2J_74vUgMj5beCsZB1O5s06OLhL9OA";
        string FirstDuration = "";
        bool isRequestingUpdate = false;
        //GOOGLECLIENT
        private GoogleMap mainMap;
        Marker pickupMarker;
        Marker mylocationMarker;
        Marker destinationMarker;
        private const int MY_PERMISSION_REQUEST_CODE = 7171;
        private const int PLAY_SERVICES_RESOLUTION_REQUEST = 7172;
        private bool mRequestingLocationUpdates = true;
        private LocationRequest mLocationRequest;
        private GoogleApiClient mGoogleApiClient;
        private Android.Locations.Location mLastLocation;
        private Android.Gms.Maps.Model.Polyline mPolyLine;
        private static int UPDATE_INTERVAL = 5000; //SEC
        private static int FASTEST_INTERVAL = 3000; //SEC
        private static int DISPLACEMENT = 5; // METERS

        FirebaseDatabase database;
        public DatabaseReference OngoingRideRef;
        DatabaseReference DriverEarningRef;
        OngoingRideListener ongoingride_listener;
        DatabaseReference EarningRef;
        EarningsValueEventListener earning_listener;

        Stopwatch DurationCounter;
        string App_basefare;
        string App_timefare;
        string App_distancefare;
        string App_stopfare;
      public  string Payment_method;
        string App_earning_percent;
       public double stops = 0;

        double earning_overall = 0;
        double earning_unpaid = 0;
        string myphone;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        private string status;

        MapFunctionHelper mapfunction = new MapFunctionHelper();
        WebRequestHelpers webhelpers = new WebRequestHelpers();
        HelperFunctions helpers = new HelperFunctions();
        private int updatelocation_animecount = 0;

        RelativeLayout btn_golocation;
        ImageView btn_golocation_img;
        bool isBackground = false;
        bool ridercancelled = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
   .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
.SetFontAttrId(Resource.Attribute.fontPath)
.Build());

            SetContentView(Resource.Layout.enroute);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.enrouteToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Enroute";
            string jstr = Intent.GetStringExtra("ride");
            status = Intent.GetStringExtra("resume_status");

            if (!string.IsNullOrEmpty(jstr))
            {
                riderDetails = JsonConvert.DeserializeObject<RiderDetails>(jstr);
            }

            SupportMapFragment mapfragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.routemap);
            mapfragment.GetMapAsync(this);
            myphone = pref.GetString("phone", "");
            txtridername = (TextView)FindViewById(Resource.Id.txtridername_route);
            txtlocation_enroute = (TextView)FindViewById(Resource.Id.txtlocation_enroute);
            txtdestination_enroute = (TextView)FindViewById(Resource.Id.txtdestination_enroute);
      
            txtridername.Text = (riderDetails != null && riderDetails.rider_name != null) ? riderDetails.rider_name : "Not available";
            txtlocation_enroute.Text = (riderDetails != null && riderDetails.pickup_address != null) ? riderDetails.pickup_address : "Not available";
            txtdestination_enroute.Text = (riderDetails != null && riderDetails.rider_name != null) ? riderDetails.destination_address : "Not available";

            btncall = (ImageView)FindViewById(Resource.Id.imgcall_route);
            btnenroute = (Button)FindViewById(Resource.Id.btnroute_action);
            btnenroute.Click += Btnenroute_Click;
            btncall.Click += Btncall_Click;

            btn_golocation = (RelativeLayout)FindViewById(Resource.Id.btn_mylocation);
            btn_golocation_img = (ImageView)FindViewById(Resource.Id.btn_mylocation_img);

            btn_golocation.Click += Btn_golocation_Click;
            btn_golocation_img.Click += Btn_golocation_img_Click;

            BuildGoogleApiClient();
            CreateLocationRequest();
            SetUpFirebase();
            
            //  CheckRideValidity();

            OngoingRideRef = database.GetReference("rideCreated/" + riderDetails.ride_id);
            ongoingride_listener = new OngoingRideListener(this);
            OngoingRideRef.AddValueEventListener(ongoingride_listener);
            UpdateStatusState();
        }

        protected override void OnPause()
        {
            isBackground = true;
            base.OnPause();
        }

        protected override void OnResume()
        {
            if (ridercancelled)
            {
                AppAlertDialogue appalert = new AppAlertDialogue("Rider has cancelled this trip");
                appalert.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction();
                appalert.Show(trans, "appalert");

                appalert.AlertOk += (o, r) =>
                {

                    RecoverTripInvalid();
                    appalert.Dismiss();
                    resumeMain();
                    this.Finish();
                };

                appalert.AlertCancel += (o, f) =>
                {
                    RecoverTripInvalid();
                    appalert.Dismiss();
                    resumeMain();
                    this.Finish();
                };
            }
            base.OnResume();
        }
        private void Btn_golocation_img_Click(object sender, EventArgs e)
        {
                //Notify Passenger,
                //Notify Passenger,
            if(driverActionState == "accepted")
            {
                LatLng address = new LatLng(riderDetails.latloc, riderDetails.lngloc);
                Navigate(address);
            }
            else if (driverActionState == "ongoing")
            {
                LatLng address = new LatLng(riderDetails.latdes, riderDetails.lngdes);
                Navigate(address);
            }
        }

        private void Btn_golocation_Click(object sender, EventArgs e)
        {
            if (driverActionState == "accepted")
            {
                LatLng address = new LatLng(riderDetails.latloc, riderDetails.lngloc);
                Navigate(address);
            }
            else if (driverActionState == "ongoing")
            {
                LatLng address = new LatLng(riderDetails.latdes, riderDetails.lngdes);
                Navigate(address);
            }
        }

        public void UpdateStatusState()
        {
           
            if (!string.IsNullOrEmpty(status))
            {

                if(status == "arrived")
                {
                    driverActionState = "arrived";
                    GetEarnings();
                    btnenroute.Text = "Start trip";
                    OngoingRideRef.Child("status").SetValue("arrived");

                   // TripReady();
                }
                else if (status == "ongoing")
                {
                    driverActionState = "ongoing";
                    GetEarnings();
                   
                    //show navigate button
                    OngoingRideRef.Child("status").SetValue("ongoing");
                    btnenroute.Text = "End trip";
                   // TripStart();
                }
            }
        }

        public void Navigate(LatLng address)
        {
            string toparse = "google.navigation:q=" + address.Latitude.ToString() + "," + address.Longitude.ToString();
            Android.Net.Uri gmmIntentUri = Android.Net.Uri.Parse(toparse);

            Intent mapIntent = new Intent(Intent.ActionView, gmmIntentUri);
            mapIntent.SetPackage("com.google.android.apps.maps");
            try
            {
                StartActivity(mapIntent);
            }
            catch
            {
                Toast.MakeText(this, "Google Map is not installed on your device", ToastLength.Short).Show();
            }

        }

        async Task CheckRideValidity()
        {
            Task.Delay(10000);
            DatabaseReference ridevalidRef = database.GetReference("rideCreated/" + riderDetails.ride_id);
            RideValidityValueEventListener ridevalid_listener = new RideValidityValueEventListener();
            ridevalidRef.AddValueEventListener(ridevalid_listener);

            ridevalid_listener.IsRideValid += (o, r) =>
            {
                if (r.validity)
                {
                    return;
                }
                else
                {
                    ridevalidRef.RemoveEventListener(ridevalid_listener);
                    ridevalidRef.RemoveValue();
                    RideCancelled();

                    //AppAlertDialogue appl = new AppAlertDialogue("This trip has been cancelled by the rider");
                    //appl.Cancelable = false;
                    //var trans = SupportFragmentManager.BeginTransaction();
                    //appl.Show(trans, "appl");

                    //appl.AlertCancel += (i, p) =>
                    //{
                    //    resumeMain();
                    //    Finish();
                    //};

                    //appl.AlertOk += (u, b) =>
                    //{
                        
                    //    resumeMain();
                    //    Finish();
                    //};


                }
            };
        }

        private void Btnenroute_Click(object sender, EventArgs e)
        {
            if (driverActionState == "accepted")
            {
                driverActionState = "arrived";
                //Notify Passenger,
                //Show arrived on txtmins
                //change ongoing ridestatus to arrived
                GetEarnings();
                btnenroute.Text = "Start trip";
                OngoingRideRef.Child("status").SetValue("arrived");

                TripReady(); 
            }
            else if (driverActionState == "arrived")
            {
                driverActionState = "ongoing";
                AppAlertDialogue thisalert = new AppAlertDialogue("You are about to start this trip");
                var trans = SupportFragmentManager.BeginTransaction();
                thisalert.Cancelable = false;
                thisalert.Show(trans, "alert");
                thisalert.AlertOk += (o, p) =>
                {
                    //startlocation updates,
                    //begin timer,
                    //show navigate button
                    OngoingRideRef.Child("status").SetValue("ongoing");
                    btnenroute.Text = "End trip";
                    thisalert.Dismiss();
                    TripStart();
                };

                thisalert.AlertCancel += (t, w) =>
                {
                    thisalert.Dismiss();
                    return;
                };

            }
            else if (driverActionState == "ongoing")
            {
                AppAlertDialogue thisalert = new AppAlertDialogue("Drop-off passenger");
                var trans = SupportFragmentManager.BeginTransaction();
                thisalert.Cancelable = false;
                thisalert.Show(trans, "alert");
                thisalert.AlertOk += (o, p) =>
                {
                    TripEnd();
                    OngoingRideRef.Child("status").SetValue("ended");
                    btnenroute.Text = "Trip ended";
                    thisalert.Dismiss(); 
                };

                thisalert.AlertCancel += (t, w) =>
                {
                    thisalert.Dismiss();
                    return;
                };
            }
           
        }

        public void UpdateCurrentLocation()
        {

            LatLng pickuplocation = new LatLng(riderDetails.latloc, riderDetails.lngloc);
            LatLng destination = new LatLng(riderDetails.latdes, riderDetails.lngdes);
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
           
            if (mLastLocation != null)
            {
                HashMap map = new HashMap();
                map.Put("latitude", mLastLocation.Latitude.ToString());
                map.Put("longitude", mLastLocation.Longitude.ToString());
                if(database != null)
                {
                    DatabaseReference cLocationRef = database.GetReference("rideCreated/" + riderDetails.ride_id + "/driverLocation");
                    cLocationRef.SetValue(map);
                }
            }
           
        }

        public void UpdateWorkingLocation()
        {
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);

            if (mLastLocation != null)
            {
                HashMap map = new HashMap();
                map.Put("latitude", mLastLocation.Latitude.ToString());
                map.Put("longitude", mLastLocation.Longitude.ToString());
                if (database != null)
                {
                    DatabaseReference cLocationRef = database.GetReference("driversWorking/" + myphone);
                    cLocationRef.SetValue(map);
                }
            }
        }

        public async void UpdateRidersLocation()
        {
            LatLng pickuplocation = new LatLng(riderDetails.latloc, riderDetails.lngloc);
            LatLng destination = new LatLng(riderDetails.latdes, riderDetails.lngdes);
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
          
          
            if (mLastLocation != null)
            {
                LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
                string durl = "";
                string json = "";


                durl = mapfunction.getMapsApiDirectionsUrl(pickuplocation, myposition, DirectionKey);

                await Task.Run(() =>
                {
                    json = webhelpers.GetDirectionJson(durl);
                });

                if (string.IsNullOrEmpty(json))
                {
                    return;
                }

                var deser = JsonConvert.DeserializeObject<DirectionParser>(json);
                string duration = deser.routes[0].legs[0].duration.text;
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

              //  mPolyLine = mainMap.AddPolyline(pointConnect);
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
                markerOptions1.SetPosition(lastpoint);
                markerOptions1.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.car));
                markerOptions1.Anchor(0.5f, 0.5f);
                markerOptions1.SetRotation(helpers.bearingBetweenLocations(lastpoint, firstpoint));

                MarkerOptions markerOptions2 = new MarkerOptions();
                markerOptions2.SetPosition(firstpoint);
                markerOptions2.SetTitle("Pickup Location");
                markerOptions2.SetSnippet("Your rider is " + duration + " Away");
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


        public async void TripReady()
        {
            
            mainMap.Clear();
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetTitle("Drawing route");
            progress.SetCancelable(false);
            progress.Show();

            LatLng pickuplocation = new LatLng(riderDetails.latloc, riderDetails.lngloc);
            LatLng destination = new LatLng(riderDetails.latdes, riderDetails.lngdes);
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
            LatLng myposition = null;
            if (mLastLocation == null)
            {
                string lng = pref.GetString("longitude", "");
                string lat = pref.GetString("latitude", "");
                if(!string.IsNullOrEmpty(lng) && !string.IsNullOrEmpty(lat))
                {
                    myposition = new LatLng(double.Parse(lat), double.Parse(lng));
                }
            }
            else
            {
                myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
            }

            MapFunctionHelper mapfunction = new MapFunctionHelper();
            WebRequestHelpers webrequests = new WebRequestHelpers();
            string durl = "";
            bool isposition = false;

            if(mLastLocation != null)
            {
                isposition = true;
                durl = mapfunction.getMapsApiDirectionsUrl(myposition, destination, DirectionKey);
            }
            else
            {
                isposition = false;
                durl = mapfunction.getMapsApiDirectionsUrl(pickuplocation, destination, DirectionKey);
            }

            string json = "";
            await Task.Run(() =>
            {
                json = webrequests.GetDirectionJson(durl);
            });

            if (string.IsNullOrEmpty(json))
            {
                progress.Dismiss();
                AppAlertDialogue appAlert = new AppAlertDialogue("Unable to draw route, check internet connectivity");
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
                return;
            }

            var deser = JsonConvert.DeserializeObject<DirectionParser>(json);

            progress.Dismiss();

            string duration = deser.routes[0].legs[0].duration.text;
            FirstDuration = duration;
            var pointcode = deser.routes[0].overview_polyline.points;
            var line = PolyUtil.Decode(pointcode);
            LatLng firstpoint = line[0];
            LatLng lastpoint = line[line.Count - 1];

            if (isposition)
            {
                MarkerOptions markerOptions = new MarkerOptions();
                markerOptions.SetPosition(firstpoint);
                markerOptions.SetTitle("Current Location");
                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.car));
                pickupMarker = mainMap.AddMarker(markerOptions);
            }
            else
            {
                MarkerOptions markerOptions = new MarkerOptions();
                markerOptions.SetPosition(firstpoint);
                markerOptions.SetTitle("Pickup Address");
                markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
                pickupMarker = mainMap.AddMarker(markerOptions);
            }


            MarkerOptions markerOptions1 = new MarkerOptions();
            markerOptions1.SetPosition(lastpoint);
            markerOptions1.SetTitle("Destination");
            markerOptions1.SetSnippet(duration);
            markerOptions1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
            destinationMarker = mainMap.AddMarker(markerOptions1);
            
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

            PolylineOptions pointConnect = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(6)
                .InvokeStartCap(new SquareCap())
                .InvokeEndCap(new SquareCap())
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
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(tripbound, 150));
            destinationMarker.ShowInfoWindow();
        }
       
        public async void UpdateOnTrip()
        {
          
            LatLng destination = new LatLng(riderDetails.latdes, riderDetails.lngdes);
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
                //  Toast.MakeText(this, "Is Requesting update", ToastLength.Short).Show();
                await Task.Run(() =>
                {
                    json = webrequests.GetDirectionJson(durl);
                });

                if (string.IsNullOrEmpty(json))
                {
                    return;
                }

                var deser = JsonConvert.DeserializeObject<DirectionParser>(json);
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

        public void RideCancelled()
        {
            if (OngoingRideRef != null)
            {
                try
                {
                    OngoingRideRef.RemoveEventListener(ongoingride_listener);
                }
                catch
                {

                }
            }

            if (!isBackground)
            {
                AppAlertDialogue appalert = new AppAlertDialogue("Rider has cancelled this trip");
                appalert.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction();
                appalert.Show(trans, "appalert");

                appalert.AlertOk += (o, r) =>
                {

                    RecoverTripInvalid();
                    appalert.Dismiss();
                    resumeMain();
                    this.Finish();
                };

                appalert.AlertCancel += (o, f) =>
                {
                    RecoverTripInvalid();
                    appalert.Dismiss();
                    resumeMain();
                    this.Finish();
                };
            }
            else
            {
                ridercancelled = true;
            }
            
        }

        public void TripStart()
        {
            DurationCounter = new Stopwatch();
            DurationCounter.Start();
            mainMap.Clear();
            LatLng destination = new LatLng(riderDetails.latdes, riderDetails.lngdes);
            MarkerOptions markerOptions = new MarkerOptions();
            markerOptions.SetPosition(destination);
            markerOptions.SetTitle("Destination");
            markerOptions.SetSnippet(FirstDuration);
            markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
            destinationMarker = mainMap.AddMarker(markerOptions);
            mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(destination, 16));
          
            destinationMarker.ShowInfoWindow();
            DatabaseReference appsettingRef = database.GetReference("appSettings");
            AppSettingsListener appreflistener = new AppSettingsListener();
            appsettingRef.AddListenerForSingleValueEvent(appreflistener);
            appreflistener.BaseFareFound += (o, k) =>
            {
                App_basefare = k.basefare;
                App_distancefare = k.distancefare;
                App_timefare = k.timefare;
                App_stopfare = k.stopfare;
                App_earning_percent = k.earning_percentage;
            };
            
        }

        public void RecoverTripInvalid()
        {
            DatabaseReference tripOngoing = database.GetReference("drivers/" + myphone + "/ongoing");
            tripOngoing.RemoveValue();
        }
        public async void TripEnd()
        {

            RecoverTripInvalid();
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
            LatLng Endposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
            LatLng pickuplocation = new LatLng(riderDetails.latloc, riderDetails.lngloc);
            LatLng destinationLocation = new LatLng(riderDetails.latdes, riderDetails.lngdes);

           // var dist1 = SphericalUtil.ComputeDistanceBetween(pickuplocation, Endposition);
           
            MapFunctionHelper mapfunction = new MapFunctionHelper();
            WebRequestHelpers webrequests = new WebRequestHelpers();
            string durl = "";

            if (mLastLocation != null)
            {
                durl = mapfunction.getMapsApiDirectionsUrl(pickuplocation, Endposition, DirectionKey);
            }

            DurationCounter.Stop();
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();

            string json = "";

           await Task.Run(() =>
            {
                json =  webrequests.GetDirectionJson(durl);
            }); 

           
            progress.Dismiss();
            double dist = 0;
            dist = SphericalUtil.ComputeDistanceBetween(pickuplocation, Endposition);
            DirectionParser deser;

            Console.WriteLine("Spherical Distance = " + dist);
            double distanceValue = 0;
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    distanceValue = dist;
                }
                else
                {
                    deser = JsonConvert.DeserializeObject<DirectionParser>(json);
                    distanceValue = deser.routes[0].legs[0].distance.value;

                    Console.WriteLine("Google Distane = " + distanceValue);
                }
                
            }
            catch
            {
                distanceValue =  dist;
            }
           
            StopLocationUpdates();

            double duration =  (int.Parse( DurationCounter.ElapsedMilliseconds.ToString()) / 60000);
            double timefare = (duration * int.Parse(App_timefare));
            double distancefare =  ((distanceValue/1000) * int.Parse(App_distancefare));
            double stopsfare = stops * double.Parse(App_stopfare);

            Console.WriteLine("DistnaceValue = " + distanceValue.ToString() + "  DistaanceFare = " + distancefare.ToString());
            double basefare = int.Parse(App_basefare);
            double amount = timefare + distancefare + basefare + stopsfare;

            string timefare_s = timefare.ToString();
            string distance_fare_s = distancefare.ToString();
            
         
            double fare = Math.Floor(amount / 10) * 10;
            double rounddown = ((amount - fare) > 0 ? amount - fare : 0);

            HashMap map = new HashMap();
            map.Put("total_fare", amount.ToString());
            map.Put("time_fare", timefare_s);
            map.Put("distance_fare", distance_fare_s);
            map.Put("base_fare", basefare.ToString());
            map.Put("round_down", rounddown);
            map.Put("stop_fare", stopsfare.ToString());
            OngoingRideRef.Child("fares").SetValue(map);

            if (!string.IsNullOrEmpty(Payment_method))
            {
                if (Payment_method == "cash")
                {
                    TripEarning(fare, false);
                    collectpay_dialogue = new CollectPaymentDialogue(fare.ToString(), basefare.ToString(), distance_fare_s, timefare_s, stopsfare.ToString());
                    collectpay_dialogue.Cancelable = false;
                    var trans = SupportFragmentManager.BeginTransaction();
                    collectpay_dialogue.Show(trans, "collect_pay");
                    collectpay_dialogue.CollectPaymentClicked += (o, r) =>
                    {
                        collectpay_dialogue.Dismiss();
                        resumeMain();
                        this.Finish();
                    };
                }
                else if (Payment_method == "card")
                {
                    database.GetReference("users/" + riderDetails.rider_phone + "/wallet/ride_wallet").SetValue(amount);
                    TripEarning(fare, true);
                    resumeMain();
                    this.Finish();
                }
               
            }
           
        }

        public void GetEarnings()
        {
            earning_listener = new EarningsValueEventListener();
            earning_listener.EarningChanged += Earning_listener_EarningChanged;
            EarningRef = database.GetReference("driverEarnings/" + myphone);
            EarningRef.AddValueEventListener(earning_listener);

        }

        private void Earning_listener_EarningChanged(object sender, EarningsValueEventListener.OnEarningValue e)
        {
            earning_overall = e.earningOverall;
            earning_unpaid = e.earningBalance;
        }

        public void TripEarning( double fare , bool card)
        {
            DriverEarningRef = database.GetReference("driverEarnings/" + myphone);
            double percent = double.Parse(App_earning_percent) / 100;

            double earning = fare * percent;
            double new_earning_overall = 0;
            double new_earning_unpaid = 0;

            if (card)
            {
                new_earning_overall = earning_overall + earning;
                new_earning_unpaid = earning_unpaid + earning;
            }
            else
            {
                double acxi_earning = fare - earning;
                new_earning_overall = earning_overall + earning;
                new_earning_unpaid = earning_unpaid - acxi_earning;
            }

            HashMap earnMap = new HashMap();
            earnMap.Put("earning_overall", new_earning_overall.ToString());
            earnMap.Put("earning_unpaid", new_earning_unpaid.ToString());
            DriverEarningRef.SetValue(earnMap);
        }

        private void Btncall_Click(object sender, EventArgs e)
        {
            string rphone = "0" + riderDetails.rider_phone.Substring(3, riderDetails.rider_phone.Length - 3);
            var uri = Android.Net.Uri.Parse("tel:" + rphone);
            var intent = new Intent(Intent.ActionDial, uri);
            StartActivity(intent);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            //try
            //{
            //    bool success = googleMap.SetMapStyle(MapStyleOptions.LoadRawResourceStyle(Application.Context, Resource.Raw.uber_style_map));
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
            mainMap.UiSettings.MapToolbarEnabled = false;
            mainMap.UiSettings.MyLocationButtonEnabled = false;
          
            if(status == "ongoing")
            {
                TripStart();
            }
            else if(status == "arrived")
            {
                TripReady();
            }

        }

       public async void DrawPickUp()
        {
            if (riderDetails != null)
            {
               
                LatLng pickuplocation = new LatLng(riderDetails.latloc, riderDetails.lngloc);
                MarkerOptions markerOptions = new MarkerOptions();
                markerOptions.SetPosition(pickuplocation);
                markerOptions.SetTitle("Pickup Address");
                markerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
                pickupMarker = mainMap.AddMarker(markerOptions);

                mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
                if (mLastLocation != null)
                {
                    LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
                    MarkerOptions markerOptions1 = new MarkerOptions();
                    markerOptions1.SetPosition(new LatLng(mLastLocation.Latitude, mLastLocation.Longitude));
                    markerOptions1.SetTitle("My Location");
                       markerOptions1.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.car));
                    mylocationMarker = mainMap.AddMarker(markerOptions1);
                    mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 13));
                    
                    MapFunctionHelper mapfunction = new MapFunctionHelper();
                    WebRequestHelpers webrequests = new WebRequestHelpers();

                    string durl = mapfunction.getMapsApiDirectionsUrl(myposition, pickuplocation, DirectionKey);
                    string json = "";
                    await Task.Run(() =>
                    {
                        json = webrequests.GetDirectionJson(durl);
                    });

                    if (string.IsNullOrEmpty(json))
                    {
                        try
                        {
                            json = await webrequests.GetDirectionJsonAsync(durl);
                            if (string.IsNullOrEmpty(json))
                            {
                                Toast.MakeText(Application.Context, "There was a slight glitch drawing route", ToastLength.Short).Show();
                                return;
                            }
                        }
                        catch
                        {
                            Toast.MakeText(Application.Context, "There was a slight glitch drawing route", ToastLength.Short).Show();
                            return;
                        }
                    }

                    var deser = JsonConvert.DeserializeObject<DirectionParser>(json);
                    mainMap.Clear();
                    var pointcode = deser.routes[0].overview_polyline.points;
                    var line = PolyUtil.Decode(pointcode);
                    LatLng firstpoint = line[0];
                    LatLng lastpoint = line[line.Count - 1];

                    string duration = deser.routes[0].legs[0].duration.text;
                    MarkerOptions markerOptions2 = new MarkerOptions();
                    markerOptions2.SetPosition(firstpoint);
                    markerOptions2.SetTitle("My Location");
                    markerOptions2.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.car));
                    markerOptions2.Anchor(0.5f, 0.5f);
                    markerOptions2.SetRotation(helpers.bearingBetweenLocations(pickuplocation, myposition));

                    mylocationMarker = mainMap.AddMarker(markerOptions2);


                    MarkerOptions markerOptions3 = new MarkerOptions();
                    markerOptions3.SetPosition(lastpoint);
                    markerOptions3.SetTitle("Pickup Address");
                    markerOptions3.SetSnippet("Your rider is " + duration + " away");
                    markerOptions3.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
                    pickupMarker = mainMap.AddMarker(markerOptions3);

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
                   
                    PolylineOptions pointConnect = new PolylineOptions()
                        .AddAll(routeList)
                        .InvokeWidth(6)
                        .InvokeStartCap(new SquareCap())
                        .InvokeEndCap(new SquareCap())
                        .InvokeColor(Color.DarkGray)
                        .Geodesic(true);
                  //  mPolyLine = mainMap.AddPolyline(pointConnect);
                    double southlng = deser.routes[0].bounds.southwest.lng;
                    double southlat = deser.routes[0].bounds.southwest.lat;
                    double northlat = deser.routes[0].bounds.northeast.lat;
                    double northlng = deser.routes[0].bounds.northeast.lng;
                    LatLng southwest = new LatLng(southlat, southlng);
                    LatLng northeast = new LatLng(northlat, northlng);
                    LatLngBounds tripbound = new LatLngBounds(southwest, northeast);
                    mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(tripbound, 150));
                    pickupMarker.ShowInfoWindow();
                }
            }

           
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
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
                    Toast.MakeText(Application.Context, "This device is not support Google Play Services", ToastLength.Long).Show();
                    resumeMain();
                    Finish();
                }

                return false;
            }
            return true;
        }

        private void StartLocationUpdates()
        {
            if (ActivityCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }
            LocationServices.FusedLocationApi.RequestLocationUpdates(mGoogleApiClient, mLocationRequest, this);
        }

        private void StopLocationUpdates()
        {
            LocationServices.FusedLocationApi.RemoveLocationUpdates(mGoogleApiClient, this);
        }

        public void DisplayLocation()
        {
            if (ActivityCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }

            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);

            if (mLastLocation != null)
            {
                LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
                MarkerOptions markerOptions = new MarkerOptions();
                markerOptions.SetPosition(new LatLng(mLastLocation.Latitude, mLastLocation.Longitude));
                markerOptions.SetTitle("My Location");
                markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Mipmap.ic_location_on_black_48dp));
                pickupMarker = mainMap.AddMarker(markerOptions);
                mainMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 13));
            }
            else
            {

            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            if (string.IsNullOrEmpty(status))
            {
                DrawPickUp();
            }
            if(status == "accepted")
            {
                DrawPickUp();
            }
            StartLocationUpdates();

        }

        public void OnConnectionSuspended(int cause)
        {
            mGoogleApiClient.Connect();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            mGoogleApiClient.Connect();
        }

        public void OnLocationChanged(Location location)
        {
            mLastLocation = location;

            if(driverActionState == "ongoing")
            {
                UpdateOnTrip();
            }
           
            if (driverActionState == "ongoing")
            {
                UpdateCurrentLocation();
            }

            if (driverActionState == "accepted")
            {
                UpdateCurrentLocation();
                UpdateRidersLocation();
            }
        }

        public void resumeMain()
        {
            string channel = "";
            channel = Intent.GetStringExtra("channel");

            DatabaseReference cLocationRef = database.GetReference("driversWorking/" + myphone);
            cLocationRef.RemoveValue();
            if (string.IsNullOrEmpty(channel))
            {
                Intent intent = new Intent(this, typeof(AppMainActivity));
                StartActivity(intent);
            }
            else
            {
                DatabaseReference DriverAvailableRef = database.GetReference("driversAvailable/" + myphone + "/ride_id");
                DriverAvailableRef.SetValue("waiting");
            }

           
        }
    }
}