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
using SupportWidget = Android.Support.V7.Widget;
using Calligraphy;
using Refractored.Controls;
using Acxi.Helpers;
using Android.Gms.Location.Places.UI;
using Android.Gms.Common.Apis;
using Android.Gms.Location.Places;
using Android.Gms.Common;
using Firebase.Database;
using Firebase;
using Java.Util;
using Acxi.DialogueFragment;

namespace Acxi.Activities
{
    [Activity(Label = "ProfileActivity", Theme ="@style/AcxiTheme1")]
    public class ProfileActivity : AppCompatActivity, IPlaceSelectionListener
    {
        private SupportWidget.Toolbar mToolbar;
        
        EditText txtname;
        EditText txtemail;

        TextView txtphone;
        TextView txthomeaddress;
        TextView txtworkaddress;

        LinearLayout btnhomeaddress;
        LinearLayout btnworkaddress;
        LinearLayout btnplaces;

        CircleImageView imgProfile;

        //SHARED PREFRENCES
        string phone_s;
        string firstname_s;
        string lastname_s;
        string email_s;
        string phontourl_s;
        string homeaddress_s;
        string workaddress_s;
        FirebaseDatabase database;
        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        //HELPERS
        HelperFunctions helper = new HelperFunctions();

        //VARIABLE TRACKS WHERE TO SET LOCATION SELECTED FROM PLACES API; (HOME/WORK)
        string whichlocation = "";
        int PLACE_AUTOCOMPLETE_REQUEST_CODE = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
        .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
     .SetFontAttrId(Resource.Attribute.fontPath)
     .Build());

            SetContentView(Resource.Layout.profile);
            mToolbar = (SupportWidget.Toolbar) FindViewById(Resource.Id.profileToolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Profile";

            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);

            txtname = (EditText)FindViewById(Resource.Id.txtname_profile);
            txtemail = (EditText)FindViewById(Resource.Id.txtemail_profile);

            txtphone = (TextView)FindViewById(Resource.Id.txtphone_profile);
            txthomeaddress = (TextView)FindViewById(Resource.Id.txthomeaddress_profile);
            txtworkaddress = (TextView)FindViewById(Resource.Id.txtworkaddress_profile);

            btnhomeaddress = (LinearLayout)FindViewById(Resource.Id.btnhomeaddress_profile);
            btnworkaddress = (LinearLayout)FindViewById(Resource.Id.btnworkaddress_profile);
            btnplaces = (LinearLayout)FindViewById(Resource.Id.btnplaces_profile);

            btnhomeaddress.Click += Btnhomeaddress_Click;
            btnworkaddress.Click += Btnworkaddress_Click;
            btnplaces.Click += Btnplaces_Click;


