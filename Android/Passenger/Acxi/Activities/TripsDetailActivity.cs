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
using Newtonsoft.Json;
using Android.Graphics;
using System.Globalization;
using Calligraphy;
using Acxi.Helpers;
using System.Threading.Tasks;
using Plugin.Connectivity;
using Refractored.Controls;
using System.Threading;
using Acxi.DialogueFragment;
using Firebase.Database;
using Java.Util;
using Firebase;

namespace Acxi.Activities
{
    [Activity(Label = "TripsDetailActivity", Theme = "@style/AcxiTheme1", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class TripsDetailActivity : AppCompatActivity
    {
        SupportWidget.Toolbar mToolbar;
        
        TextView txtlocation;
         TextView txtdestination;
         TextView txtamount;
         TextView txttimestamp;
         TextView txtstatus;
         TextView txtpayment;

         TextView txtdrivername;
         TextView txtcarmodel;
         TextView txtplate;
        TextView txtfeedback;

         ImageView star1;
         ImageView star2;
         ImageView star3;
         ImageView star4;
         ImageView star5;

        CircleImageView driverImage;

         Button btnreport;
         Button btnresend;

        LinearLayout layout;
        HelperFunctions helper = new Helpers.HelperFunctions();
        WebRequestHelpers webhelper = new WebRequestHelpers();

        string ride_id = "";
        FirebaseDatabase database;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
        .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
     .SetFontAttrId(Resource.Attribute.fontPath)
     .Build());

            SetContentView(Resource.Layout.historydetails);

            mToolbar = (SupportWidget.Toolbar)FindViewById(Resource.Id.tripsdetailToolbar);

            layout = (LinearLayout)FindViewById(Resource.Id.layoutcover_history_detail);
            txtlocation = (TextView)FindViewById(Resource.Id.txtlocation_tripdetails);
            txtdestination = (TextView)FindViewById(Resource.Id.txtdestination_tripdetails);
            txtamount = (TextView)FindViewById(Resource.Id.txtfare_tripdetails);
            txtstatus = (TextView)FindViewById(Resource.Id.txttripstatus_tripdetails);
            txttimestamp = (TextView)FindViewById(Resource.Id.txttimestamp_tripdetails);
            txtpayment = (TextView)FindViewById(Resource.Id.txtpayment_tripdetails);

            txtdrivername = (TextView)FindViewById(Resource.Id.txtdrivername_tripdetails);
            txtcarmodel = (TextView)FindViewById(Resource.Id.txtcarmodel_tripdetails);
            txtplate = (TextView)FindViewById(Resource.Id.txtplatenumber_tripdetails);

            txtfeedback = (TextView)FindViewById(Resource.Id.txtfeedback_tripdetails);
            

            btnreport = (Button)FindViewById(Resource.Id.btnreport_tripdetails);
            btnresend = (Button)FindViewById(Resource.Id.btnresend_tripdetails);
            btnreport.Click += Btnreport_Click;
            btnresend.Click += Btnresend_Click;

            star1 = (ImageView)FindViewById(Resource.Id.star1_tripdetails);
            star2 = (ImageView)FindViewById(Resource.Id.star2_tripdetails);
            star3 = (ImageView)FindViewById(Resource.Id.star3_tripdetails);
            star4 = (ImageView)FindViewById(Resource.Id.star4_tripdetails);
            star5 = (ImageView)FindViewById(Resource.Id.star5_tripdetails);

            driverImage = (CircleImageView)FindViewById(Resource.Id.imgdriver_historydetail);

            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Trip details";

            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);

            layout.Visibility = ViewStates.Visible;
            if (CrossConnectivity.Current.IsConnected)
            {
                string jsondata = Intent.GetStringExtra("data");
                QueryHistory(jsondata);
            }
            else
            {
                layout.Visibility = ViewStates.Visible;
            }
           
          //  SetupView(jsondata);
            
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

        private async void Btnresend_Click(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                AlertInit("Internet connectivity is not available");
                return;
            }

            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();
            string response = "";
            string email = pref.GetString("email", "");

            if (string.IsNullOrEmpty(email))
            {
                progress.Dismiss();
                Toast.MakeText(this, "Please add your email address to your profile", ToastLength.Short).Show();
                return;
            }

            await Task.Run(() =>
            {
                response = webhelper.ResendReceipt(email, ride_id);
               
            });

            if (string.IsNullOrEmpty(response))
            {
                Toast.MakeText(this, "Opps! something went wrong please try again", ToastLength.Short).Show();
                return;
            }

            if (response == "success")
            {
                Toast.MakeText(this, "Receipt was sent successfully", ToastLength.Short).Show();
            }

            progress.Dismiss();
        }

