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
using Android.Views.InputMethods;
using Calligraphy;
using Android.Content.PM;
using Java.Security;
using Xamarin.Facebook;
using Acxi.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Acxi.Activities
{
    [Activity(Label = "VerifyActivity", MainLauncher = false, Theme = "@style/AcxiTheme1")]
    public class VerifyActivity : AppCompatActivity
    {
        EditText txtverify_pin1;
        EditText txtverify_pin2;
        EditText txtverify_pin3;
        EditText txtverify_pin4;

        TextView txtresend;
        TextView txtverify_phone;

        Button btnverify;
        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        string serverCode = "";
        string phone = "";
        string userexistence;

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
            // token = Intent.GetStringExtra("token");

            txtverify_phone.Text = phone;
        }

        private async void Txtresend_Click(object sender, EventArgs e)
        {
            //WebRequestHelpers webHelpers = new WebRequestHelpers();
            //ProgressDialog progress = new ProgressDialog(this);
            //progress.Indeterminate = true;
            //progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            //progress.SetMessage("Please wait...");
            //progress.SetTitle("Resending Verification Code");
            //progress.SetCancelable(false);
            //progress.Show();

            //await Task.Run(() =>
            //{
            //    serverCode = webHelpers.otp(phone);
            //});
            //progress.Dismiss();
            //Toast.MakeText(this, "OTP was resent successfully", ToastLength.Short).Show();

            if (userexistence.Contains("exi"))
            {
                WebRequestHelpers webHelpers = new WebRequestHelpers();
                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Please wait...");
                progress.SetCancelable(false);
                progress.Show();

                string userjson = await webHelpers.userprofilejson(phone);
                //DESERIALIZE AND SAVE TO APP DATA


                if (!string.IsNullOrEmpty(userjson))
                {
                    try
                    {
                        var data = (JObject)JsonConvert.DeserializeObject(userjson);
                        string firstname = data["first_name"].Value<string>();
                        string lastname = data["last_name"].Value<string>();
                        string email = data["email"].Value<string>();
                        string photo = data["photo_url"].Value<string>();

                        edit = pref.Edit();
                        edit.PutString("phone", phone);
                        edit.PutString("firstname", firstname);
                        edit.PutString("lastname", lastname);
                        edit.PutString("email", email);
                        edit.PutString("photourl", photo);
                        edit.PutString("firsttime", "true");
                        edit.PutString("logintype", "unknown");
                        //  edit.PutString("token", token);

                        edit.Apply();
                        progress.Dismiss();
                        this.FinishAffinity();

                        Intent intent = new Intent(this, typeof(MainActivity));
                        StartActivity(intent);
                    }
                    catch
                    {
                        progress.Dismiss();
                        Intent intent = new Intent(this, typeof(CompleteProfileActivity));
                        intent.PutExtra("phone", phone);
                        StartActivity(intent);
                    }

                }
            }
            else
            {
                Intent intent = new Intent(this, typeof(CompleteProfileActivity));
                intent.PutExtra("phone", phone);
                StartActivity(intent);
            }

        }

        private async void Btnverify_Click(object sender, EventArgs e)
        {


            var userCode = txtverify_pin1.Text + txtverify_pin2.Text + txtverify_pin3.Text + txtverify_pin4.Text;
            if (userCode == serverCode)
            {
                if (userexistence.Contains("exi"))
                {
                    WebRequestHelpers webHelpers = new WebRequestHelpers();
                    ProgressDialog progress = new ProgressDialog(this);
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage("Please wait...");
                    progress.SetCancelable(false);
                    progress.Show();

                    string userjson = await webHelpers.userprofilejson(phone);
                    //DESERIALIZE AND SAVE TO APP DATA


                    if (!string.IsNullOrEmpty(userjson))
                    {
                        try
                        {
                            var data = (JObject)JsonConvert.DeserializeObject(userjson);
                            string firstname = data["first_name"].Value<string>();
                            string lastname = data["last_name"].Value<string>();
                            string email = data["email"].Value<string>();
                            string photo = data["photo_url"].Value<string>();

                            edit = pref.Edit();
                            edit.PutString("phone", phone);
                            edit.PutString("firstname", firstname);
                            edit.PutString("lastname", lastname);
                            edit.PutString("email", email);
                            edit.PutString("photourl", photo);
                            edit.PutString("firsttime", "true");
                            edit.PutString("logintype", "unknown");
                            //  edit.PutString("token", token);

                            edit.Apply();
                            progress.Dismiss();
                            this.FinishAffinity();

                            Intent intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                        }
                        catch
                        {
                            progress.Dismiss();
                            Intent intent = new Intent(this, typeof(CompleteProfileActivity));
                            intent.PutExtra("phone", phone);
                            StartActivity(intent);
                        }

                    }
                }
                else
                {
                    Intent intent = new Intent(this, typeof(CompleteProfileActivity));
                    intent.PutExtra("phone", phone);
                    StartActivity(intent);
                }

            }
            else
            {

                if (userexistence.Contains("exi"))
                {
                    WebRequestHelpers webHelpers = new WebRequestHelpers();
                    ProgressDialog progress = new ProgressDialog(this);
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.SetMessage("Please wait...");
                    progress.SetCancelable(false);
                    progress.Show();

                    string userjson = await webHelpers.userprofilejson(phone);
                    //DESERIALIZE AND SAVE TO APP DATA


                    if (!string.IsNullOrEmpty(userjson))
                    {
                        try
                        {
                            var data = (JObject)JsonConvert.DeserializeObject(userjson);
                            string firstname = data["first_name"].Value<string>();
                            string lastname = data["last_name"].Value<string>();
                            string email = data["email"].Value<string>();
                            string photo = data["photo_url"].Value<string>();

                            edit = pref.Edit();
                            edit.PutString("phone", phone);
                            edit.PutString("firstname", firstname);
                            edit.PutString("lastname", lastname);
                            edit.PutString("email", email);
                            edit.PutString("photourl", photo);
                            edit.PutString("firsttime", "true");
                            edit.PutString("logintype", "unknown");
                            //  edit.PutString("token", token);

                            edit.Apply();
                            progress.Dismiss();
                            this.FinishAffinity();

                            Intent intent = new Intent(this, typeof(MainActivity));
                            StartActivity(intent);
                        }
                        catch
                        {
                            progress.Dismiss();
                            Intent intent = new Intent(this, typeof(CompleteProfileActivity));
                            intent.PutExtra("phone", phone);
                            StartActivity(intent);
                        }

                    }
                }
                else
                {
                    Intent intent = new Intent(this, typeof(CompleteProfileActivity));
                    intent.PutExtra("phone", phone);
                    StartActivity(intent);
                }
                // Toast.MakeText(this, "CODE does not match", ToastLength.Short).Show();
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
    }
}