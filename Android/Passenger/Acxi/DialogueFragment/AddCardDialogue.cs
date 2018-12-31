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

namespace Acxi.DialogueFragment
{
    public class OnSaveCardEventArgs: EventArgs
    {
        public string cardnumber { get; set; }
        public string expyear { get; set; }
        public string expmonth { get; set; }
        public string cvv { get; set; }
        public string pin { get; set; }
    }
    public class AddCardDialogue : Android.Support.V4.App.DialogFragment
    {
        TextInputLayout txtcardnum;
        TextInputLayout txtexp;
        TextInputLayout txtcvv;
        TextInputLayout txtpin;
        Button btnsavecard;
        string lasttext = "";

        public event EventHandler<OnSaveCardEventArgs> SaveCardDetails;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.addcard_dialogue, container, false);
            txtcardnum = (TextInputLayout)view.FindViewById(Resource.Id.txtcardnum);
            txtexp = (TextInputLayout)view.FindViewById(Resource.Id.txtcardexp);
            txtcvv = (TextInputLayout)view.FindViewById(Resource.Id.txtcardcvv);
            txtpin = (TextInputLayout)view.FindViewById(Resource.Id.txtcardpin);
            btnsavecard = (Button)view.FindViewById(Resource.Id.btnsavecard);
            txtexp.EditText.BeforeTextChanged += EditText_BeforeTextChanged;
            txtexp.EditText.TextChanged += EditText_TextChanged;
            btnsavecard.Click += Btnsavecard_Click;
            return view;
        }

        private void EditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (txtexp.EditText.Text.Length == 2 && lasttext != "/")
            {
                txtexp.EditText.Text = txtexp.EditText.Text + "/";
                txtexp.EditText.SetSelection(3);
            }
            else if (txtexp.EditText.Text.Length == 2 && lasttext == "/")
            {
                txtexp.EditText.Text = txtexp.EditText.Text.Substring(0, 1);
                txtexp.EditText.SetSelection(1);
            }

        }

        private void EditText_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (txtexp.EditText.Text.Length != 0)
            {
                lasttext = txtexp.EditText.Text.Substring(txtexp.EditText.Text.Length - 1, 1);
            }
        }

        private void Btnsavecard_Click(object sender, EventArgs e)
        {
            string mcard = txtcardnum.EditText.Text;
            string mexp = txtexp.EditText.Text;
            string mcvv = txtcvv.EditText.Text;
            string mpin = txtpin.EditText.Text;
            string mexpyear = "", mexpmonth = "";
            if(mcard.Length != 16 && mcard.Length != 19)
            {
                Toast.MakeText(Application.Context, "Please provide a valid card number", ToastLength.Short).Show();
                return;
            }
            else if (mexp.Length != 5)
            {
                Toast.MakeText(Application.Context, "Please provide a valid expiry date", ToastLength.Short).Show();
                return;
            }
            else
            {
                mexpyear =  mexp.Substring(3, 2);
                mexpmonth = mexp.Substring(0, 2);
            }

            if(mcvv.Length != 3)
            {
                Toast.MakeText(Application.Context, "Please provide a valid CVV", ToastLength.Short).Show();
                return;
            }
            else if(mpin.Length != 4)
            {
                Toast.MakeText(Application.Context, "Please provide a PIN", ToastLength.Short).Show();
                return;
            }

            SaveCardDetails.Invoke(this, new OnSaveCardEventArgs { cardnumber = mcard, cvv = mcvv, expmonth = mexpmonth, expyear = mexpyear, pin = mpin });
        }
    }
}