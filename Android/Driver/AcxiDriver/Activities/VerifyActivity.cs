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
using UK.CO.Chrisjenx.Calligraphy;
using Android.Views.InputMethods;
using Acxi.Helpers;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase;

namespace AcxiDriver.Activities
{
    [Activity(Label = "VerifyActivity", Theme = "@style/AcxiTheme1")]
    public class VerifyActivity : AppCompatActivity, IValueEventListener
    {
        EditText txtverify_pin1;
        EditText txtverify_pin2;
        EditText txtverify_pin3;
        EditText txtverify_pin4;

        TextView txtresend;
        TextView txtverify_phone;

        Button btnverify;

        string serverCode = "";
        string phone = "";
        string userexistence = "";

        FirebaseDatabase database;
        DatabaseReference driver_details;


        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
      .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
   .SetFontAttrId(Resource.Attribute.fontPath)
   .Build());
            SetContentView(Resource.Layout.verify);

            btnverify = FindViewById<Button>(Resource.Id.btnverify);
            txtverify_phone = FindViewById<TextView>(Resource.Id.txtphone_verify);
            txtresend = FindViewById<TextView>(Resource.Id.txtresend_verify);

            txtverify_pin1 = FindViewById<EditText>(Resource.Id.txtverif_pin1);
            txtverify_pin2 = FindViewById<EditText>(Resource.Id.txtverif_pin2);
            txtverify_pin3 = FindViewById<EditText>(Resource.Id.txtverif_pin3);
            txtverify_pin4 = FindViewById<EditText>(Resource.Id.txtverif_pin4);

            txtverify_pin1.TextChanged += Txtverify_pin1_TextChanged;
            txtverify_pin2.TextChanged += Txtverify_pin2_TextChanged;
            txtverify_pin3.TextChanged += Txtverify_pin3_TextChanged;
            txtverify_pin4.TextChanged += Txtverify_pin4_TextChanged;
            txtresend.Click += Txtresend_Click;

            btnverify.Click += Btnverify_Click;

            serverCode = Intent.GetStringExtra("code");
            phone = Intent.GetStringExtra("phone");
            userexistence = Intent.GetStringExtra("userexistence");
            txtverify_phone.Text = phone;
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

        private async void Txtresend_Click(object sender, EventArgs e)
        {
            WebRequestHelpers webHelpers = new WebRequestHelpers();
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetTitle("Resending Verification Code");
            progress.SetCancelable(false);
            progress.Show();

            await Task.Run(() =>
            {
                serverCode = webHelpers.otp(phone);
            });

            progress.Dismiss();
            Toast.MakeText(this, "OTP was resent successfully", ToastLength.Short).Show();

        }

        private  void Btnverify_Click(object sender, EventArgs e)
        {

            var userCode = txtverify_pin1.Text + txtverify_pin2.Text + txtverify_pin3.Text + txtverify_pin4.Text;
            if (userCode == serverCode)
            {
                edit.PutString("phone", phone);
                edit.Apply();
                if (userexistence.Contains("exis"))
                {
                    ProgressDialog progress = new ProgressDialog(this);
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage("Please wait...");
                    progress.SetCancelable(false);
                    progress.Show();

                    driver_details = database.GetReference("drivers/" + phone);
                    driver_details.AddListenerForSingleValueEvent(this);
                }
                else
                {

                    Intent intent = new Intent(this, typeof(RegistrationActivity));
                    intent.PutExtra("phone", phone);
                    StartActivity(intent);
                }

            }
            else
            {
                Toast.MakeText(this, "CODE does not match", ToastLength.Short).Show();
            }
        }

