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
using Acxi.Helpers;
using UK.CO.Chrisjenx.Calligraphy;
using System.Threading.Tasks;
using System.Threading;

namespace AcxiDriver.Activities
{
    [Activity(Label = "DocumentActivity", Theme ="@style/AcxiTheme1")]
    public class DocumentActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar mtoolbar;
        LinearLayout btnlicense;
        LinearLayout btnworthiness;
        ImageView imgdocument;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        HelperFunctions helpers = new HelperFunctions();
        WebRequestHelpers webHelpers = new WebRequestHelpers();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
              .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
              .SetFontAttrId(Resource.Attribute.fontPath)
              .Build());

            SetContentView(Resource.Layout.documentview);

            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.document_Toolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Documents";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_arrow_back_white);

            btnlicense = (LinearLayout)FindViewById(Resource.Id.laylicense_document);
            btnworthiness = (LinearLayout)FindViewById(Resource.Id.layworthiness_document);
            imgdocument = (ImageView)FindViewById(Resource.Id.imgdocumentview);

            edit = pref.Edit();
            btnlicense.Click += Btnlicense_Click;
            btnworthiness.Click += Btnworthiness_Click;
        }

        private async void Btnworthiness_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetTitle("Downloading document");
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();

            string base64 = pref.GetString("worthiness", "");
            if (!string.IsNullOrEmpty(base64))
            {
                helpers.SetProfileImage2(imgdocument, base64);
            }
            else
            {
                string imagestring = "";
                await Task.Run(() =>                 
                {
                    string worthiness_url = pref.GetString("worthiness_url", "");
                    if (!string.IsNullOrEmpty(worthiness_url))
                    {
                        imagestring = webHelpers.downloadImage(worthiness_url);
                        edit.PutString("worthiness", imagestring);
                        edit.Apply();
                    }
                });

                if (!string.IsNullOrEmpty(imagestring))
                {
                    helpers.SetProfileImage2(imgdocument, imagestring);
                }
            }
            progress.Dismiss();
        }

        private async void Btnlicense_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetTitle("Downloading document");
            progress.SetCancelable(false);
            progress.Show();

            string base64 = pref.GetString("license", "");
            if (!string.IsNullOrEmpty(base64))
            {
                helpers.SetProfileImage2(imgdocument, base64);
            }
            else
            {
                string imagestring = "";
                await Task.Run(() =>
                {
                    string license_url = pref.GetString("license_url", "");
                    if (!string.IsNullOrEmpty(license_url))
                    {
                        imagestring = webHelpers.downloadImage(license_url);
                        edit.PutString("license", imagestring);
                        edit.Apply();
                    }
                });

                if (!string.IsNullOrEmpty(imagestring))
                {
                    helpers.SetProfileImage2(imgdocument, imagestring);
                }
            }
            progress.Dismiss();
        }

        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}