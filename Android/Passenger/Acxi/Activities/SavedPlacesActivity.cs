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
using Acxi.DataModels;
using Android.Support.V7.Widget;
using Acxi.Adapter;
using Calligraphy;
using Acxi.DialogueFragment;
using Newtonsoft.Json;
using Firebase.Database;
using Firebase;

namespace Acxi.Activities
{
    [Activity(Label = "SavedPlaces", Theme ="@style/AcxiTheme1")]
    public class SavedPlacesActivity : AppCompatActivity
    {
        List<SavedPlaceItem> Data;
        List<SavedPlaceItem> SavedData = new List<SavedPlaceItem>();
        Android.Support.V7.Widget.Toolbar mtoolbar;
        PlacesAdapter mAdapter;
        RecyclerView recy;
        AddNewPlace_Dialogue addplace_dialogue;
        
        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        FirebaseDatabase database;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
      .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
   .SetFontAttrId(Resource.Attribute.fontPath)
   .Build());
            SetContentView(Resource.Layout.savedplaces);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.savedToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Saved places";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);

            recy = (RecyclerView)FindViewById(Resource.Id.recysavedplace);
            edit = pref.Edit();
            CreateData();
            SetupRecy(recy);
           
        }

        private void SetupRecy(RecyclerView recy)
        {
            mAdapter = new PlacesAdapter(Data);
            recy.SetLayoutManager(new LinearLayoutManager(recy.Context));
            recy.SetAdapter(mAdapter);
            mAdapter.ItemRemove += MAdapter_ItemRemove;
        }

        private void MAdapter_ItemRemove(object sender, PlacesAdapterClickEventArgs e)
        {
            Data.RemoveAt(e.Position);
            SavedData.RemoveAt(e.Position - 2);
            mAdapter.NotifyDataSetChanged();
            Toast.MakeText(this, "Place removed succesfully", ToastLength.Short).Show();
            //string contactjson = JsonConvert.SerializeObject(DataList);
            //edit.PutString("contacts", contactjson);
            //edit.Apply();
        }

        void CreateData()
        {
            Data = new List<SavedPlaceItem>();
            //Data.Add(new SavedPlaceItem { title = "Home", address = "SPAR Port Harcourt", latitude = 0.65645, longitude = -6.8978 });
            //Data.Add(new SavedPlaceItem { title = "Work", address = "Port Harcourt Pleasure park", latitude = 0.65645, longitude = -6.8978 });
            //Data.Add(new SavedPlaceItem { title = "School", address = "University of Port Harcourt", latitude = 0.65645, longitude = -6.8978 });
            //Data.Add(new SavedPlaceItem { title = "Fun place", address = "House of Suya, VGC Lekki", latitude = 0.65645, longitude = -6.8978 });

           

            string homeaddress = pref.GetString("homeaddress", "");
            string homelat = pref.GetString("homelat", "");
            string homelng = pref.GetString("homelng", "");

            if(!string.IsNullOrEmpty(homeaddress) && !string.IsNullOrEmpty(homelat) && !string.IsNullOrEmpty(homelng))
            {
                Data.Add(new SavedPlaceItem { title = "Home", address = homeaddress, latitude = double.Parse(homelat), longitude = double.Parse(homelng) });
            }
            else
            {
                Data.Add(new SavedPlaceItem { title = "Home", address = "Set home address on your profile"});
            }

            string workaddress = pref.GetString("workaddress", "");
            string worklat = pref.GetString("worklat", "");
            string worklng = pref.GetString("worklng", "");

            if (!string.IsNullOrEmpty(workaddress) && !string.IsNullOrEmpty(worklat) && !string.IsNullOrEmpty(worklng))
            {
                Data.Add(new SavedPlaceItem { title = "Work", address = workaddress, latitude = double.Parse(worklat), longitude = double.Parse(worklng) });
            }
            else
            {
                Data.Add(new SavedPlaceItem { title = "Work", address = "Set work address on your profile" });
            }

            string jsonstr = pref.GetString("savedplaces", "");
            if (!string.IsNullOrEmpty(jsonstr))
            {
                var deser = JsonConvert.DeserializeObject<List<SavedPlaceItem>>(jsonstr);
                foreach(SavedPlaceItem item in deser)
                {
                    Data.Add(item);
                    SavedData.Add(item);
                }
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.savedplaces_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.Finish();
                    return true;
                case Resource.Id.btnaddnew:
                    AddNewPlace();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void AddNewPlace()
        {
            addplace_dialogue = new AddNewPlace_Dialogue();
            var trans = SupportFragmentManager.BeginTransaction();
            addplace_dialogue.Show(trans, "addplace");
            addplace_dialogue.SavePlace += Addplace_dialogue_SavePlace;
        }

        private void Addplace_dialogue_SavePlace(object sender, OnSavePlaceEventArgs e)
        {
            if(addplace_dialogue != null)
            {
                Data.Add(new SavedPlaceItem { title = e.title, address = e.address, latitude = e.latitude, longitude = e.longitude });
                SavedData.Add(new SavedPlaceItem { title = e.title, address = e.address, latitude = e.latitude, longitude = e.longitude });
                mAdapter.NotifyDataSetChanged();
                var savejson = JsonConvert.SerializeObject(SavedData);
                edit.PutString("savedplaces", savejson);
                edit.Apply();
                Toast.MakeText(this, "Place saved successfully", ToastLength.Short).Show();

                database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
                string phone = pref.GetString("phone", "");
                DatabaseReference otheraddress = database.GetReference("users/" + phone + "/savedplaces/others");
                otheraddress.SetValue(savejson);
                addplace_dialogue.Dismiss();
            }
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}