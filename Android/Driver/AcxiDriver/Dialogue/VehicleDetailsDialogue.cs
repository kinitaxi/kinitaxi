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
using System.Globalization;

namespace AcxiDriver.Dialogue
{
    public class OnSaveVehicleDetailsEventArgs : EventArgs
    {
        public string make { get; set; }
        public string model { get; set; }
        public string year { get; set; }
        public string color { get; set; }
        public string platenumber { get; set; }

    }
    public class VehicleDetailsDialogue : Android.Support.V4.App.DialogFragment
    {
        TextInputLayout txtmake;
        TextInputLayout txtmodel;
        TextInputLayout txtyear;
        TextInputLayout txtcolor;
        TextInputLayout txtplatenumber;
        Button btnsave;
        public event EventHandler<OnSaveVehicleDetailsEventArgs> OnSaveVehicleDetails;

        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        string make_a;
        string model_a;
        string year_a;
        string color_a;
        string platenumber_a;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            edit = pref.Edit();
            make_a = pref.GetString("make", "");
            model_a = pref.GetString("model", "");
            year_a = pref.GetString("year", "");
            color_a = pref.GetString("color", "");
            platenumber_a = pref.GetString("platenumber", "");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.vehicledetails_dialogue, container, false);
            txtmake = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutmake_vehicle);
            txtmodel = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutmodel_vehicle);
            txtyear = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutyear_vehicle);
            txtcolor = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutcolor_vehicle);
            txtplatenumber = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutplatenumber_vehicle);
            btnsave = (Button)view.FindViewById(Resource.Id.btnsavedetails_vehicle);

            txtmodel.EditText.Click += EditText_Click1;
            txtmake.EditText.Click += EditText_Click;
            txtmake.EditText.Text = make_a;
            txtmodel.EditText.Text = model_a;
            txtyear.EditText.Text = year_a;
            txtcolor.EditText.Text = color_a;
            txtplatenumber.EditText.Text = platenumber_a;
            btnsave.Click += Btnsave_Click;

            return view;
        }

        private void EditText_Click1(object sender, EventArgs e)
        {
            ListDialogueModel lst = new ListDialogueModel(txtmodel, txtmake.EditText.Text.Trim());
            var trans = FragmentManager.BeginTransaction();
            lst.Show(trans, "lst");
        }

        private void EditText_Click(object sender, EventArgs e)
        {
            ListDialogueMake lst = new ListDialogueMake(txtmake);
            var trans = FragmentManager.BeginTransaction();
            lst.Show(trans, "lst");
        }

        private void Btnsave_Click(object sender, EventArgs e)
        {
            string mMake = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtmake.EditText.Text);
            string mModel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtmodel.EditText.Text);
            string mColor = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtcolor.EditText.Text) ;
            string mYear = txtyear.EditText.Text;
            string mPlateNumber =  txtplatenumber.EditText.Text.ToUpper();

            if(string.IsNullOrEmpty(mMake) || string.IsNullOrWhiteSpace(mMake))
            {
                Toast.MakeText(Application.Context, "Please provide your Vehicle Make", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(mModel) || string.IsNullOrWhiteSpace(mModel))
            {
                Toast.MakeText(Application.Context, "Please provide your Vehicle Model", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(mColor) || string.IsNullOrWhiteSpace(mColor))
            {
                Toast.MakeText(Application.Context, "Please provide your Vehicle Color", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(mYear) || string.IsNullOrWhiteSpace(mYear))
            {
                Toast.MakeText(Application.Context, "Please provide your Vehicle Year", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(mPlateNumber) || string.IsNullOrWhiteSpace(mPlateNumber))
            {
                Toast.MakeText(Application.Context, "Please provide your Vehicle Plate Number", ToastLength.Short).Show();
                return;
            }

           
            OnSaveVehicleDetails.Invoke(this, new OnSaveVehicleDetailsEventArgs { make = mMake, color = mColor, model = mModel, platenumber = mPlateNumber, year = mYear });
            this.Dismiss();
        }

    }
}