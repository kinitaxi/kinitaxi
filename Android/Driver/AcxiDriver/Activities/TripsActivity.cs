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
using SupportWidget = Android.Support.V7.Widget;
using UK.CO.Chrisjenx.Calligraphy;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Plugin.Connectivity;
using UK.CO.Chrisjenx.Calligraphy;
using AcxiDriver.DataModels;
using Acxi.Helpers;
using AcxiDriver.Adapter;

namespace AcxiDriver.Activities
{
    [Activity(Label = "TripsActivity", Theme ="@style/AcxiTheme1")]
    public class TripsActivity : AppCompatActivity
    {
        SupportWidget.RecyclerView triprecy;
        Android.Support.V7.Widget.Toolbar mtoolbar;
        List<rideHistory> mTripData;
        TripAdapter mAdapter;
        TextView txthistorycheck;
        ImageView imgmoto;
        RelativeLayout lay_notrip;
        Button btn_notrip;
        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

    WebRequestHelpers webhelpers = new WebRequestHelpers();
        string phone = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
         .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
      .SetFontAttrId(Resource.Attribute.fontPath)
      .Build());
            SetContentView(Resource.Layout.trips);

            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.tripsToolbar);
            txthistorycheck = (TextView)FindViewById(Resource.Id.txttripcheck);
            imgmoto = (ImageView)FindViewById(Resource.Id.img_tripmoto);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Your trips";

            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_arrow_back_white);

            triprecy = (SupportWidget.RecyclerView)FindViewById(Resource.Id.tripsrecycler);
            lay_notrip = (RelativeLayout)FindViewById(Resource.Id.lay_notrip);
            btn_notrip = (Button)FindViewById(Resource.Id.btn_notrip);
            btn_notrip.Click += Btn_notrip_Click;

            phone = pref.GetString("phone", "");
            edit = pref.Edit();
          
          

            CreateData();

            // Create your application here
        }

        private void Btn_notrip_Click(object sender, EventArgs e)
        {
            Finish();
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
        private async void CreateData()
        {
            // DownloadFrom 
            mTripData = new List<rideHistory>();
            WebRequestHelpers webhelpers = new WebRequestHelpers();

            string jstr = "";
            if (!string.IsNullOrEmpty(phone))
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    ProgressDialog progress = new ProgressDialog(this);
                    progress.SetCancelable(false);
                    progress.SetMessage("Fetching history...");
                    progress.Indeterminate = true;
                    progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progress.Show();

                   await Task.Run(() =>
                   {
                        jstr = webhelpers.GetTripHistory(phone, "all");
                   });

                    progress.Dismiss();
                    if (!jstr.Contains("no_ride"))
                    {
                        mTripData = JsonConvert.DeserializeObject<List<rideHistory>>(jstr);
                        mTripData = mTripData.OrderByDescending(o => o.created_at).ToList();
                        var Ddata = mTripData;
                        for (int i = 0; i< mTripData.Count; i++)
                        {
                            Console.WriteLine("created_at = " + Ddata[i].created_at);
                            if (string.IsNullOrEmpty(Ddata[i].created_at) || string.IsNullOrEmpty(Ddata[i].status))
                            {
                                Ddata.RemoveAt(i);
                               i -= 1;
                            }
                        }

                        mTripData = Ddata;
                        triprecy.Visibility = ViewStates.Visible;
                        lay_notrip.Visibility = ViewStates.Gone;
                        SetupRecyclerView(triprecy);
                        edit.PutString("ride_history", JsonConvert.SerializeObject(mTripData));
                        edit.Apply();
                    }
                    else
                    {
                        triprecy.Visibility = ViewStates.Gone;
                        lay_notrip.Visibility = ViewStates.Visible;

                    }

                }
                else
                {
                    jstr = pref.GetString("ride_history", "");
                    if (!string.IsNullOrEmpty(jstr))
                    {
                        mTripData = JsonConvert.DeserializeObject<List<rideHistory>>(jstr);
                        if(mTripData.Count > 0)
                        {
                            triprecy.Visibility = ViewStates.Visible;
                            lay_notrip.Visibility = ViewStates.Gone;

                            SetupRecyclerView(triprecy);
                        }
                        else
                        {
                            triprecy.Visibility = ViewStates.Gone;
                            lay_notrip.Visibility = ViewStates.Visible;

                        }
                    }
                    else
                    {
                        triprecy.Visibility = ViewStates.Gone;
                        lay_notrip.Visibility = ViewStates.Visible;

                    }
                }
               
            }

        }

     
        private void SetupRecyclerView(SupportWidget.RecyclerView triprecy)
        {
            triprecy.SetLayoutManager(new SupportWidget.LinearLayoutManager(triprecy.Context));
            mAdapter = new TripAdapter(mTripData, this);
            triprecy.SetAdapter(mAdapter);
            mAdapter.ItemClick += MAdapter_ItemClick;
        }

        private void MAdapter_ItemClick(object sender, TripAdapterClickEventArgs e)
        {
            //string data = JsonConvert.SerializeObject(mTripData[e.Position]);
            //Intent intent = new Intent(this, typeof(TripsDetailActivity));
            //intent.PutExtra("data", data);
            //StartActivity(intent);
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}