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
using AcxiDriver.DataModels;
using AcxiDriver.Adapter;
using Newtonsoft.Json;

namespace AcxiDriver.Activities
{
    [Activity(Label = "FeedbackActivity", Theme = "@style/AcxiTheme1")]
    public class FeedbackActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.RecyclerView feedbackrecy;
        Android.Support.V7.Widget.Toolbar mtoolbar;
        List<Rating_Status> mData;
        FeedbackAdapter mAdapter;
        string phone;
        TextView txtfeedbackcheck;
        RelativeLayout lay_nofeedback;
        Button btn_nofeedback;

        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
              .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
              .SetFontAttrId(Resource.Attribute.fontPath)
              .Build());
            SetContentView(Resource.Layout.feedback);

            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.feedbackToolbar);
            txtfeedbackcheck = (TextView)FindViewById(Resource.Id.txtfeedcheck);
            lay_nofeedback = (RelativeLayout)FindViewById(Resource.Id.lay_nofeedback);
            btn_nofeedback = (Button)FindViewById(Resource.Id.btn_nofeedback);
            btn_nofeedback.Click += Btn_nofeedback_Click;

            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Feedbacks";

            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_arrow_back_white);

            feedbackrecy = (Android.Support.V7.Widget.RecyclerView)FindViewById(Resource.Id.feedbackrecycler);
            phone = pref.GetString("phone", "");
            edit = pref.Edit();

            lay_nofeedback.Visibility = ViewStates.Gone;
            feedbackrecy.Visibility = ViewStates.Gone;

            CreateData();
        }

        private void Btn_nofeedback_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void CreateData()
        {
            string jstr = pref.GetString("feedback", "");
            if (!string.IsNullOrEmpty(jstr))
            {
                mData = JsonConvert.DeserializeObject<List<Rating_Status>>(jstr);

                var Ddata = mData;
                for (int i = 0; i < mData.Count; i++)
                {
                    Console.WriteLine("created_at = " + Ddata[i].created_at);
                    if (string.IsNullOrEmpty(Ddata[i].created_at))
                    {
                        Ddata.RemoveAt(i);
                        i -= 1;
                    }
                }


                mData = Ddata;
                if (mData.Count > 0)
                {
                    SetupRecyclerView();
                }
                else
                {
                    lay_nofeedback.Visibility = ViewStates.Visible;
                    feedbackrecy.Visibility = ViewStates.Gone;
                }
            }
            else
            {
                lay_nofeedback.Visibility = ViewStates.Visible;
                feedbackrecy.Visibility = ViewStates.Gone;
            }
        }

        private void SetupRecyclerView()
        {
            feedbackrecy.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(feedbackrecy.Context));
            mAdapter = new FeedbackAdapter(mData, this);
            feedbackrecy.SetAdapter(mAdapter);
            lay_nofeedback.Visibility = ViewStates.Gone;
            feedbackrecy.Visibility = ViewStates.Visible;
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