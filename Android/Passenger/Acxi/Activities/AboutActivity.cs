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
using Calligraphy;

namespace Acxi.Activities
{
    [Activity(Label = "AboutActivity", Theme = "@style/AcxiTheme1", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class AboutActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar toolbar;
        LinearLayout lay_terms;
        LinearLayout lay_facebook;
        LinearLayout lay_twitter;
        LinearLayout lay_googleplay;
        LinearLayout lay_feedback;
        LinearLayout lay_acxi;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
   
                .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .Build());
            

            SetContentView(Resource.Layout.about);
            toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.aboutToolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "About";

            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);
            // Create your application here

            lay_acxi = (LinearLayout)FindViewById(Resource.Id.lay_aboutvisit);
            lay_feedback = (LinearLayout)FindViewById(Resource.Id.lay_aboutfeedback);
            lay_googleplay = (LinearLayout)FindViewById(Resource.Id.lay_about_rateus);
            lay_terms = (LinearLayout)FindViewById(Resource.Id.lay_about_terms);
            lay_twitter = (LinearLayout)FindViewById(Resource.Id.lay_about_twitter);
            lay_facebook = (LinearLayout)FindViewById(Resource.Id.lay_about_facebook);

            lay_acxi.Click += Lay_acxi_Click;
            lay_facebook.Click += Lay_facebook_Click;
            lay_feedback.Click += Lay_feedback_Click;
            lay_googleplay.Click += Lay_googleplay_Click;
            lay_terms.Click += Lay_terms_Click;
            lay_twitter.Click += Lay_twitter_Click;
        }

        private void Lay_twitter_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://www.twitter.com/kinitaxi");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Lay_terms_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Lay_googleplay_Click(object sender, EventArgs e)
        {
            string appPackageName = PackageManager.GetPackageInfo(PackageName, 0).PackageName.ToString();
            Android.Net.Uri market_uri = Android.Net.Uri.Parse("market://details?id=" + appPackageName);
            Android.Net.Uri store_uri = Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=" + appPackageName);

            try
            {
                Intent market = new Intent(Intent.ActionView, market_uri);
                StartActivity(market);
            }
            catch
            {
                Intent store = new Intent(Intent.ActionView, store_uri);
                StartActivity(store);
            }
        }

        private void Lay_feedback_Click(object sender, EventArgs e)
        {            
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Lay_facebook_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://m.facebook.com/kinitaxi/");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Lay_acxi_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com");
            var intent = new Intent(Intent.ActionView, uri);
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

       

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}