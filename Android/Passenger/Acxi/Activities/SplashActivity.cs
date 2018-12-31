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
using System.Threading.Tasks;
using System.IO;
using Acxi.Helpers;
using Android.Locations;
using Acxi.DialogueFragment;
using Android;
using Android.Content.PM;
using Java.Security;

namespace Acxi.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
           
            base.OnCreate(savedInstanceState, persistentState);

        }
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { DecideStartUp(); });
            startupWork.Start();
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

        public async void DecideStartUp()
        {
            //  TurnOnLOcationService();
         //  TryToGetPermissions();
            ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
            ISharedPreferencesEditor edit = pref.Edit();

            string logintype = pref.GetString("logintype", "");
            if(string.IsNullOrEmpty(logintype))
            {
                StartActivity(new Intent(Application.Context, typeof(GetStartedActivity)));
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }

            //PackageInfo info = this.PackageManager.GetPackageInfo("Acxi.Acxi", PackageInfoFlags.Signatures);
            //string keyhash = "";
            //foreach (Android.Content.PM.Signature signature in info.Signatures)
            //{
            //    MessageDigest md = MessageDigest.GetInstance("SHA");
            //    md.Update(signature.ToByteArray());
            //    keyhash = Convert.ToBase64String(md.Digest());
            //    Console.WriteLine("KeyHash:", keyhash);
            //}

            //string a = keyhash;
        }


        #region RuntimePermissions

        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                GetPermissionsAsync();
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
       void GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.AccessFineLocation;

            if (CheckSelfPermission(permission) == (int)Android.Content.PM.Permission.Granted)
            {
                //TODO change the message to show the permissions name
                Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();
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
                            Toast.MakeText(this, "Special permissions granted", ToastLength.Short).Show();

                        }
                        else
                        {
                            //Permission Denied :(
                            Toast.MakeText(this, "Special permissions denied", ToastLength.Short).Show();

                        }
                    }
                    break;
            }
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion
    }
}