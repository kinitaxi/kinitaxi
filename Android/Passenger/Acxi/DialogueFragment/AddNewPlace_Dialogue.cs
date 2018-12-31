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
using Android.Support.Design.Widget;
using Android.Gms.Location.Places.UI;
using Android.Gms.Common.Apis;
using Android.Gms.Location.Places;
using Acxi.Helpers;
using Android.Gms.Common;

namespace Acxi.DialogueFragment
{
    public class OnSavePlaceEventArgs : EventArgs
    {
        public string address { get; set; }
        public string title { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
    public class AddNewPlace_Dialogue : Android.Support.V4.App.DialogFragment, IPlaceSelectionListener
    {
        public event EventHandler<OnSavePlaceEventArgs> SavePlace;
        TextInputLayout txttitle;
        TextInputLayout txtaddress;
        Button btnsave;

        string title ="";
        string address = "";
        double lat = 0;
        double lng = 0;
        int PLACE_AUTOCOMPLETE_REQUEST_CODE = 1;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.addplace_dialogue, container, false);
            txttitle = (TextInputLayout)view.FindViewById(Resource.Id.txtlaytitle_addplace);
            txtaddress = (TextInputLayout)view.FindViewById(Resource.Id.txtlayaddress_addplace);
            btnsave = (Button)view.FindViewById(Resource.Id.btnsaveplace);
            txtaddress.EditText.Focusable = false;
            txtaddress.EditText.Clickable = true;

            txtaddress.EditText.Click += EditText_Click;
            btnsave.Click += Btnsave_Click;
            return view;
        }

        private void EditText_Click(object sender, EventArgs e)
        {
            try
            {
                AutocompleteFilter typeFilter = new AutocompleteFilter.Builder()
                    .SetCountry("NG")
                    .Build();
                Intent intent = new PlaceAutocomplete.IntentBuilder(PlaceAutocomplete.ModeOverlay)
                    .SetFilter(typeFilter)
                    .Build(Activity);
                StartActivityForResult(intent, PLACE_AUTOCOMPLETE_REQUEST_CODE);
            }
            catch (GooglePlayServicesRepairableException ex)
            {

            }
            catch (GooglePlayServicesNotAvailableException ex)
            {
                Toast.MakeText(Activity, "Google play service is not available", ToastLength.Short).Show();
            }
        }

        private void Btnsave_Click(object sender, EventArgs e)
        {
            title = txttitle.EditText.Text.Trim();

            if (string.IsNullOrEmpty(title))
            {
                Toast.MakeText(Activity, "Please provide the TITLE of the place to be saved", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(address))
            {
                Toast.MakeText(Activity, "Please select the ADDRESS of the place to be saved", ToastLength.Short).Show();
                return;
            }

            SavePlace.Invoke(this, new OnSavePlaceEventArgs { title = title, address = address, latitude = lat, longitude = lng });   
        }

        public void OnError(Statuses status)
        {
            //
        }

        public void OnPlaceSelected(IPlace place)
        {
           //
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == PLACE_AUTOCOMPLETE_REQUEST_CODE)
            {
                if (resultCode == (int)Result.Ok)
                {
                    var place = PlaceAutocomplete.GetPlace(Activity, data);
                    address = place.NameFormatted.ToString();
                    lat = place.LatLng.Latitude;
                    lng = place.LatLng.Longitude;
                    txtaddress.EditText.Text = address;
                }
                else if ((int)(resultCode) == (int)PlaceAutocomplete.ResultError)
                {
                    Console.WriteLine("error");

                }
                else if (resultCode ==  (int) Android.App.Result.Canceled)
                {
                    // The user canceled the operation.
                }
            }
        }
    }
}