            imgProfile = (CircleImageView)FindViewById(Resource.Id.imgprofile);
            SetSharedPrefrences();
            SetProfileImage();
            database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
        }

        private void Btnplaces_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(SavedPlacesActivity));
            StartActivity(intent);
        }

        public override void OnBackPressed()
        {
            if (email_s != txtemail.Text)
            {
                if (txtemail.Text.Contains("@"))
                {
                    edit.PutString("email", txtemail.Text);
                    edit.Apply();

                    try
                    {
                        DatabaseReference emailRef = database.GetReference("users/" + phone_s + "/email");
                        emailRef.SetValue(txtemail.Text);
                    }
                    catch
                    {

                    }
                }
            }
            string name = txtname.Text;
            string[] names = name.Split(' ');

            if(names.Length< 2)
            {
                this.Finish();
               
            }
            if (names[0] != null)
            {
                if (!string.IsNullOrWhiteSpace(names[0]) && names[0].Length >= 2)
                {
                    string _firstname = names[0];
                    if (firstname_s != _firstname)
                    {
                        DatabaseReference firstnameRef = database.GetReference("users/" + phone_s + "/first_name");
                        firstnameRef.SetValue(_firstname);
                        edit.PutString("firstname", _firstname);
                        edit.Apply();
                    }
                }
            }

            if (names[1] != null)
            {
                if (!string.IsNullOrWhiteSpace(names[1]) && names[1].Length >= 2)
                {
                    string _lastname = names[1];
                    if (lastname_s != _lastname)
                    {
                        DatabaseReference lastnameRef = database.GetReference("users/" + phone_s + "/last_name");
                        lastnameRef.SetValue(_lastname);
                        edit.PutString("lastname", _lastname);
                        edit.Apply();
                    }
                }
            }


            this.Finish();
            base.OnBackPressed();
        }
        private void Btnworkaddress_Click(object sender, EventArgs e)
        {
            whichlocation = "work";
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

        private void Btnhomeaddress_Click(object sender, EventArgs e)
        {
            whichlocation = "home";
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

        public void SetSharedPrefrences()
        {
            phone_s = pref.GetString("phone", "");
            firstname_s = pref.GetString("firstname", "");
            lastname_s = pref.GetString("lastname", "");
            email_s = pref.GetString("email", "");
            phontourl_s = pref.GetString("photourl", "");
            homeaddress_s = pref.GetString("homeaddress", "");
            workaddress_s = pref.GetString("workaddress", "");

            txtname.Text = firstname_s + " " + lastname_s;
            txtemail.Text = email_s;
            txtphone.Text = phone_s;

            if (!string.IsNullOrEmpty(homeaddress_s))
            {
                txthomeaddress.Text = homeaddress_s;
            }

            if (!string.IsNullOrEmpty(workaddress_s))
            {
                txtworkaddress.Text = workaddress_s;
            }
            edit = pref.Edit();
        }
        public void SetProfileImage()
        {
            string imagebase64 = pref.GetString("imagestring", "");
            if (!string.IsNullOrEmpty(imagebase64))
            {
                helper.SetProfileImage(imgProfile, imagebase64);
            }

        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        void IPlaceSelectionListener.OnError(Statuses status)
        {
            //
        }

        void IPlaceSelectionListener.OnPlaceSelected(IPlace place)
        {
            //
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.profile_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    if (email_s != txtemail.Text)
                    {
                        if (txtemail.Text.Contains("@"))
                        {
                            edit.PutString("email", txtemail.Text);
                            edit.Apply();

                            try
                            {
                                DatabaseReference emailRef = database.GetReference("users/" + phone_s + "/email");
                                emailRef.SetValue(txtemail.Text);
                            }
                            catch
                            {

                            }
                        }
                    }
                    string name = txtname.Text;
                    string[] names = name.Split(' ');

                    if (names.Length < 2)
                    {
                        this.Finish();
                        return true;
                    }

                    if (names[0] != null)
                    {
                        if (!string.IsNullOrWhiteSpace(names[0]) && names[0].Length >=2)
                        {
                            string _firstname = names[0];
                            if(firstname_s != _firstname)
                            {
                                DatabaseReference firstnameRef = database.GetReference("users/" + phone_s + "/first_name");
                                firstnameRef.SetValue(_firstname);
                                edit.PutString("firstname", _firstname);
                                edit.Apply();
                            }
                        }
                    }

                    if(names[1] != null)
                    {
                        if(!string.IsNullOrWhiteSpace(names[1]) && names[1].Length >= 2)
                        {
                            string _lastname = names[1];
                            if(lastname_s != _lastname)
                            {
                                DatabaseReference lastnameRef = database.GetReference("users/" + phone_s + "/last_name");
                                lastnameRef.SetValue(_lastname);
                                edit.PutString("lastname", _lastname);
                                edit.Apply();
                            }
                        }
                    }
                    

                    this.Finish();
                    return true;

                case Resource.Id.btnlogout:
                    Logout();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PLACE_AUTOCOMPLETE_REQUEST_CODE)
            {
                if (resultCode == Android.App.Result.Ok)
                {
                    var place = PlaceAutocomplete.GetPlace(this, data);
                    // Console.WriteLine("this Place: " + place.AddressFormatted);
                    if (whichlocation == "home")
                    {
                        string homeaddress = place.NameFormatted.ToString();
                        double lat = place.LatLng.Latitude;
                        double lng = place.LatLng.Longitude;

                        edit.PutString("homeaddress", homeaddress);
                        edit.PutString("homelat", lat.ToString());
                        edit.PutString("homelng", lng.ToString());
                        txthomeaddress.Text = homeaddress;
                        edit.Apply();

                      
                        DatabaseReference homeref = database.GetReference("users/" + phone_s + "/savedplaces/home");
                        HashMap map = new HashMap();
                        map.Put("address", homeaddress);
                        map.Put("latitude", lat.ToString());
                        map.Put("longitude", lng.ToString());
                        homeref.SetValue(map);

                    }
                    else if (whichlocation == "work")
                    {
                       
                        string workaddress = place.NameFormatted.ToString();
                        double lat = place.LatLng.Latitude;
                        double lng = place.LatLng.Longitude;

                        edit.PutString("workaddress", workaddress);
                        edit.PutString("worklat", lat.ToString());
                        edit.PutString("worklng", lng.ToString());

                        txtworkaddress.Text = workaddress;
                        edit.Apply();

                        database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
                        DatabaseReference workref = database.GetReference("users/" + phone_s + "/savedplaces/work");
                        HashMap map = new HashMap();
                        map.Put("address", workaddress);
                        map.Put("latitude", lat.ToString());
                        map.Put("longitude", lng.ToString());
                        workref.SetValue(map);
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

        public void Logout()
        {
            AppAlertDialogue appalert = new AppAlertDialogue("You are about to Logout");
            appalert.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            appalert.Show(trans, "alert");
            appalert.AlertCancel += (o, i) =>
            {
                appalert.Dismiss();
            };
            appalert.AlertOk += (w, q) =>
            {
                appalert.Dismiss();
                edit.Clear();
                edit.Apply();
                FinishAffinity();
                StartActivity(new Intent(Application.Context, typeof(GetStartedActivity)));
              
            };
        }
    }
}