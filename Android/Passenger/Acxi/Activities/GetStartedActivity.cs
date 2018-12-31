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
using Calligraphy;
using System.Threading.Tasks;
using Acxi.Helpers;
using Plugin.Connectivity;
using Android;
using Firebase.Messaging;
using Firebase.Iid;
using Android.Util;
using Android.Content.PM;
using Java.Security;

namespace Acxi.Activities
{
    [Activity(Label = "GetStartedActivity", MainLauncher = false, Theme ="@style/AcxiTheme1")]
    public class GetStartedActivity : AppCompatActivity
    {
        Button btngetstarted;
        EditText txtphone;
        WebRequestHelpers webHelpers = new WebRequestHelpers();
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            //SETS DEFAULT FONT
            try
            {
                await TryToGetPermissions();
            }
            catch
            {

            }
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
     
                .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());

            // Create your application here
            SetContentView(Resource.Layout.getstarted);
            btngetstarted = (Button)FindViewById(Resource.Id.btnstarted);
            txtphone = (EditText)FindViewById(Resource.Id.txtphone_getstarted);
            btngetstarted.Click += Btngetstarted_Click;
            edit = pref.Edit();


            //PackageInfo info = this.PackageManager.GetPackageInfo("com.kinitaxi.ng", PackageInfoFlags.Signatures);

            //string keyhash = "";
            //foreach (Android.Content.PM.Signature signature in info.Signatures)
            //{
            //    MessageDigest md = MessageDigest.GetInstance("SHA");
            //    md.Update(signature.ToByteArray());
            //    keyhash = Convert.ToBase64String(md.Digest());
            //    Console.WriteLine("KeyHash:", keyhash);
            //}
            //txtphone.Text = keyhash;
            //string a = keyhash;
        }

        private async void Btngetstarted_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetTitle("Sending Verification Code");
            progress.SetCancelable(false);

            //Task.Run(() =>
            //{
            //    var instanceid = FirebaseInstanceId.Instance;
            //    instanceid.DeleteInstanceId();
            //    Log.Debug("TAG", "{0} {1}", instanceid.Token, instanceid.GetToken(this.GetString(Resource.String.gcm_defaultSenderId), Firebase.Messaging.FirebaseMessaging.InstanceIdScope));
            //});





            Intent intent = new Intent(this, typeof(VerifyActivity));
                      
            //CHECK IF PHONE NUMBER IS CORRECT
            string phone = txtphone.Text;
            if (phone.Length == 10 || phone.Length == 11)
            {
                if (phone.Length == 10)
                {
                    phone = "234" + phone;
                    intent.PutExtra("phone", phone);
                }
                else if (phone.Length == 11 && phone[0].ToString() == "0")
                {
                    phone = "234" + phone.Substring(1, 10);
                    intent.PutExtra("phone", phone);
                }
                else
                {
                    //incorrect phone
                    Toast.MakeText(this, "Phone number is incorrect", ToastLength.Short).Show();
                    return;
                }

            }
            else
            {
                //incorrect phone
                Toast.MakeText(this, "Phone number is incorrect", ToastLength.Short).Show();
                return;
            }
            progress.Show();

            string code = "";
            string userexistence = "";

            if (CrossConnectivity.Current.IsConnected)
            {
                  try
            {
               string token = FirebaseInstanceId.Instance.Token;
                edit.PutString("apptoken", token);
                edit.Apply();
            }
            catch
            {
                var instanceid = FirebaseInstanceId.Instance;
                instanceid.DeleteInstanceId();
                string token = FirebaseInstanceId.Instance.Token;
                edit.PutString("apptoken", token);
                edit.Apply();
            }
                await Task.Run(() =>
                {
                    code = webHelpers.otp(phone);
                    Console.WriteLine(code);
                });


                if (string.IsNullOrEmpty(code))
                {
                    progress.Dismiss();
                    Toast.MakeText(this, "Something went wrong, please try again", ToastLength.Short).Show();
                    return;
                }

                userexistence = await webHelpers.userexistence(phone);
                Console.WriteLine("otp = " + code);
                progress.Dismiss();
                intent.PutExtra("code", code);
                intent.PutExtra("userexistence", userexistence);
                this.StartActivity(intent);
            }
            else
            {
                progress.Dismiss();
                Toast.MakeText(this, "Internet connection is not available", ToastLength.Short).Show();
            }
          
        }

        

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }


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
                           // Toast.MakeText(this, "Permissions granted", ToastLength.Short).Show();
                            
                        }
                        else
                        {
                            //Permission Denied :(
                            Toast.MakeText(this, "Permissions denied", ToastLength.Short).Show();
                            FinishAffinity();
                        }
                    }
                    break;
            }
            //base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion
    }
}