        private void Txtverify_pin4_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);

            if (txtverify_pin4.Text.Length == 1)
            {
                var currentFocus = Window.CurrentFocus;
                if (currentFocus != null)
                {
                    inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                }
            }
        }

        private void Txtverify_pin3_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (txtverify_pin3.Text.Length == 1)
            {
                txtverify_pin4.RequestFocus();
            }
        }

        private void Txtverify_pin2_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (txtverify_pin2.Text.Length == 1)
            {
                txtverify_pin3.RequestFocus();
            }
        }

        private void Txtverify_pin1_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            EditText me = (EditText)sender;
            if (me.Text.Length == 1)
            {
                txtverify_pin2.RequestFocus();
            }
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        public void OnCancelled(DatabaseError error)
        {
            
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
           
            if (snapshot.Value != null)
            {
                string license_status = "", worthiness_status = "", profilepix_status = "";
                string created_at = "", first_name = "", lastname = "", phone_d = "", email = "", city = "", invite = "";
                string make = "", model = "", year = "", car_color = "", plate_number = "";
                string account_status = "";

                #region IF STATEMENTS
                if (snapshot.Child("documents").Value != null)
                {
                    if(snapshot.Child("account_status").Value != null)
                    {
                        account_status = snapshot.Child("account_status").Value.ToString();
                        edit.PutString("account_status", account_status);
                        edit.Apply();
                    }

                    if (snapshot.Child("documents").Child("license").Child("status").Value != null)
                    {
                        license_status = snapshot.Child("documents").Child("license").Child("status").Value.ToString();
                        edit.PutString("license_url", snapshot.Child("documents").Child("license").Child("url").Value.ToString());
                        edit.PutString("license_status", license_status);
                        edit.Apply();
                    }

                    if (snapshot.Child("documents").Child("worthiness").Child("status").Value != null)
                    {
                        worthiness_status = snapshot.Child("documents").Child("worthiness").Child("status").Value.ToString();
                        edit.PutString("worthiness_url", snapshot.Child("documents").Child("worthiness").Child("url").Value.ToString());
                        edit.PutString("worthiness_status", worthiness_status);
                        edit.Apply();
                    }

                    if (snapshot.Child("documents").Child("profile_pic").Child("status").Value != null)
                    {
                        profilepix_status = snapshot.Child("documents").Child("profile_pic").Child("status").Value.ToString();
                        edit.PutString("profilepix_url", snapshot.Child("documents").Child("profile_pic").Child("url").Value.ToString());
                        edit.PutString("profilepix_status", profilepix_status);
                        edit.Apply();
                    }
                }

                if (snapshot.Child("vehicle_details").Value != null)
                {
                  
                    if (snapshot.Child("vehicle_details").Child("make").Value != null)
                    {
                        make = snapshot.Child("vehicle_details").Child("make").Value.ToString();
                    }
                    if(snapshot.Child("vehicle_details").Child("model").Value != null)
                    {
                        model = snapshot.Child("vehicle_details").Child("model").Value.ToString();
                    }

                    if (snapshot.Child("vehicle_details").Child("year").Value != null)
                    {
                        year = snapshot.Child("vehicle_details").Child("year").Value.ToString();
                    }

                    if (snapshot.Child("vehicle_details").Child("color").Value != null)
                    {
                        car_color = snapshot.Child("vehicle_details").Child("color").Value.ToString();
                    }

                    if (snapshot.Child("vehicle_details").Child("plate_number").Value != null)
                    {
                        plate_number = snapshot.Child("vehicle_details").Child("plate_number").Value.ToString();
                    }

                    edit.PutString("make", make);
                    edit.PutString("model", model);
                    edit.PutString("year", year);
                    edit.PutString("color", car_color);
                    edit.PutString("platenumber", plate_number);
                    edit.Apply();
                }

                if(snapshot.Child("personal_details").Value != null)
                {

                    if(snapshot.Child("personal_details").Child("created_at").Value != null)
                    {
                        created_at = snapshot.Child("personal_details").Child("created_at").Value.ToString();
                    }

                    if (snapshot.Child("personal_details").Child("first_name").Value != null)
                    {
                        first_name = snapshot.Child("personal_details").Child("first_name").Value.ToString();
                    }

                    if (snapshot.Child("personal_details").Child("last_name").Value != null)
                    {
                        lastname = snapshot.Child("personal_details").Child("last_name").Value.ToString();
                    }

                    if (snapshot.Child("personal_details").Child("phone").Value != null)
                    {
                        phone_d = snapshot.Child("personal_details").Child("phone").Value.ToString();
                    }

                    if (snapshot.Child("personal_details").Child("email").Value != null)
                    {
                        email = snapshot.Child("personal_details").Child("email").Value.ToString();
                    }

                    if (snapshot.Child("personal_details").Child("city").Value != null)
                    {
                        city = snapshot.Child("personal_details").Child("city").Value.ToString();
                    }

                    if (snapshot.Child("personal_details").Child("invite").Value != null)
                    {
                        invite = snapshot.Child("personal_details").Child("invite").Value.ToString();
                    }

                    edit.PutString("firstname", first_name);
                    edit.PutString("lastname", lastname);
                    edit.PutString("phone", phone_d);
                    edit.PutString("email", email);
                    edit.PutString("city", city);
                    edit.PutString("invitecode", invite);
                    edit.Apply();

                }

                if(snapshot.Child("bank_details").Value != null)
                {
                    string account_name = "", account_number = "", bank_name = "";

                    if (snapshot.Child("bank_details").Child("account_name").Value != null)
                    {
                        account_name = snapshot.Child("bank_details").Child("account_name").Value.ToString();
                    }

                    if (snapshot.Child("bank_details").Child("account_number").Value != null)
                    {
                        account_number = snapshot.Child("bank_details").Child("account_number").Value.ToString();
                    }

                    if (snapshot.Child("bank_details").Child("bank_name").Value != null)
                    {
                        bank_name = snapshot.Child("bank_details").Child("bank_name").Value.ToString();
                    }

                    edit.PutString("account_name", account_name);
                    edit.PutString("account_number", account_number);
                    edit.PutString("bank_name", bank_name);
                    edit.Apply();
                }

                #endregion

                //if (license_status == "approved" && worthiness_status == "approved" && profilepix_status == "approved")
                //{
                //    // GOTO APP MAIN
                //    edit.PutString("login", "true");
                //    edit.Apply();
                //    Intent intent = new Intent(this, typeof(AppMainActivity));
                //    StartActivity(intent);
                //    this.FinishAffinity();

                //}
                //else if (license_status == "pending" && worthiness_status == "pending" && profilepix_status == "pending")
                //{
                //    // GOTO REGISTARTION COMPLETED
                //    edit.PutString("login", "true");
                //    edit.Apply();
                //    Intent intent = new Intent(this, typeof(RegCompletedActivity));
                //    StartActivity(intent);
                //    this.FinishAffinity();
                //}
                //else
                //{
                //    //GOTO REGISTRATION

                //    Intent intent = new Intent(this, typeof(RegistrationActivity));
                //    intent.PutExtra("phone", phone);
                //    StartActivity(intent);
                //    this.FinishAffinity();
                //}

            if(account_status == "pending")
                {

                    if (license_status == "pending" && worthiness_status == "pending" && profilepix_status == "pending")
                    {
                        // GOTO APP MAIN
                        edit.PutString("login", "true");
                        edit.Apply();
                        Intent intent = new Intent(this, typeof(RegCompletedActivity));
                        StartActivity(intent);
                        this.FinishAffinity();
                    }
                    else if (license_status == "rejected" || worthiness_status == "rejected" && profilepix_status == "rejected")
                    {
                        edit.PutString("login", "true");
                        edit.Apply();
                        Intent intent = new Intent(this, typeof(RegistrationActivity));
                        StartActivity(intent);
                        this.FinishAffinity();
                    }
                    else if(string.IsNullOrEmpty(license_status) || string.IsNullOrEmpty(license_status) || string.IsNullOrEmpty(profilepix_status))
                    {
                        edit.PutString("login", "true");
                        edit.Apply();
                        Intent intent = new Intent(this, typeof(RegistrationActivity));
                        StartActivity(intent);
                        this.FinishAffinity();
                    }
                    else
                    {
                        // GOTO REGISTARTION COMPLETED
                        edit.PutString("login", "true");
                        edit.Apply();
                        Intent intent = new Intent(this, typeof(RegCompletedActivity));
                        intent.PutExtra("status", account_status);
                        StartActivity(intent);
                        this.FinishAffinity();
                    }

                  
                }
                else if(account_status == "approved")
                {
                    // GOTO APP MAIN
                    edit.PutString("login", "true");
                    edit.PutString("account_status", account_status);
                    edit.Apply();
                    Intent intent = new Intent(this, typeof(AppMainActivity));
                    StartActivity(intent);
                    this.FinishAffinity();
                }
                else if (account_status == "suspended")
                {
                    // SUSPENDED
                    Intent intent = new Intent(this, typeof(RegCompletedActivity));
                    intent.PutExtra("status", account_status);
                    edit.PutString("account_status", account_status);
                    StartActivity(intent);
                    this.FinishAffinity();
                }
                else
                {
                    Intent intent = new Intent(this, typeof(RegistrationActivity));
                    if (string.IsNullOrEmpty(phone_d))
                    {
                        if (!string.IsNullOrEmpty(phone))
                        {
                            intent.PutExtra("phone", phone);
                            edit.PutString("phone", phone);
                            edit.Apply();
                        }
                    }
                    else
                    {
                        intent.PutExtra("phone", phone_d);
                        edit.PutString("phone", phone_d);
                        edit.Apply();
                    }
                   
                    StartActivity(intent);
                    this.FinishAffinity();
                }


            }
        }
    }
}