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
using Android.Graphics;
using Refractored.Controls;
using Acxi.Helpers;
using Acxi.DataModels;

namespace Acxi.DialogueFragment
{
    public class RatingEventArgs : EventArgs
    {
        public string feedback { get; set; }
        public string rating { get; set; }
    }
    public class RateTrip_Frag : Android.Support.V4.App.DialogFragment
    {
        ImageView ImgStar1;
        ImageView ImgStar2;
        ImageView ImgStar3;
        ImageView ImgStar4;
        ImageView ImgStar5;
        CircleImageView imgdriverratetrip;

        TextView txtname;
        EditText txtfeedback;
        Button btndone;

        double rating;
        string feedback;
        string imgstring;
        string mfirstname, mlastname;

        public event EventHandler<RatingEventArgs> OnRateDone;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.ratetrip_dialogue, container, false);
            ImgStar1 = (ImageView)view.FindViewById(Resource.Id.star1_ratetrip);
            ImgStar2 = (ImageView)view.FindViewById(Resource.Id.star2_ratetrip);
            ImgStar3 = (ImageView)view.FindViewById(Resource.Id.star3_ratetrip);
            ImgStar4 = (ImageView)view.FindViewById(Resource.Id.star4_ratetrip);
            ImgStar5 = (ImageView)view.FindViewById(Resource.Id.star5_ratetrip);
            imgdriverratetrip = (CircleImageView)view.FindViewById(Resource.Id.img_driverratetrip);

            txtname = (TextView)view.FindViewById(Resource.Id.txtname_ratetrip);
            txtfeedback = (EditText)view.FindViewById(Resource.Id.txtfeedback_ratetrip);
            btndone = (Button)view.FindViewById(Resource.Id.btnfeedback_ratetrip);
            btndone.Click += Btndone_Click;

            txtname.Text = mfirstname + " " + mlastname;
            ImgStar1.Click += ImgStar1_Click;
            ImgStar2.Click += ImgStar2_Click;
            ImgStar3.Click += ImgStar3_Click;
            ImgStar4.Click += ImgStar4_Click;
            ImgStar5.Click += ImgStar5_Click;

            if(!string.IsNullOrEmpty(imgstring))
            {
                HelperFunctions helpers = new HelperFunctions();
                helpers.SetProfileImage(imgdriverratetrip, imgstring);
                imgstring = null;
            }
            return view;
        }

        public RateTrip_Frag(string imagestring, string firstname, string lastname)
        {
            
            imgstring = imagestring;
            mfirstname = firstname;
            mlastname = lastname;
        }

        private void Btndone_Click(object sender, EventArgs e)
        {
           if(rating != null)
            {
                OnRateDone.Invoke(this, new RatingEventArgs { feedback = txtfeedback.Text, rating = rating.ToString() });
                this.Dismiss();
            }
        }

        private void ImgStar5_Click(object sender, EventArgs e)
        {
            rating = 5;

            ImgStar1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar1.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar2.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar3.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar4.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar4.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar5.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar5.SetColorFilter(Color.Rgb(238, 134, 31));
        }

        private void ImgStar4_Click(object sender, EventArgs e)
        {
            rating = 4;

            ImgStar1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar1.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar2.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar3.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar4.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar4.SetColorFilter(Color.Rgb(238, 134, 31));



            ImgStar5.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar5.SetColorFilter(Color.Rgb(128, 128, 128));

        }

        private void ImgStar3_Click(object sender, EventArgs e)
        {
            rating = 3;

            ImgStar1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar1.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar2.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar3.SetColorFilter(Color.Rgb(238, 134, 31));



            ImgStar4.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar4.SetColorFilter(Color.Rgb(128, 128, 128));

            ImgStar5.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar5.SetColorFilter(Color.Rgb(128, 128, 128));

        }

        private void ImgStar2_Click(object sender, EventArgs e)
        {
            rating = 2;

            ImgStar1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar1.SetColorFilter(Color.Rgb(238, 134, 31));

            ImgStar2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar2.SetColorFilter(Color.Rgb(238, 134, 31));



            ImgStar3.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar3.SetColorFilter(Color.Rgb(128, 128, 128));

            ImgStar4.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar4.SetColorFilter(Color.Rgb(128, 128, 128));

            ImgStar5.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar5.SetColorFilter(Color.Rgb(128, 128, 128));

        }

        private void ImgStar1_Click(object sender, EventArgs e)
        {
            rating = 1;

            ImgStar1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
            ImgStar1.SetColorFilter(Color.Rgb(238, 134, 31));



            ImgStar2.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar2.SetColorFilter(Color.Rgb(128, 128, 128));

            ImgStar3.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar3.SetColorFilter(Color.Rgb(128, 128, 128));

            ImgStar4.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar4.SetColorFilter(Color.Rgb(128, 128, 128));

            ImgStar5.SetImageResource(Resource.Mipmap.ic_star_border_black_48dp);
            ImgStar5.SetColorFilter(Color.Rgb(128, 128, 128));


        }
    }
}