using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using static Android.Gms.Common.Apis.GoogleApiClient;
using System;
using Android.Gms.Common;
using Android.Locations;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using Android.Support.V4.App;
using Android;
using Firebase;
using Firebase.Database;
using Java.Util;
using AcxiDriver.DataModels;
using Android.Gms.Maps.Model;
using Acxi.Helpers;

namespace AcxiDriver
{
    [Activity(Label = "AcxiDriver", MainLauncher = false, Theme = "@style/AcxiTheme1")]
    public class MainActivity : AppCompatActivity, IConnectionCallbacks, IOnConnectionFailedListener, Android.Gms.Location.ILocationListener, IValueEventListener
    {
        Android.Support.V7.Widget.Toolbar mtoolbar;
        TextView txtlongitude;
        TextView txtlatitude;
        TextView txtridername;
        TextView txtriderphone;
        TextView txtride_location;
        TextView txtride_destination;

        Button btnonline;
        Button btnaccept;

        // SETS WHEN DRIVE IS AVAILABLE ONLINE
        bool DriverOnline = false;
        //SETS WHEN DRIVER HAS BEEN MATCHED BUT HAS NOT ACCEPTED;
        bool DriverMatched = false;
        //SETS WHEN DRIVER HAS BEEN MATCHED AND HAS BEEN ACCEPTED;
        bool DriverRideAccepted;


        //APPDATA
        string phone_s = "2348166106863";

        //LOCATION VARIABLES
        private const int MY_PERMISSION_REQUEST_CODE = 7171;
        private const int PLAY_SERVICES_RESOLUTION_REQUEST = 7172;
        private bool mRequestingLocationUpdates = true;
        private LocationRequest mLocationRequest;
        private GoogleApiClient mGoogleApiClient;
        private Location mLastLocation;
        private static int UPDATE_INTERVAL = 1000; //SEC
        private static int FASTEST_INTERVAL = 1000; //SEC
        private static int DISPLACEMENT = 10; // METERS


        //FIREBASE VARIABLES
        FirebaseDatabase database;
        DatabaseReference DriverAvailableRef;
        DatabaseReference DriverLocationRef;

        public DatabaseReference RiderFoundRef;
        public DatabaseReference RiderRequestRef;
        public DatabaseReference OrderAcceptedRef;
        public DatabaseReference DriverWorkingLocationRef;
        public DatabaseReference OngoingRideRef;

        public string rideID;
        public RiderDetails riderDetails;
        TextView txtfirelong;
        TextView txtfirelat;
       

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.tripsToolbar);
            btnonline = (Button)FindViewById(Resource.Id.btnonline);
            btnaccept = (Button)FindViewById(Resource.Id.btnaccept);
            txtlatitude = (TextView)FindViewById(Resource.Id.txtlatitude);
            txtlongitude = (TextView)FindViewById(Resource.Id.txtlongitude);

            txtfirelong = (TextView)FindViewById(Resource.Id.txtfireLong);
            txtfirelat = (TextView)FindViewById(Resource.Id.txtfireLat);
            txtridername = (TextView)FindViewById(Resource.Id.txtridername);
            txtriderphone = (TextView)FindViewById(Resource.Id.txtriderphone);
            txtride_location = (TextView)FindViewById(Resource.Id.txtpickupaddress);
            txtride_destination = (TextView)FindViewById(Resource.Id.txtdestinationaddress);

            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Home";

            btnonline.Click += Btnonline_Click;
            btnaccept.Click += Btnaccept_Click;
            //if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            //{
            //    ActivityCompat.RequestPermissions(this, new string[] {
            //        Manifest.Permission.AccessFineLocation,
            //        Manifest.Permission.AccessCoarseLocation
            //    }, MY_PERMISSION_REQUEST_CODE);
            //}
            //else
            //{
            //    if (CheckPlayServices())
            //    {
            //        BuildGoogleApiClient();
            //        CreateLocationRequest();
            //    }
            //}

