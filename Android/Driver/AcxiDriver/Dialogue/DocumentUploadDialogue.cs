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

namespace AcxiDriver.Dialogue
{
    public class DocumentUploadDialogue : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler TakeAPhoto;
        public event EventHandler UploadAPhoto;

        string whichdialogue = "";
        string license_body = "To ensure that your documents are accepted please take note of the following,\n1. Ensure that information on the document is readable. \n2. Ensure that all corners of the document is visible; \n3. Ensure that your document is valid and has not expired.  \n4. Ensure that the name on your license exactly matches your ACXI registered name";
        string worthiness_body = "To ensure that your documents are accepted please take note of the following,\n1. Ensure that information on the document is readable. \n2. Ensure that all corners of the document is visible; \n3. Ensure that your document is valid and has not expired.";
        string profile_body = "Ensure your face is showing clearly in the headshot image. This will serve as our profile picture";
        TextView txttake;
        TextView txtupload;
        TextView txtbody;
        TextView txtheader;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.uploaddocument_dialogue, container, false);
            txtbody = (TextView)view.FindViewById(Resource.Id.txtbody_documentupload);
            txtupload = (TextView)view.FindViewById(Resource.Id.txtupload_documentupload);
            txttake = (TextView)view.FindViewById(Resource.Id.txttake_documentupload);
            txtheader = (TextView)view.FindViewById(Resource.Id.txtheader_documentupload);
            SetTextView();
            txttake.Click += Txttake_Click;
            txtupload.Click += Txtupload_Click;
            return view;
        }

        private void Txtupload_Click(object sender, EventArgs e)
        {
            UploadAPhoto.Invoke(this, new EventArgs());
            this.Dismiss();
        }

        private void Txttake_Click(object sender, EventArgs e)
        {
            TakeAPhoto.Invoke(this, new EventArgs());
            this.Dismiss();
        }

        private void SetTextView()
        {
            if(whichdialogue == "license")
            {
                txtbody.Text = license_body;
                txtheader.Text = "Drivers License";
            }
            else if(whichdialogue == "worthiness")
            {
                txtheader.Text = "Road Worthiness Cert.";
                txtbody.Text = worthiness_body;
            }
            else if(whichdialogue == "profile")
            {
                txtbody.Text = profile_body;
                txtheader.Text = "Profile Picture";
            }
        }

        public DocumentUploadDialogue (string dialogue)
        {
            whichdialogue = dialogue;
        }
    }
}