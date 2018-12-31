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
using Xamarin.Facebook.Login.Widget;
using SupporWidget = Android.Support.V7.Widget;
using Xamarin.Facebook;
using Java.Lang;
using Xamarin.Facebook.Login;
using Calligraphy;
using Android.Gms.Common.Apis;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using static Android.Gms.Common.Apis.GoogleApiClient;
using Android.Gms.Common;
using Org.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Acxi.Helpers;
using System.Globalization;

namespace Acxi.Activities
{
    [Activity(Label = "CompleteProfileActivity", MainLauncher = false, Theme = "@style/AcxiTheme1", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class CompleteProfileActivity : AppCompatActivity, IFacebookCallback, IConnectionCallbacks, IOnConnectionFailedListener, GraphRequest.IGraphJSONObjectCallback
    {
        LoginButton facebookLoginButton;
        SupporWidget.Toolbar mToolbar;

        private ICallbackManager mCallBackManager;
        private MyProfileTracker mProfileTracker;

        private GoogleApiClient mGoogleApiClient;
        private Button google_Loginbutton;
        private GoogleSignInOptions gso1;

        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        //CHECKS WHICH AUTH WAS CHOSEN (GOOGLE OR FACEBOOK)
        int whichAuth = 0;

        string phone = "";
        string facebookemail;

        EditText txtfullname;
        EditText txtemail;
        EditText txtpassword;
        EditText txtconfirm;
        Button btnfinalize;

        WebRequestHelpers webhelpers = new WebRequestHelpers();
        HelperFunctions helpers = new HelperFunctions();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());

            SetContentView(Resource.Layout.finishprofile);
            phone = Intent.GetStringExtra("phone");

            mToolbar = (SupporWidget.Toolbar)FindViewById(Resource.Id.completeprofileToolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back_black);

            mProfileTracker = new MyProfileTracker();
            mProfileTracker.mOnProfileChanged += MProfileTracker_mOnProfileChanged;
            mProfileTracker.StartTracking();

            //if (AccessToken.CurrentAccessToken != null)
            //{
            //    this.Finish();
            //    //Intent intent = new Intent(this, typeof(MainActivity));
            //    //StartActivity(intent);
            //}
            //else
            //{
            //    Console.WriteLine("Logged Out");
            //}

            //LOGIN WITH FACEBOOK
            facebookLoginButton = FindViewById<LoginButton>(Resource.Id.login_button);
            facebookLoginButton.SetReadPermissions(new List<string> { "public_profile", "user_friends", "email" });

            mCallBackManager = CallbackManagerFactory.Create();
            facebookLoginButton.RegisterCallback(mCallBackManager, this);

            //GOOGLE LOGIN
            google_Loginbutton = (Button)FindViewById(Resource.Id.googlebutton);
            google_Loginbutton.Click += Google_Loginbutton_Click;

            gso1 = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
               .RequestEmail()
               .Build();
            BuildGoogleApiClient();


            //USER REG
            btnfinalize = (Button)FindViewById(Resource.Id.btnfinalize);
            btnfinalize.Click += Btnfinalize_Click;
        }

        private async void Btnfinalize_Click(object sender, EventArgs e)
        {
            txtconfirm = (EditText)FindViewById(Resource.Id.txtconfirmpassword_reg);
            txtemail = (EditText)FindViewById(Resource.Id.txtemail_reg);
            txtfullname = (EditText)FindViewById(Resource.Id.txtname_reg);
            txtpassword = (EditText)FindViewById(Resource.Id.txtpassword_reg);

            if (txtfullname.Text.Length < 4)
            {
                Toast.MakeText(this, "Please provide your full name", ToastLength.Short).Show();
                return;
            }

            if (txtemail.Text.Length < 5 || !txtemail.Text.Contains("@"))
            {
                Toast.MakeText(this, "Please provide a valid email address", ToastLength.Short).Show();
                return;
            }


            if (txtpassword.Text != txtconfirm.Text)
            {
                Toast.MakeText(this, "Password does not match", ToastLength.Short).Show();
                return;
            }

            string firstname = "";
            string lastname = "";
            string password = txtpassword.Text.Trim();
            string email = txtemail.Text.Trim();

            string name = txtfullname.Text;
            string[] names = name.Split(' ');

            if (name.Length == 1)
            {
                firstname = names[0];
            }
            else if (names.Length == 2)
            {
                firstname = names[0];
                lastname = names[1];
            }
            else if (names.Length > 2)
            {
                firstname = names[0];
                lastname = names[1];
            }

            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetTitle("Logging you in");
            progress.SetCancelable(false);
            progress.Show();
            string responsejson = "";

            await Task.Run(() =>
            {
                responsejson = webhelpers.CommitRegistration(firstname, lastname, "", email, phone, "token", helpers.GetTimeStampNow().ToString(), "yes");
            });

            if (responsejson.Contains("suc"))
            {
                edit = pref.Edit();
                edit.PutString("phone", phone);
                edit.PutString("firstname", firstname);
                edit.PutString("lastname", lastname);
                edit.PutString("email", email);
                edit.PutString("photourl", "");
                edit.PutString("imagestring", "");
                edit.PutString("logintype", "user");
                edit.PutString("justregistered", "true");
                edit.Apply();

                progress.Dismiss();
                this.FinishAffinity();
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
            {
                progress.Dismiss();
                Toast.MakeText(this, "Opps, we experience a slight glitch while registering you, please try again", ToastLength.Long).Show();
            }
        }


        private void BuildGoogleApiClient()
        {
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                            .AddOnConnectionFailedListener(this)
                            .AddConnectionCallbacks(this)
                            .AddApi(Auth.GOOGLE_SIGN_IN_API, gso1).Build();
            mGoogleApiClient.Connect();
        }

        private void Google_Loginbutton_Click(object sender, EventArgs e)
        {
            whichAuth = 1;
            var intent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(intent, 101);
        }

        //LOGIN WITH FACEBOOK;
        private async void MProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            if (e.mProfile != null)
            {
                //GETS USER EMAIL ADDRESS
                GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);
                Bundle parameters = new Bundle();
                parameters.PutString("fields", "email");
                request.Parameters = parameters;

                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Please wait...");
                progress.SetTitle("Finalizing registration");
                progress.SetCancelable(false);
                progress.Show();
                await Task.Run(() =>
                {
                    request.ExecuteAndWait();
                });

                string firstname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.mProfile.FirstName);
                string lastname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.mProfile.LastName);
                string imageurl = (string)e.mProfile.GetProfilePictureUri(120, 120);
                string respnsejson = "";
                await Task.Run(() =>
                {
                    respnsejson = webhelpers.CommitRegistration(firstname, lastname, imageurl, facebookemail, phone, "token", helpers.GetTimeStampNow().ToString(), "yes");
                    Console.WriteLine(respnsejson);
                });

