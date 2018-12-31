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
using Android.Gms.Maps;
using SupportFragment = Android.Support.V4.App.Fragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using Android.Gms.Maps.Model;
using Android.Content.Res;
using Android.Gms.Location;
using Android.Gms.Common.Apis;
using static Android.Gms.Common.Apis.GoogleApiClient;
using Android.Gms.Common;
using Android.Locations;
using Android.Support.V4.App;
using Android;

namespace AcxiDriver.Fragments
{
    public class OnLocationUpdatedEventArgs : EventArgs
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Location LastLocation { get; set; }
    }
    public class HomeFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback, IConnectionCallbacks, IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        //GOOGLECLIENT
        private GoogleMap mainMap;
        Marker locationMarker;
        public bool firstlocation = false;

        private const int MY_PERMISSION_REQUEST_CODE = 7171;
        private const int PLAY_SERVICES_RESOLUTION_REQUEST = 7172;
        private bool mRequestingLocationUpdates = false;
        private LocationRequest mLocationRequest;
        private GoogleApiClient mGoogleApiClient;
        private Android.Locations.Location mLastLocation;

        private static int UPDATE_INTERVAL = 5; //SEC
        private static int FASTEST_INTERVAL = 5 ; //SEC
        private static int DISPLACEMENT = 2; // METERS

        //PUBLIC EVENT TO BROADCAST LOCATION WHEN UPDATED
        public event EventHandler<OnLocationUpdatedEventArgs> MOnLocationUpdate;
        //PUBLIC EVENT TO BROADCAST LOCATION WHEN DRIVER GOES ONLINE
        public event EventHandler<OnLocationUpdatedEventArgs> MFirstLocation;
        ImageView imgcenter;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Activity, new string[] {
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
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.home_frag, container, false);
            SupportMapFragment mapfragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            imgcenter = (ImageView)view.FindViewById(Resource.Id.img_center);
            ////if (mapfragment == null)
            ////{
            ////    var trans = FragmentManager.BeginTransaction();
            ////    mapfragment = SupportMapFragment.NewInstance();
            ////    trans.Replace(Resource.Id.map, mapfragment).Commit();
            ////    mapfragment.GetMapAsync(this);
            ////}
            ////else
            ////{
            ////    mapfragment.GetMapAsync(this);
            ////}

            mapfragment.GetMapAsync(this);

            return view;
        }
        
        public void OnMapReady(GoogleMap googleMap)
        {
            //try
            //{
            //    // Customise the styling of the base map using a JSON object defined
            //    // in a raw resource file.
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
         
        }

        private void CreateLocationRequest()
        {
            mLocationRequest = new LocationRequest();
            mLocationRequest.SetInterval(5 * 1000);
            mLocationRequest.SetFastestInterval(1 * 1000);
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            mLocationRequest.SetSmallestDisplacement(DISPLACEMENT);
        }

        private void BuildGoogleApiClient()
        {
            mGoogleApiClient = new GoogleApiClient.Builder(Activity)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .AddApi(LocationServices.API).Build();
            mGoogleApiClient.Connect();
        }

        private bool CheckPlayServices()
        {
            int resultCode = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(Application.Context);
            if (resultCode != ConnectionResult.Success)
            {
                if (GooglePlayServicesUtil.IsUserRecoverableError(resultCode))
                {
                    GooglePlayServicesUtil.GetErrorDialog(resultCode, Activity, PLAY_SERVICES_RESOLUTION_REQUEST).Show();
                }
                else
                {
                    Toast.MakeText(Application.Context, "This device is not support Google Play Services", ToastLength.Long).Show();
                    Activity.Finish();
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

        public void StopLocationUpdates()
        {
            LocationServices.FusedLocationApi.RemoveLocationUpdates(mGoogleApiClient, this);
        }

        public void DisplayLocation()
        {
            
            if (ActivityCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }

          mLastLocation =  LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);

            if (mLastLocation != null)
            {
                Console.WriteLine("displayLocation: Latitude = " + mLastLocation.Latitude.ToString() + "   longitude = " + mLastLocation.Longitude.ToString());
                LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);
                firstlocation = true;
                try
                {
                    mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 13));
                }
                catch
                {

                }
                MFirstLocation.Invoke(this, new OnLocationUpdatedEventArgs { Latitude = mLastLocation.Latitude, Longitude = mLastLocation.Longitude, LastLocation = mLastLocation });
               
            }
            else
            {
                LocationServices.FusedLocationApi.RequestLocationUpdates(mGoogleApiClient, mLocationRequest, this);
              
            }
          
        }

        public void UpdateMapLocation()
        {
            if (ActivityCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted && ActivityCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessCoarseLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }

            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);

            if (mLastLocation != null)
            {
                try
                {
                    mainMap.Clear();
                }
                catch
                {

                }
                LatLng myposition = new LatLng(mLastLocation.Latitude, mLastLocation.Longitude);

                try
                {
                    mainMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myposition, 13));
                }
                catch
                {

                }
                //INVOKES EVENT IN APP_MAINACTIVITY TO UPDATE LOCATION TO FIREBASE
                if (!firstlocation)
                {
                    MFirstLocation.Invoke(this, new OnLocationUpdatedEventArgs { Latitude = mLastLocation.Latitude, Longitude = mLastLocation.Longitude, LastLocation = mLastLocation });
                }
                MOnLocationUpdate.Invoke(this, new OnLocationUpdatedEventArgs { Latitude = mLastLocation.Latitude, Longitude = mLastLocation.Longitude, LastLocation = mLastLocation });
                firstlocation = true;
            }
          
        }

        public void GoOffline()
        {
            firstlocation = false;
            StopLocationUpdates();
            imgcenter.Visibility = ViewStates.Invisible;
           
        }

        public bool GoOnline()
        {
            bool response = true;
            StartLocationUpdates();
            DisplayLocation();
            if (response)
            {
                imgcenter.Visibility = ViewStates.Visible;
               
            }
            else
            {
                StopLocationUpdates();
            }
            return response;
        }

        
        public void OnConnectionFailed(ConnectionResult result)
        {
            mGoogleApiClient.Connect();
        }

        public void OnConnected(Bundle connectionHint)
        {
            mLastLocation = LocationServices.FusedLocationApi.GetLastLocation(mGoogleApiClient);
            Console.WriteLine("location");
        }

        public void OnConnectionSuspended(int cause)
        {
            mGoogleApiClient.Connect();
        }

        public void OnLocationChanged(Location location)
        {
            mLastLocation = location;
            UpdateMapLocation();
        }
    }
}