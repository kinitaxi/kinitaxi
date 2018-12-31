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
using Acxi.DataModels;
using Refractored.Controls;
using Acxi.Helpers;

namespace Acxi.DialogueFragment
{
    public class DriverDetails_Frag : Android.Support.V4.App.DialogFragment
    {
        string img_string;
        DriverDetails driver_details;

        CircleImageView imgdriver;
        TextView txtname;
        TextView txtplatenumber;
        TextView txtcarmodel;
        TextView txtagenumber;
        TextView txtagetype;
        TextView txtlanguage;
        TextView txtlocation;
        TextView txtrides;
        TextView txtrating;
        HelperFunctions helpers = new HelperFunctions();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.driverprofile, container, false);
            imgdriver = (CircleImageView)view.FindViewById(Resource.Id.img_driverprofile);
            txtname = (TextView)view.FindViewById(Resource.Id.txtname_driverprofile);
            txtplatenumber = (TextView)view.FindViewById(Resource.Id.txtplatenumber_driverprofile);
            txtcarmodel = (TextView)view.FindViewById(Resource.Id.txtcarmodel_driverprofile);
            txtagenumber = (TextView)view.FindViewById(Resource.Id.txtagenumber_driverprofile);
            txtagetype = (TextView)view.FindViewById(Resource.Id.txtagetype_driverprofile);
            txtlanguage = (TextView)view.FindViewById(Resource.Id.txtlanguage_driverprofile);
            txtlocation = (TextView)view.FindViewById(Resource.Id.txtlocation_driverprofile);
            txtrides = (TextView)view.FindViewById(Resource.Id.txtrides_driverprofile);
            txtrating = (TextView)view.FindViewById(Resource.Id.txtrating_driverprofile);

            txtlocation.Text =  "Location - " + driver_details.city + ", Nigeria";
            txtname.Text = driver_details.firstname + " " + driver_details.lastname;
            txtplatenumber.Text = driver_details.plate_number;
            txtlanguage.Text = "Language - " + "English";
            txtcarmodel.Text = driver_details.car_color + " " + driver_details.car_make + " " + driver_details.car_model + " " + driver_details.car_year;
            char[] MyChar = { ' ' };
            string[] arr = driver_details.acxi_age.Split(MyChar);
            try
            {
                txtagenumber.Text = arr[0];
                txtagetype.Text = arr[1];
            }
            catch
            {
                txtagenumber.Text = "0";
                txtagetype.Text = "days";
            }

            //TODO CHECKED NULL
            //Done
            txtrides.Text = (driver_details.completed_rides.ToString() != null) ? driver_details.completed_rides.ToString() : "0";
            txtrating.Text = (driver_details.rating.ToString() != null) ? driver_details.rating.ToString() : "0";
           

            if (!string.IsNullOrEmpty(img_string))
            {
                try
                {
                    helpers.SetProfileImage(imgdriver, img_string);
                }
                catch
                {

                }
            }

            return view;
        }

        public DriverDetails_Frag( string imagebase, DriverDetails Driver)
        {
            img_string = imagebase;
            driver_details = Driver;
        }
    }
}