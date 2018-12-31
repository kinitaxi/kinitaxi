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
using System.Threading.Tasks;

namespace AcxiDriver.Activities
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { DecideStartUp(); });
            startupWork.Start();
        }

        public async void DecideStartUp()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
            ISharedPreferencesEditor edit = pref.Edit();

            string license_status = pref.GetString("license_status", "");
            string worthiness_status = pref.GetString("worthiness_status", "");
            string profilepix_status = pref.GetString("profilepix_status", "");
            string account_status = pref.GetString("account_status", "");
            string login = pref.GetString("login", "");

            if (string.IsNullOrEmpty(account_status))
            {
                StartActivity(new Intent(Application.Context, typeof(GetStartedActivity)));

            }
            else if(account_status == "pending")
            {
                 if (license_status == "rejected" || worthiness_status == "rejected" || profilepix_status == "rejected")
                {
                    StartActivity(new Intent(Application.Context, typeof(RegistrationActivity)));
                }
                 else if( string.IsNullOrEmpty(license_status) || string.IsNullOrEmpty(worthiness_status) || string.IsNullOrEmpty(profilepix_status))
                {
                    StartActivity(new Intent(Application.Context, typeof(RegistrationActivity)));
                }
                else
                {
                    StartActivity(new Intent(Application.Context, typeof(RegCompletedActivity)));
                   
                }
            }
            else if(account_status == "approved")
            {
                StartActivity(new Intent(Application.Context, typeof(AppMainActivity)));
               

            }
            else if(account_status == "suspended")
            {
                // SUSPENDED
                Intent intent = new Intent(this, typeof(RegCompletedActivity));
                intent.PutExtra("status", account_status);
                StartActivity(intent);
               
            }

            //if (string.IsNullOrEmpty(login))
            //{
            //    StartActivity(new Intent(Application.Context, typeof(GetStartedActivity)));
            //}
            //else
            //{
            //    if(login == "true")
            //    //{
            //    //    if (license_status == "approved" && worthiness_status == "approved" && profilepix_status == "approved")
            //    //    {
            //    //        StartActivity(new Intent(Application.Context, typeof(AppMainActivity)));
            //    //    }
            //    //    else if (license_status == "pending" && worthiness_status == "pending" && profilepix_status == "pending")
            //    //    {
            //    //        StartActivity(new Intent(Application.Context, typeof(RegCompletedActivity)));
            //    //    }
            //    //    else if (license_status == "rejected" || worthiness_status == "rejected" || profilepix_status == "rejected")
            //    //    {
            //    //        StartActivity(new Intent(Application.Context, typeof(RegistrationActivity)));
            //    //    }
            //    //    else if(!string.IsNullOrEmpty(license_status) && !string.IsNullOrEmpty(worthiness_status) && !string.IsNullOrEmpty(profilepix_status))
            //    //    {
            //    //        StartActivity(new Intent(Application.Context, typeof(RegCompletedActivity)));
            //    //    }
            //    //}
            //    else
            //    {
            //        StartActivity(new Intent(Application.Context, typeof(GetStartedActivity)));
            //    }
            //   }




        }
    }
}