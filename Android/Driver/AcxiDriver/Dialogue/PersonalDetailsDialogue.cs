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
using Android.Support.Design.Widget;
using System.Globalization;

namespace AcxiDriver.Dialogue
{
   public class OnSaveProfileDetailsEventArgs : EventArgs
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string city { get; set; }
    }
    public class PersonalDetailsDialogue : Android.Support.V4.App.DialogFragment
    {
        TextInputLayout txtfirstname;
        TextInputLayout txtlastname;
        TextInputLayout txtphone;
        TextInputLayout txtcity;
        TextInputLayout txtemail;
        Button btnsave;
        public event EventHandler<OnSaveProfileDetailsEventArgs> OnSaveProfileDetails;

        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
      
        string firstname_a;
        string lastname_a;
        string email_a;
        string city_a;
        string phone_a;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            edit = pref.Edit();
            firstname_a = pref.GetString("firstname", "");
            lastname_a = pref.GetString("lastname", "");
            email_a = pref.GetString("email", "");
            city_a = pref.GetString("city", "");
            phone_a = pref.GetString("phone", "");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.profile_dialogue, container, false);
            txtfirstname = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutfirstname_account);
            txtlastname = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutlastname_account);
            txtemail = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutemail_account);
            txtcity = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutcity_account);
            txtphone = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutphone_account);
            btnsave = (Button)view.FindViewById(Resource.Id.btnsaveprofile_account);

            txtphone.EditText.Text = phone_a;
            txtcity.EditText.Text = city_a;
            txtemail.EditText.Text = email_a;
            txtfirstname.EditText.Text = firstname_a;
            txtlastname.EditText.Text = lastname_a;

            txtphone.EditText.Enabled= false;
            btnsave.Click += Btnsave_Click;
            return view;
        }

        private void Btnsave_Click(object sender, EventArgs e)
        {
            string firstname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtfirstname.EditText.Text.Trim());
            string lastname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtlastname.EditText.Text.Trim());
            string email = txtemail.EditText.Text.Trim();
            string city = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtcity.EditText.Text.Trim());

            if (string.IsNullOrEmpty(firstname) || string.IsNullOrWhiteSpace(firstname))
            {
                Toast.MakeText(Application.Context, "Please provide FIRSTNAME", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(lastname) || string.IsNullOrWhiteSpace(lastname))
            {
                Toast.MakeText(Application.Context, "Please provide LASTNAME", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email) || email.Contains("@") == false)
            {
                Toast.MakeText(Application.Context, "Please provide a valid email address", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(city) || string.IsNullOrWhiteSpace(city))
            {
                Toast.MakeText(Application.Context, "Please provide LASTNAME", ToastLength.Short).Show();
                return;
            }

            OnSaveProfileDetails.Invoke(this, new OnSaveProfileDetailsEventArgs { firstname = firstname, city = city, email = email, lastname = lastname });
            this.Dismiss();
        }
    }
}