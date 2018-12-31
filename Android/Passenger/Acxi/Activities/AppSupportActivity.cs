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
using Calligraphy;
using FR.Ganfra.Materialspinner;
using Android.Support.Design.Widget;
using Acxi.Helpers;
using Java.Util;
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
using System.Threading;

namespace Acxi.Activities
{
    [Activity(Label = "AppSupportActivity", Theme ="@style/AcxiTheme1" , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class AppSupportActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar mtoolbar;
        MaterialSpinner spinnercategory;
        TextInputLayout txttitle;
        TextInputLayout txtmessage;
        Button btnsend;

        List<string> ListCategory;
        ArrayAdapter<string> adapter;
        string supportcategory = "";
        HelperFunctions helpers = new HelperFunctions();
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        FirebaseDatabase database;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());

            SetContentView(Resource.Layout.support);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.supportToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Contact support";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);

            spinnercategory = (MaterialSpinner)FindViewById(Resource.Id.spinner_support);
            txttitle = (TextInputLayout)FindViewById(Resource.Id.txttitle_support);
            txtmessage = (TextInputLayout)FindViewById(Resource.Id.txtmessage_support);
            btnsend = (Button)FindViewById(Resource.Id.btnsend_support);

            InitCategory();
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, ListCategory);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnercategory.Adapter = adapter;

            spinnercategory.ItemSelected += Spinnercategory_ItemSelected;
                btnsend.Click += Btnsend_Click;
         
        }

        private void Spinnercategory_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {           
            if(e.Position != -1)
            {
                string category = ListCategory[e.Position];
                supportcategory = category;
            }
        }

     

        private async void Btnsend_Click(object sender, EventArgs e)
        {
            string message = txtmessage.EditText.Text;
            string title = txttitle.EditText.Text;

            if (string.IsNullOrEmpty(supportcategory))
            {
                Toast.MakeText(this, "Please select the category of your message", ToastLength.Short).Show();
                return;
            }
           else if (string.IsNullOrEmpty(title))
            {
                Toast.MakeText(this, "Please enter the subject of the message", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(message))
            {
                Toast.MakeText(this, "Please enter your message", ToastLength.Short).Show();
                return;
            }
            double timestamp = helpers.GetTimeStampNow();
            string status = "active";
            string phone = pref.GetString("phone", "");
            string ticket_id = helpers.GenerateRandomString(10);
            HashMap map = new HashMap();
            map.Put("message", message);
            map.Put("title", title);
            map.Put("created_at", timestamp.ToString());
            map.Put("status", status);
            map.Put("user_id", phone);
            map.Put("category", supportcategory);

            try
            {
                database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
                DatabaseReference supportref = database.GetReference("userSupport/" + ticket_id);
                supportref.SetValue(map);
                DatabaseReference userspportref = database.GetReference("users/" + phone + "/tickets/" + ticket_id);

                userspportref.SetValue(true);
                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Please wait...");
                progress.SetCancelable(false);
                progress.Show();
                await Task.Run(() =>
                {
                    Thread.Sleep(2000);
                });
                progress.Dismiss();
                Toast.MakeText(this, "Your message was successfully sent", ToastLength.Short).Show();
                txttitle.EditText.Text = "";
                txtmessage.EditText.Text = "";
            }
            catch
            {

            }
        }


        private void InitCategory()
        {
            ListCategory = new List<string>();
            ListCategory.Add("Technical issues");
            ListCategory.Add("Unknown charges");
            ListCategory.Add("Others");
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