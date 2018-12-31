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
using UK.CO.Chrisjenx.Calligraphy;
using Plugin.Connectivity;
using System.Threading.Tasks;
using Acxi.Helpers;

namespace AcxiDriver.Activities
{
    [Activity(Label = "RegCompletedActivity", Theme ="@style/AcxiTheme1")]
    public class RegCompletedActivity : AppCompatActivity
    {
        string regcompleted = "Registration is complete, You will be noified and able to use the app as soon as your account verification is successful";
        string suspended = "Your account is currently suspended from Acxi";
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        string status = "";
        TextView txtbody;
        ImageView img;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
    .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
 .SetFontAttrId(Resource.Attribute.fontPath)
 .Build());

            SetContentView(Resource.Layout.regcompleted);
            Button btncompleted = (Button)FindViewById(Resource.Id.btnregcompleted);
            txtbody = (TextView)FindViewById(Resource.Id.txtbody_regcompleted);
            img = (ImageView)FindViewById(Resource.Id.img_regcompleted);

            status = Intent.GetStringExtra("status");
            if(status == "suspended")
            {
                txtbody.Text = suspended;
                img.SetImageResource(Resource.Drawable.cancel_icon);
            }
            else if(status == "pending")
            {
                txtbody.Text = regcompleted;
                img.SetImageResource(Resource.Drawable.ic_checked);

            }
            edit = pref.Edit();
            btncompleted.Click += Btncompleted_Click;
        }

        private async void Btncompleted_Click(object sender, EventArgs e)
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                ProgressDialog progress = new ProgressDialog(this);
                progress.SetCancelable(false);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Please wait...");
                string newstatus = "";
                string phone = pref.GetString("phone", "");
                progress.Show();

                await Task.Run(() =>
                {
                    WebRequestHelpers webhelper = new WebRequestHelpers();
                    newstatus = webhelper.CheckAccountStatus(phone);
                    char[] MyChar = { '"' };
                    newstatus = newstatus.Trim(MyChar);
                    edit.PutString("account_status", newstatus);
                    edit.Apply();
                });
                progress.Dismiss();

                if (newstatus == "suspended")
                {
                    txtbody.Text = suspended;
                    img.SetImageResource(Resource.Drawable.cancel_icon);
                }
                else if (newstatus == "approved")
                {
                    Toast.MakeText(this, "Your account has been approved", ToastLength.Short).Show();
                    Intent intent = new Intent(this, typeof(AppMainActivity));
                    edit.PutString("login", "true");
                    edit.PutString("account_status", newstatus);
                    edit.Apply();
                    StartActivity(intent);
                    Finish();
                }
                else if (newstatus == "pending")
                {
                    txtbody.Text = regcompleted;
                    img.SetImageResource(Resource.Drawable.ic_checked);
                }
            }
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}