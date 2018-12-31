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
using Android.Support.V7.Widget;
using Calligraphy;

namespace Acxi.Activities
{
    [Activity(Label = "HelpActivity", Theme = "@style/AcxiTheme1")]
    public class HelpActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar mtoolbar;
        LinearLayout laysupport;
        LinearLayout laybilling;
        LinearLayout layuseacxi;
        LinearLayout laypayment;
        LinearLayout laypromo;
        LinearLayout layterms;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                     .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                   .SetFontAttrId(Resource.Attribute.fontPath)
                  .Build());

            SetContentView(Resource.Layout.help);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.helpToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Help";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);
            laysupport = (LinearLayout)FindViewById(Resource.Id.layhelp_support);
            laybilling = (LinearLayout)FindViewById(Resource.Id.layhelp_cardbilling);
            layuseacxi = (LinearLayout)FindViewById(Resource.Id.layhelp_useacxi);
            laypayment = (LinearLayout)FindViewById(Resource.Id.layhelp_payment);
            laypromo = (LinearLayout)FindViewById(Resource.Id.layhelp_promo);
            layterms = (LinearLayout)FindViewById(Resource.Id.layhelp_terms);

            laysupport.Click += Laysupport_Click;
            layuseacxi.Click += Layuseacxi_Click;
            laypromo.Click += Laypromo_Click;
            layterms.Click += Layterms_Click;
            laybilling.Click += Laybilling_Click;
            // Create your application here
        }

        private void Layterms_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com/");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Laypromo_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com/");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Layuseacxi_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com/");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Laybilling_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com/");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Laysupport_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(AppSupportActivity));
            StartActivity(intent);
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
        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }
    }
}