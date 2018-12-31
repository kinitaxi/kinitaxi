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
using Acxi.Helpers;

namespace Acxi.DialogueFragment
{
    public class OTPValueEventArgs : EventArgs
    {
        public string status_value { get; set; }
    }
    public class OtpDialogue : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler<OTPValueEventArgs> OnOtpProvided;
        TextInputLayout txtotp;
        TextView txtheader;
        TextView txtdialogue_body;
        Button btnfinalize;
        string dialogue_type;
        double mAmount;

        HelperFunctions helpers = new HelperFunctions();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.otp_dialogue, container, false);
            txtotp = (TextInputLayout)view.FindViewById(Resource.Id.txtotp_otpdialogue);
            txtdialogue_body = (TextView)view.FindViewById(Resource.Id.txtamount_otpdialogue);
            txtheader = (TextView)view.FindViewById(Resource.Id.txtheader_otpdialogue);
            btnfinalize = (Button)view.FindViewById(Resource.Id.btnfinish_otpdialogue);
            if (dialogue_type == "send_otp")
            {
                txtdialogue_body.Text = "Please provide the OTP sent to your phone number";
                txtotp.EditText.Hint = "OTP";
            }
            else if(dialogue_type == "send_phone")
            {
                txtdialogue_body.Text = "Please provide the Phone Number attached to this card";
                txtotp.EditText.Hint = "Phone";
                
            }
            else if(dialogue_type == "send_birthday")
            {
                txtdialogue_body.Text = "Please provide your birthday on this account";
                txtotp.EditText.Hint = "Birthday";
            }
            btnfinalize.Click += Btnfinalize_Click;
            return view;
        }

        private void Btnfinalize_Click(object sender, EventArgs e)
        {
            if(txtotp.EditText.Text.Length > 4)
            {
                OnOtpProvided.Invoke(Application.Context, new OTPValueEventArgs { status_value = txtotp.EditText.Text });
                this.Dismiss();
            }
            else
            {
                Toast.MakeText(Application.Context, "Please provide the required input", ToastLength.Short).Show();
            }
        }

        public OtpDialogue(string dtype)
        {
            dialogue_type = dtype;
        }

    }
}