                if (respnsejson.Contains("suc") || respnsejson.Contains("Exception"))
                {
                    edit = pref.Edit();
                    edit.PutString("phone", phone);
                    edit.PutString("firstname", firstname);
                    edit.PutString("lastname", lastname);
                    edit.PutString("email", facebookemail);
                    edit.PutString("photourl", imageurl);
                    edit.PutString("logintype", "facebook");
                    edit.PutString("firsttime", "true");
                    edit.PutString("justregistered", "true");


                    edit.Apply();
                    progress.Dismiss();
                    this.FinishAffinity();
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                else if (respnsejson.Contains("exi"))
                {
                    await Task.Run(() =>
                    {
                        respnsejson = webhelpers.CommitRegistration(firstname, lastname, imageurl, facebookemail, phone, "token", helpers.GetTimeStampNow().ToString(), "yes");
                        Console.WriteLine(respnsejson);
                    });

                    if (respnsejson.Contains("suc") || respnsejson.Contains("Exception"))
                    {
                        edit = pref.Edit();
                        edit.PutString("phone", phone);
                        edit.PutString("firstname", firstname);
                        edit.PutString("lastname", lastname);
                        edit.PutString("email", facebookemail);
                        edit.PutString("photourl", imageurl);
                        edit.PutString("logintype", "facebook");
                        edit.PutString("firsttime", "true");
                        edit.PutString("justregistered", "true");

                        edit.Apply();
                        progress.Dismiss();
                        this.FinishAffinity();
                        Intent intent = new Intent(this, typeof(MainActivity));
                        StartActivity(intent);
                    }
                    else
                    {

                        progress.Dismiss();
                        Console.WriteLine("myresponse = " + respnsejson);
                        Toast.MakeText(this, "Opps, we experience a slight glitch while registering you, please try again", ToastLength.Long).Show();

                    }

                }
                else
                {
                    progress.Dismiss();
                    Console.WriteLine("myresponse = " + respnsejson);
                    Toast.MakeText(this, "Opps, we experience a slight glitch while registering you, please try again", ToastLength.Long).Show();
                }

            }

        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (whichAuth != 1)
            {
                mCallBackManager.OnActivityResult(requestCode, (int)resultCode, data);
            }

