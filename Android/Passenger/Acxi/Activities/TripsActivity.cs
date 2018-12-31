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
using Acxi.DataModels;
using Acxi.Adapter;
using Calligraphy;
using Newtonsoft.Json;
using Acxi.Helpers;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Acxi.DialogueFragment;

namespace Acxi.Activities
{
    [Activity(Label = "TripsActivity", Theme = "@style/AcxiTheme1", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TripsActivity : AppCompatActivity
    {
        SupportWidget.RecyclerView triprecy;
        Android.Support.V7.Widget.Toolbar mtoolbar;
        List<rideHistory> mTripData;
        TripAdapter mAdapter;
        TextView txthistorycheck;
        ImageView imgmoto;
        Button btn_noride;
        RelativeLayout lay_noride;
        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
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
            btn_noride = (Button)FindViewById(Resource.Id.btn_noride);
            lay_noride = (RelativeLayout)FindViewById(Resource.Id.lay_noride);

            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Your trips";

            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);

            triprecy = (SupportWidget.RecyclerView)FindViewById(Resource.Id.tripsrecycler);
            phone = pref.GetString("phone", "");
            edit = pref.Edit();
          
            triprecy.Visibility = ViewStates.Invisible;
            lay_noride.Visibility = ViewStates.Gone;
            btn_noride.Click += Btn_noride_Click;
            CreateData();

            // Create your application here
        }

        private void Btn_noride_Click(object sender, EventArgs e)
        {
            this.Finish();
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

        public void AlertInit(string body)
        {
            AppAlertDialogue appAlert = new AppAlertDialogue(body);
            appAlert.Cancelable = true;
            var trans1 = SupportFragmentManager.BeginTransaction();
            appAlert.Show(trans1, "appalert");

            appAlert.AlertCancel += (i, h) =>
            {
                appAlert.Dismiss();
                appAlert = null;
            };

            appAlert.AlertOk += (y, t) =>
            {
                appAlert.Dismiss();
                appAlert = null;
            };

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
                       try
                       {
                           jstr = webhelpers.GetRideHistory(phone, "all");
                       }
                       catch
                       {
                           triprecy.Visibility = ViewStates.Invisible;
                           txthistorycheck.Visibility = ViewStates.Visible;
                          imgmoto.Visibility = ViewStates.Visible;
                          txthistorycheck.Text = "No internet connectivity";
                       }
                   });
                    progress.Dismiss();

                    if (!jstr.Contains ("no_ride"))
                    {
                        mTripData = JsonConvert.DeserializeObject<List<rideHistory>>(jstr);
                        mTripData = mTripData.OrderByDescending(o => o.created_at).ToList();

                        var Ddata = mTripData;
                        for (int i = 0; i < mTripData.Count; i++)
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
                        txthistorycheck.Visibility = ViewStates.Invisible;
                        imgmoto.Visibility = ViewStates.Invisible;

                        SetupRecyclerView(triprecy);
                        edit.PutString("ride_history", jstr);
                        edit.Apply();
                    }
                    else
                    {
                        triprecy.Visibility = ViewStates.Gone;
                        lay_noride.Visibility = ViewStates.Visible;
                    }

                }
                else
                {
                    jstr = pref.GetString("ride_history", "");
                    if (!string.IsNullOrEmpty(jstr))
                    {
                        mTripData = JsonConvert.DeserializeObject<List<rideHistory>>(jstr);
                        mTripData = mTripData.OrderByDescending(o => o.created_at).ToList();
                        if (mTripData.Count > 0)
                        {
                            triprecy.Visibility = ViewStates.Visible;
                            lay_noride.Visibility = ViewStates.Gone;
                            SetupRecyclerView(triprecy);
                        }
                        else
                        {
                            triprecy.Visibility = ViewStates.Gone;
                            lay_noride.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        triprecy.Visibility = ViewStates.Gone;
                        lay_noride.Visibility = ViewStates.Visible;

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
            string data = JsonConvert.SerializeObject(mTripData[e.Position]);
            Intent intent = new Intent(this, typeof(TripsDetailActivity));
            intent.PutExtra("data", data);
            StartActivity(intent);
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}