            BuildGoogleApiClient();
            CreateLocationRequest();
            SetUpFirebase();
        }

        private void Btnaccept_Click(object sender, EventArgs e)
        {
            FirebaseAccepted();
        }

        private void Btnonline_Click(object sender, System.EventArgs e)
        {
            if (!DriverOnline)
            {
                DriverGoOnline();
            }
            else
            {
                DriverGoOffline();
            }
        }



        #region FIREBASE FUNCTIONS


        private void DriverGoOnline()
        {
            btnonline.Text = "GO OFFLINE";
            txtlatitude.Text = "0.0";
            txtlongitude.Text = "0.0";
            DriverOnline = true;
            StartLocationUpdates();
            FirebaseGoOnline();
        }

        private void DriverGoOffline()
        {
            btnonline.Text = "GO ONLINE";
            DriverOnline = false;
            StopLocationUpdates();
            FirebaseGoOffline();
        }
        public void SetUpFirebase()
        {
            try
            {
                var options = new FirebaseOptions.Builder()
                .SetApplicationId("acxi-185916")
                .SetApiKey("AIzaSyCNjO-hS2KkVk91KpgFbkPVMAEXfA6i1iE")
                .SetDatabaseUrl("https://acxi-185916.firebaseio.com")
                .SetStorageBucket("acxi-185916.appspot.com")
                .Build();
                var app = Firebase.FirebaseApp.InitializeApp(this, options);
                database = FirebaseDatabase.GetInstance(app);
            }
            catch
            {
                database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
            }


        }

        public void FirebaseGoOnline()
        {
            DriverAvailableRef = database.GetReference("driversAvailable/" + phone_s);
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);

            HashMap location = new HashMap();
            //location.Put("longitude", mLastLocation.Longitude.ToString());
            //location.Put("latitude", mLastLocation.Latitude.ToString());

            HashMap map = new HashMap();
            map.Put("location", location);
            map.Put("ride_id", "waiting");
            
            DriverAvailableRef.AddValueEventListener(this);
            DriverAvailableRef.SetValue(map);
            DriverLocationRef = database.GetReference("driversAvailable/" + phone_s + "/location");

        }

        public void FirebaseUpdateLocation()
        {
            HashMap map = new HashMap();
            map.Put("longitude", mLastLocation.Longitude.ToString());
            map.Put("latitude", mLastLocation.Latitude.ToString());
            DriverLocationRef.SetValue(map);
        }

        public void FirebaseGoOffline()
        {
            if(DriverAvailableRef != null)
            {
                DriverAvailableRef.RemoveValueAsync();
                DriverAvailableRef.RemoveEventListener(this);
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
                DriverAvailableRef.RemoveEventListener(this);
                DriverLocationRef.RemoveValueAsync();

                OrderAcceptedRef = database.GetReference("rideRequest/" + rideID + "/driver_id");
                OrderAcceptedRef.SetValue(phone_s);

                if (DriverRideAccepted)
                {
                    FirebaseUpdateWorkLocation();

                    OngoingRideRef = database.GetReference("rideOngoing/" + rideID );
                    OngoingRideRef.AddValueEventListener(new OngoingRideListener(this));
                    
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
        }
        public void ShowRider()
        {
            txtridername.Text = riderDetails.rider_name;
            txtriderphone.Text = riderDetails.rider_phone;
            txtride_location.Text = riderDetails.pickup_address;
            txtride_destination.Text = riderDetails.destination_address;
            
        }

        public class RiderFoundValueListener : Java.Lang.Object, IValueEventListener
        {
            MainActivity main;
            public RiderFoundValueListener(MainActivity mm)
            {
                main = mm;
            }
            public void OnCancelled(DatabaseError error)
            {
                //
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
             
                if (snapshot != null)
                {
                    main.riderDetails.rider_name = snapshot.Child("first_name").Value.ToString();
                    main.ShowRider();
                }
            }
        }

        public class RideRequestValueListener : Java.Lang.Object, IValueEventListener
        {
            MainActivity main;
            public RideRequestValueListener(MainActivity mm)
            {
                main = mm;
            }
            public void OnCancelled(DatabaseError error)
            {
                //
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot != null)
                {
                    main.riderDetails = new RiderDetails();
                    double pickup_lng = 0;
                    double pickup_lat = 0;

                    double destination_lng = 0;
                    double destination_lat = 0;

                    pickup_lng = double.Parse(snapshot.Child("location").Child("longitude").Value.ToString());
                    pickup_lat = double.Parse(snapshot.Child("location").Child("latitude").Value.ToString());

                    destination_lng = double.Parse(snapshot.Child("destination").Child("longitude").Value.ToString());
                    destination_lat = double.Parse(snapshot.Child("destination").Child("latitude").Value.ToString());

                    main.riderDetails.latlng_pickup = new LatLng(pickup_lat, pickup_lng);
                    main.riderDetails.latlng_destination = new LatLng(destination_lat, destination_lng);
                    main.riderDetails.pickup_address = snapshot.Child("pickup_address").Value.ToString();
                    main.riderDetails.destination_address = snapshot.Child("destination_address").Value.ToString();
                    main.riderDetails.rider_phone = snapshot.Child("rider_id").Value.ToString();
                    main.DriverMatched = true;
                    main.FirebaseGetRiderInfo();
                }
            }
        }

        public class OngoingRideListener : Java.Lang.Object, IValueEventListener
        {
            MainActivity main;
            public OngoingRideListener(MainActivity mm)
            {
                main = mm;
            }
            public void OnCancelled(DatabaseError error)
            {
                //
            }

            public void OnDataChange(DataSnapshot snapshot)
            {
                if (snapshot != null)
                {
                    //  WATCH STATUS FOR RIDE STARTED

                    // WATCH STATUS FOR RIDE COMPLETION;

                    //WATCH STATUS FOR RIDE CANCELLED - NULL;
                    //  driverOb.firstname = snapshot.Child("first_name").Value.ToString();

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


        public void DisplayLocation()
        {
            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }

            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
            if (mLastLocation != null)
            {
                txtlatitude.Text = "Latitude : " + mLastLocation.Latitude.ToString();
                txtlongitude.Text = "Longitude : " + mLastLocation.Longitude.ToString();
                if (DriverOnline)
                {
                    if (!DriverRideAccepted)
                    {
                        FirebaseUpdateLocation();
                    }

                    if (DriverMatched && DriverRideAccepted)
                    {
                        FirebaseUpdateWorkLocation();
                    }
                }
               
            }
            else
            {
                
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            DisplayLocation();
            if (DriverOnline)
            {
                StartLocationUpdates();
            } 
        }

        public void OnConnectionSuspended(int cause)
        {
            Toast.MakeText(this, "ConnectionSuspended", ToastLength.Short).Show();
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Toast.MakeText(this, "ConnectionFailed", ToastLength.Short).Show();
        }

        public void OnLocationChanged(Location location)
        {
            mLastLocation = location;
            DisplayLocation();
        }

        public void OnCancelled(DatabaseError error)
        {
            //
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if(snapshot.Value != null)
            {
                txtfirelong.Text = "Fire Longitude : " + snapshot.Child("location").Child("longitude").Value.ToString();
                txtfirelat.Text = "Fire Latitude : " +  snapshot.Child("location").Child("latitude").Value.ToString();
                string ride_id = snapshot.Child("ride_id").Value.ToString();

                if(ride_id != "waiting")
                {
                    if (!DriverMatched)
                    {
                        DriverMatched = true;
                        FirebaseRideMatched(ride_id);
                        rideID = ride_id;
                    }
                }
            }
        }

        #endregion

        protected override void OnStop()
        {
            base.OnStop();
            if (DriverOnline)
            {
                DriverGoOffline();
                DriverOnline = true;
            }
          
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (DriverOnline)
            {
                DriverGoOnline();
            }
           
        }
    }
}