            if (whichAuth == 1)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                handleSignInResult(result);
            }
        }

        private async void handleSignInResult(GoogleSignInResult result)
        {
            whichAuth = 0;
            if (result.IsSuccess)
            {
                GoogleSignInAccount acc = result.SignInAccount;
                string firstname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acc.GivenName);
                string lastname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(acc.FamilyName);
                string imageurl = (acc.PhotoUrl != null) ? acc.PhotoUrl.ToString() : "";
                string email = acc.Email;

                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Please wait...");
                progress.SetTitle("Logging you in");
                progress.SetCancelable(false);
                progress.Show();

                string responsejson = "";
                await Task.Run(() =>
                {
                    responsejson = webhelpers.CommitRegistration(firstname, lastname, imageurl, email, phone, "token", helpers.GetTimeStampNow().ToString(), "yes");
                });

                if (responsejson.Contains("suc"))
                {
                    edit = pref.Edit();
                    edit.PutString("phone", phone);
                    edit.PutString("firstname", firstname);
                    edit.PutString("lastname", lastname);
                    edit.PutString("email", email);
                    edit.PutString("photourl", imageurl);
                    edit.PutString("logintype", "google");
                    edit.PutString("firsttime", "true");
                    edit.PutString("justregistered", "true");

                    edit.Apply();
                    progress.Dismiss();
                    this.FinishAffinity();
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                else
                {
                    progress.Dismiss();
                    Toast.MakeText(this, "Opps, we experience a slight glitch while registering you, please try again", ToastLength.Long).Show();
                }

            }
        }

        void IFacebookCallback.OnCancel()
        {
            //throw new NotImplementedException();
        }

        void IFacebookCallback.OnError(FacebookException error)
        {
            // throw new NotImplementedException();
        }

        void IFacebookCallback.OnSuccess(Java.Lang.Object result)
        {
            LoginResult loginResult = result as LoginResult;
            Console.WriteLine("user id = " + AccessToken.CurrentAccessToken.UserId);
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        void IConnectionCallbacks.OnConnected(Bundle connectionHint)
        {
            Console.WriteLine("CONNECTED");
        }

        void IConnectionCallbacks.OnConnectionSuspended(int cause)
        {
            Console.WriteLine("CONNECTION SUSPENDED");
        }

        void IOnConnectionFailedListener.OnConnectionFailed(ConnectionResult result)
        {
            mGoogleApiClient.Connect();
        }

        protected override void OnStart()
        {
            base.OnStart();
            mGoogleApiClient.Connect();
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (mGoogleApiClient.IsConnected)
            {
                mGoogleApiClient.Disconnect();
            }
        }

        protected override void OnDestroy()
        {
            mProfileTracker.StopTracking();
            mGoogleApiClient.Disconnect();
            base.OnDestroy();
        }

        void GraphRequest.IGraphJSONObjectCallback.OnCompleted(JSONObject json, GraphResponse response)
        {
            Console.WriteLine("Jsonstr is here " + json.ToString());
            var real = JObject.Parse(json.ToString());
            string email = "";
            if (real["email"] != null)
            {
                email = real["email"].ToString();
            }
            facebookemail = email;
        }
    }

    public class MyProfileTracker : ProfileTracker
    {
        public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;

        protected override void OnCurrentProfileChanged(Profile oldProfile, Profile newProfile)
        {
            if (mOnProfileChanged != null)
            {
                mOnProfileChanged.Invoke(this, new OnProfileChangedEventArgs(newProfile));
            }
        }
    }

    public class OnProfileChangedEventArgs : EventArgs
    {
        public Profile mProfile;
        public OnProfileChangedEventArgs(Profile profile) { mProfile = profile; }
    }
}