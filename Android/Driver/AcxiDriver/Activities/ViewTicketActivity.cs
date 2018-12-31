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
using Android.Support.V7.Widget;
using AcxiDriver.DataModels;
using AcxiDriver.Adapter;
using Newtonsoft.Json;

namespace AcxiDriver.Activities
{
    [Activity(Label = "ViewTicketActivity", Theme ="@style/AcxiTheme1")]
    public class ViewTicketActivity : AppCompatActivity
    {
        Android.Support.V7.Widget.Toolbar mtoolbar;
        RecyclerView recy;
        List<TicketDetailsFull> mData;
        TicketAdapter mAdapter;
        TextView txttripcheck;
        RelativeLayout lay_noticket;
        Button btn_noticket;

        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
    .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
 .SetFontAttrId(Resource.Attribute.fontPath)
 .Build());

            SetContentView(Resource.Layout.viewtickets);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.viewticketToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Tickets";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_arrow_back_white);

            recy = (RecyclerView)FindViewById(Resource.Id.ticketsrecycler);
            lay_noticket = (RelativeLayout)FindViewById(Resource.Id.lay_noticket);
            btn_noticket = (Button)FindViewById(Resource.Id.btn_noticket);
            btn_noticket.Click += Btn_noticket_Click;

            createData();
            if(mData.Count > 0)
            {
                SetupRecycler(recy);
                recy.Visibility = ViewStates.Visible;
                lay_noticket.Visibility = ViewStates.Gone;
            }
            else
            {
                recy.Visibility = ViewStates.Gone;
                lay_noticket.Visibility = ViewStates.Visible;
            }
        }

        private void Btn_noticket_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void SetupRecycler(RecyclerView recy)
        {
            recy.SetLayoutManager(new Android.Support.V7.Widget.LinearLayoutManager(recy.Context));
            mAdapter = new TicketAdapter(mData);
            recy.SetAdapter(mAdapter);
            mAdapter.ItemClick += MAdapter_ItemClick;
        }

        private void MAdapter_ItemClick(object sender, TicketAdapterClickEventArgs e)
        {
           //
        }

        private void createData()
        {
            string jstr = pref.GetString("support", "");
            if (!string.IsNullOrEmpty(jstr))
            {
                mData = JsonConvert.DeserializeObject<List<TicketDetailsFull>>(jstr);
               
            }
            else
            {
                mData = new List<TicketDetailsFull>();
            }
           
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