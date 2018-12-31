using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Support.V7.App;
using UK.CO.Chrisjenx.Calligraphy;
using Android.Support.Design.Widget;
using Acxi.Helpers;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Android;
using Android.Util;
using Firebase.Iid;
using Firebase;

namespace AcxiDriver.Activities
{
    [Activity(Label = "GetStartedActivity", MainLauncher = false, Theme ="@style/AcxiTheme1")]
    public class GetStartedActivity : AppCompatActivity
    {
        Button btngetstarted;
        TextInputLayout txtphone;
        WebRequestHelpers webHelpers = new WebRequestHelpers();

        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        FirebaseApp app_f;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            await TryToGetPermissions();

            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());

            SetContentView(Resource.Layout.getstarted);
            btngetstarted = (Button)FindViewById(Resource.Id.btngetstarted);
            txtphone = (TextInputLayout)FindViewById(Resource.Id.txtlayoutphone_getstarted);
            btngetstarted.Click += Btngetstarted_Click;
            edit = pref.Edit();
        }


        private async void Btngetstarted_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetTitle("Sending verification code");
            progress.SetCancelable(false);
            Intent intent = new Intent(this, typeof(VerifyActivity));


            string phone = txtphone.EditText.Text;
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

                progress.Show();
                string code = "";
                string userexistence = "";

               

                await Task.Run(() =>
                {
                    userexistence = webHelpers.userexistence(phone);
                    code = webHelpers.otp(phone);
                });

                if(code.Length > 20 || string.IsNullOrEmpty(code))
                {
                    Toast.MakeText(this, "Something went worng, please try again", ToastLength.Short).Show();
                    progress.Dismiss();
                    return;
                }

                if(code == "failed")
                {
                    Toast.MakeText(this, "Something went worng, please try again", ToastLength.Short).Show();
                    progress.Dismiss();
                    return;
                }
                Console.WriteLine("otp = " + code);
                progress.Dismiss();
                intent.PutExtra("code", code);
                intent.PutExtra("userexistence", userexistence.ToString());

                //TODO --If userexistence is true, load welcome back;
                this.StartActivity(intent);
            }
            else
            {
                Toast.MakeText(this, "No internet connectivity", ToastLength.Short).Show();
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
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.Camera,
                            Manifest.Permission.WakeLock,
                            Manifest.Permission.GetTopActivityInfo,
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