        private void Btnreport_Click(object sender, EventArgs e)
        {
            ReportAProblem_Dialogue report_farg = new ReportAProblem_Dialogue();
            report_farg.Cancelable = false;
            report_farg.OnReported += Report_farg_OnReported;
            var trans = SupportFragmentManager.BeginTransaction();
            report_farg.Show(trans, "report");
        }

        private void Report_farg_OnReported(object sender, ReportProblemEventArgs e)
        {
            if(e.type == "others")
            {
                Intent intent = new Intent(this, typeof(AppSupportActivity));
                intent.PutExtra("ride_id", ride_id);
                StartActivity(intent);
            }
            else
            {
                SendReport(e.type);
            }
        }

        public async void SendReport(string type)
        {
            
            double timestamp = helper.GetTimeStampNow();
            string status = "active";
            string phone = pref.GetString("phone", "");
            string ticket_id = helper.GenerateRandomString(10);
            HashMap map = new HashMap();
            string message = "Ride Id : " + ride_id;
            map.Put("message",  message);
            map.Put("created_at", timestamp.ToString());
            map.Put("status", status);
            map.Put("user_id", phone);
            map.Put("category", "Ride problem report");

            if(type== "lostitem")
            {
                map.Put("title", "I lost my item");
            }
            else if(type == "overcharge")
            {
                map.Put("title", "I was overcharged");
            }
            else if(type == "promo")
            {
                map.Put("title", "I had issues with promo");
            }
            else if(type == "longroute")
            {
                map.Put("title", "Driver took a long route");
            }

            database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
            DatabaseReference supportref = database.GetReference("userSupport/" + ticket_id);
            supportref.SetValue(map);
            DatabaseReference userspportref = database.GetReference("users/" + phone + "/tickets/" + ticket_id);

            userspportref.SetValue(true);
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();
            await Task.Run(() =>
            {
                Thread.Sleep(2000);
            });
            progress.Dismiss();
            Toast.MakeText(this, "Your message was successfully sent", ToastLength.Short).Show();
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

        public async void QueryHistory(string jsondata)
        {
            var deser = JsonConvert.DeserializeObject<rideHistory>(jsondata);
            ride_id = deser.ride_id;
            string jstr = "";
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetMessage("Fetching history...");
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetCancelable(false);
            progress.Show();
           await Task.Run(() =>
           {
                jstr = webhelper.GetRideInfo(deser.ride_id);
           });
           
            Console.WriteLine(jstr);

            progress.Dismiss();
            if (!string.IsNullOrEmpty(jstr))
            {
                layout.Visibility = ViewStates.Gone;
                SetupView(jstr);
            }

        }

        private void SetupView(string jsondata)
        {
          
            var deser = JsonConvert.DeserializeObject<RideFullDetails>(jsondata);

            Thread t2 = new Thread(delegate ()
            {
                downloadImageImage(deser.driver_pic);

            });
            t2.Start();


            txtlocation.Text = (!string.IsNullOrEmpty(deser.pickup_address)) ? deser.pickup_address : "Not available";
            txtdestination.Text = (!string.IsNullOrEmpty(deser.destination_address)) ? deser.destination_address : "Not available";
            txtamount.Text = (!string.IsNullOrEmpty(deser.total_fares)) ? helper.CurrencyConvert(double.Parse(deser.total_fares)) : "Not available";
            if (!string.IsNullOrEmpty(deser.status))
            {
                if(deser.status == "ended")
                {
                    txtstatus.Text = "Completed";
                    txtstatus.SetTextColor(Color.Rgb(70, 188, 82));
                }
                else if (deser.status.Contains("cancel"))
                {
                    txtstatus.Text = "Cancelled";
                    txtstatus.SetTextColor(Color.Rgb(251, 24, 24));
                }
                else
                {
                    txtstatus.Text = "Undefined";
                    txtstatus.SetTextColor(Color.Rgb(238, 134, 31));
                }
            }
            else
            {
                txtstatus.Text = "Not availabe";
                txtstatus.SetTextColor(Color.Rgb(128, 128, 128));
            }

            txttimestamp.Text = (double.Parse( deser.created_at) > 1) ? helper.ConvertToDate( double.Parse(deser.created_at)) : "Not Available";
            txtpayment.Text = (!string.IsNullOrEmpty(deser.payment_method)) ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(deser.payment_method) : "Not available";
            txtdrivername.Text = (!string.IsNullOrEmpty(deser.driver_firstname)) ? deser.driver_firstname : "Not available";
            string carmodel = !string.IsNullOrEmpty(deser.driver_carmodel) ? deser.driver_carmodel : "";
            string caryear = !string.IsNullOrEmpty(deser.driver_caryear) ? deser.driver_caryear : "";
            string carmake = !string.IsNullOrEmpty(deser.driver_carmake) ? deser.driver_carmake : "";
            string carcolor = !string.IsNullOrEmpty(deser.driver_carcolor) ? deser.driver_carcolor : "";
            txtcarmodel.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(carcolor) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(carmake) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(carmodel) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(caryear);
            txtplate.Text = (!string.IsNullOrEmpty(deser.driver_platenumber)) ? deser.driver_platenumber :  "Not available";
            txtfeedback.Text = (string.IsNullOrEmpty(deser.feedback)) ? deser.feedback : "";
            string rating = !string.IsNullOrEmpty(deser.rating) ? deser.rating : "0";
            CalculateStar(double.Parse(rating));
           
        }

        public void downloadImageImage(string imageurl)
        {
            if (!string.IsNullOrEmpty(imageurl))
            {

                string imagestring = webhelper.downloadImage1(imageurl);
                if (!string.IsNullOrEmpty(imagestring))
                {
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            helper.SetProfileImage(driverImage, imagestring);
                        }
                        catch
                        {

                        }

                    });
                }
            }

        }

        private void CalculateStar(double ride_rating)
        {
            if(ride_rating == 1)
            {
                //REPLACE BORDERED IMAGE WITH FILLED IMAGE RESOURCE
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                //TINT THE IMAGE WITH ORANGE COLOR
                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(149, 152, 154));
                star3.SetColorFilter(Color.Rgb(149, 152, 154));
                star4.SetColorFilter(Color.Rgb(149, 152, 154));
                star5.SetColorFilter(Color.Rgb(149, 152, 154));
            }
            else if(ride_rating == 2)
            {
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(238, 134, 31));
                star3.SetColorFilter(Color.Rgb(149, 152, 154));
                star4.SetColorFilter(Color.Rgb(149, 152, 154));
                star5.SetColorFilter(Color.Rgb(149, 152, 154));
            }
            else if (ride_rating == 3)
            {
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(238, 134, 31));
                star3.SetColorFilter(Color.Rgb(238, 134, 31));
                star4.SetColorFilter(Color.Rgb(149, 152, 154));
                star5.SetColorFilter(Color.Rgb(149, 152, 154));
            }
            else if (ride_rating == 4)
            {
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star4.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(238, 134, 31));
                star3.SetColorFilter(Color.Rgb(238, 134, 31));
                star4.SetColorFilter(Color.Rgb(238, 134, 31));
                star5.SetColorFilter(Color.Rgb(149, 152, 154));
            }
            else if (ride_rating == 5)
            {
                star1.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star2.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star3.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star4.SetImageResource(Resource.Mipmap.ic_star_black_48dp);
                star5.SetImageResource(Resource.Mipmap.ic_star_black_48dp);

                star1.SetColorFilter(Color.Rgb(238, 134, 31));
                star2.SetColorFilter(Color.Rgb(238, 134, 31));
                star3.SetColorFilter(Color.Rgb(238, 134, 31));
                star4.SetColorFilter(Color.Rgb(238, 134, 31));
                star4.SetColorFilter(Color.Rgb(238, 134, 31));

            }
        }

        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }
    }
}