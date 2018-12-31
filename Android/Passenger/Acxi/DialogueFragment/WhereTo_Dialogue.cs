using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Acxi.DataModels;
using Newtonsoft.Json;
using Acxi.Adapter;

namespace Acxi.DialogueFragment
{
    
    public class WhereTo_Dialogue : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler<OnSavePlaceEventArgs> SelectPlace;
        public event EventHandler CloseDialogue;
        TextView txtempty;
        RecyclerView recy;
        Button btnclose;
        List<SavedPlaceItem> Data = new List<SavedPlaceItem>();
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        WhereToAdapter mAdapter;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.favouriteplace_dialogue, container, false);
            recy = (RecyclerView)view.FindViewById(Resource.Id.recywhereto);
            txtempty = (TextView)view.FindViewById(Resource.Id.txtnoplaces);
            btnclose = (Button)view.FindViewById(Resource.Id.btnclose_whereto);

            btnclose.Click += Btnclose_Click;
            CreateData();
            if(Data.Count > 0)
            {
                recy.Visibility = ViewStates.Visible;
                txtempty.Visibility = ViewStates.Gone;
            }
            else
            {
                txtempty.Visibility = ViewStates.Visible;
                recy.Visibility = ViewStates.Gone;

            }
            SetupRecy();
            return view;
        }

        private void Btnclose_Click(object sender, EventArgs e)
        {
            CloseDialogue.Invoke(this, new EventArgs());
        }

        private void SetupRecy()
        {
            mAdapter = new WhereToAdapter(Data);
            recy.SetLayoutManager(new LinearLayoutManager(recy.Context));
            recy.SetAdapter(mAdapter);
            mAdapter.ItemClick += MAdapter_ItemClick;
        }

        private void MAdapter_ItemClick(object sender, WhereToAdapterClickEventArgs e)
        {
            var item = Data[e.Position];
            SelectPlace.Invoke(this, new OnSavePlaceEventArgs { address = item.address, latitude = item.latitude, longitude = item.longitude, title = item.title });
        }

        void CreateData()
        {
           
            //Data.Add(new SavedPlaceItem { title = "Home", address = "SPAR Port Harcourt", latitude = 0.65645, longitude = -6.8978 });
            //Data.Add(new SavedPlaceItem { title = "Work", address = "Port Harcourt Pleasure park", latitude = 0.65645, longitude = -6.8978 });
            //Data.Add(new SavedPlaceItem { title = "School", address = "University of Port Harcourt", latitude = 0.65645, longitude = -6.8978 });
            //Data.Add(new SavedPlaceItem { title = "Fun place", address = "House of Suya, VGC Lekki", latitude = 0.65645, longitude = -6.8978 });



            string homeaddress = pref.GetString("homeaddress", "");
            string homelat = pref.GetString("homelat", "");
            string homelng = pref.GetString("homelng", "");
            if (!string.IsNullOrEmpty(homeaddress) && !string.IsNullOrEmpty(homelat) && !string.IsNullOrEmpty(homelng))
            {
                Data.Add(new SavedPlaceItem { title = "Home", address = homeaddress, latitude = double.Parse(homelat), longitude = double.Parse(homelng) });
            }
           

            string workaddress = pref.GetString("workaddress", "");
            string worklat = pref.GetString("worklat", "");
            string worklng = pref.GetString("worklng", "");
            if (!string.IsNullOrEmpty(workaddress) && !string.IsNullOrEmpty(worklat) && !string.IsNullOrEmpty(worklng))
            {
                Data.Add(new SavedPlaceItem { title = "Work", address = workaddress, latitude = double.Parse(worklat), longitude = double.Parse(worklng) });
            }
           
            string jsonstr = pref.GetString("savedplaces", "");
            if (!string.IsNullOrEmpty(jsonstr))
            {
                var deser = JsonConvert.DeserializeObject<List<SavedPlaceItem>>(jsonstr);
                foreach (SavedPlaceItem item in deser)
                {
                    Data.Add(item);
                }
            }
        }

    }
}