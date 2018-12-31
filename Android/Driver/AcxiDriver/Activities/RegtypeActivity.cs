using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using UK.CO.Chrisjenx.Calligraphy;

namespace AcxiDriver.Activities
{
    [Activity(Label = "RegtypeActivity", Theme ="@style/AcxiTheme1")]
    public class RegtypeActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar mtoolbar;
        LinearLayout lay_regdriver;
        LinearLayout lay_regpartner;
        LinearLayout lay_regcarowner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
  .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
.SetFontAttrId(Resource.Attribute.fontPath)
.Build());

            SetContentView(Resource.Layout.regtype);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.regtypeToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Register with Acxi";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_arrow_back_white);

            lay_regdriver = (LinearLayout)FindViewById(Resource.Id.lay_regdriver);
            lay_regdriver.Click += Lay_regdriver_Click;
            // Create your application here
        }

        private void Lay_regdriver_Click(object sender, EventArgs e)
        {
            string phone = Intent.GetStringExtra("phone");
            Intent intent = new Intent(this, typeof(RegistrationActivity));
            intent.PutExtra("phone", phone);
            StartActivity(intent);
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}