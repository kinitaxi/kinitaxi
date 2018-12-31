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
    public class OnSaveRegProfileEventArgs : EventArgs
    {
        public string mfirstname { get; set; }
        public string mlastname { get; set; }
        public string memail { get; set; }
        public string mcity { get; set; }
        public string mphone { get; set; }
        public string minvitecode { get; set; }
    }
    public class PersonalProfileRegistrationDialogue : Android.Support.V4.App.DialogFragment
    {
        string phoneID = "";
        TextInputLayout txtphone;
        TextInputLayout txtfirstname;
        TextInputLayout txtlastname;
        TextInputLayout txtcity;
        TextInputLayout txtemail;
        TextInputLayout txtinvite;
        Button btnsave;
        public event EventHandler<OnSaveRegProfileEventArgs> SavePersonalDetails;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.personaldetails_reg, container, false);
            txtphone = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutphone_reg);
            txtfirstname = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutfirstname_reg);
            txtlastname = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutlastname_reg);
            txtemail = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutemail_reg);
            txtcity = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutcity_reg);
            txtinvite = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutinvite_reg);
            btnsave = (Button)view.FindViewById(Resource.Id.btnsavepersonal_reg);

            btnsave.Click += Btnsave_Click;
            txtphone.EditText.Text = phoneID;
            txtphone.EditText.Enabled = false;
            return view;
        }

        private void Btnsave_Click(object sender, EventArgs e)
        {
            string firstname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtfirstname.EditText.Text.Trim()) ;
            string lastname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtlastname.EditText.Text.Trim());
            string email = txtemail.EditText.Text.Trim();
            string city = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtcity.EditText.Text.Trim()) ;
            string phone = txtcity.EditText.Text;
            string invitecode = txtinvite.EditText.Text.Trim();

            if (string.IsNullOrEmpty(firstname)|| string.IsNullOrWhiteSpace(firstname))
            {
                Toast.MakeText(Application.Context, "Please provide FIRSTNAME", ToastLength.Short).Show();
                return;
            }
           else  if (string.IsNullOrEmpty(lastname) || string.IsNullOrWhiteSpace(lastname))
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

            SavePersonalDetails.Invoke(this, new OnSaveRegProfileEventArgs { mcity = city, memail = email, mfirstname = firstname, mlastname = lastname, mphone = phone, minvitecode = invitecode });
            this.Dismiss();
        }

        public PersonalProfileRegistrationDialogue(string phone)
        {
            phoneID = phone;
        }
    }
}