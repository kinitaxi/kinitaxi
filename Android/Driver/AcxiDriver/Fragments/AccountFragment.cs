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
using AcxiDriver.Dialogue;
using AcxiDriver.Activities;
using Acxi.Helpers;
using Refractored.Controls;
using Firebase.Database;
using Firebase;
using Java.Util;
using System.Threading;
using AcxiDriver.EventListeners;
using System.Threading.Tasks;

namespace AcxiDriver.Fragments
{
    public class AccountFragment : Android.Support.V4.App.Fragment
    {
        TextView txtaccountHeader;
        CircleImageView imgaccountHeader;

        LinearLayout btnabout;
        LinearLayout btnsupport;
        LinearLayout btnbank;
        LinearLayout btnpersonal;
        LinearLayout btnvehicle;
        LinearLayout btndocs;
        LinearLayout btnlogout;

        SupportDialogue supportdialogue;
        BankDetailsDailogue bankdialogue;
        PersonalDetailsDialogue personaldialogue;
        VehicleDetailsDialogue vehicledialogue;
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        string driver_firstname;
        string driver_lastname;

        string phone_s;

        FirebaseDatabase database;
        string accountname_s;
        string accountnumber_s;
        string bank_s;

        WebRequestHelpers webHelpers = new WebRequestHelpers();
        HelperFunctions helper = new HelperFunctions();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            edit = pref.Edit();
            driver_firstname = pref.GetString("firstname", "");
            driver_lastname = pref.GetString("lastname", "");
            phone_s = pref.GetString("phone", "");
            database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
            AccountStatus();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.account, container, false);
            btnsupport = (LinearLayout)view.FindViewById(Resource.Id.laysupport_account);
            btnbank = (LinearLayout)view.FindViewById(Resource.Id.laybankdetails_account);
            btnpersonal = (LinearLayout)view.FindViewById(Resource.Id.laypersonal_account);
            btnvehicle = (LinearLayout)view.FindViewById(Resource.Id.layvehicle_account);
            txtaccountHeader = (TextView)view.FindViewById(Resource.Id.txtname_accountheader);
            imgaccountHeader = (CircleImageView)view.FindViewById(Resource.Id.img_accountheader);
            btnabout = (LinearLayout)view.FindViewById(Resource.Id.layabout_account);
            btnabout.Click += Btnabout_Click;
            btndocs = (LinearLayout)view.FindViewById(Resource.Id.laydocument_document);
            btnlogout = (LinearLayout)view.FindViewById(Resource.Id.laylogout);
            btnlogout.Click += Btnlogout_Click;

            btnsupport.Click += Btnsupport_Click;
            btnbank.Click += Btnbank_Click;
            btnpersonal.Click += Btnpersonal_Click;
            btnvehicle.Click += Btnvehicle_Click;
            btndocs.Click += Btndocs_Click;
            txtaccountHeader.Text = (string.IsNullOrEmpty(driver_firstname)) ? "Not available" : driver_firstname;
            string imagestring = pref.GetString("profilepix", "");
            if (!string.IsNullOrEmpty(imagestring))
            {
                HelperFunctions helpers = new HelperFunctions();
              helpers.SetProfileImage1(imgaccountHeader, imagestring);
            }
            else
            {
                string profilepix_url = pref.GetString("profilepix_url", "");
                if (!string.IsNullOrEmpty(profilepix_url))
                {
                    Thread t2 = new Thread(delegate ()
                    {
                        downloadImage(profilepix_url);
                    });
                    t2.Start();
                }
            }


            return view;
        }

        private void Btnabout_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse("https://kinitaxi.com");
            var intent = new Intent(Intent.ActionView, uri);
            StartActivity(intent);
        }

        private void Btnlogout_Click(object sender, EventArgs e)
        {
            AppAlertDialogue appalert = new AppAlertDialogue("Are you sure");
            appalert.Cancelable = false;
            var trans = FragmentManager.BeginTransaction();
            appalert.Show(trans, "alert");
            appalert.AlertCancel += (o, i) =>
            {
                appalert.Dismiss();
            };
            appalert.AlertOk += (w, q) =>
            {
                edit.Clear();
                edit.Apply();
                StartActivity(new Intent(Application.Context, typeof(GetStartedActivity)));
                this.Activity.FinishAffinity();
            };
        }

        public void downloadImage(string url)
        {

            string imagestring = webHelpers.downloadImage(url);
            if (!string.IsNullOrEmpty(imagestring))
            {
                Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        helper.SetProfileImage1(imgaccountHeader, imagestring);
                    }
                    catch
                    {

                    }
                });
                edit.PutString("profilepix", imagestring);
                edit.Apply();
            }
           
        }
        private void Btndocs_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Application.Context, typeof(DocumentActivity));
            StartActivity(intent);
        }

        private void Btnvehicle_Click(object sender, EventArgs e)
        {
            vehicledialogue = new VehicleDetailsDialogue();
            var trans = FragmentManager.BeginTransaction();
            vehicledialogue.Show(trans, "alert");
            vehicledialogue.OnSaveVehicleDetails += Vehicledialogue_OnSaveVehicleDetails;
        }

        private void Vehicledialogue_OnSaveVehicleDetails(object sender, OnSaveVehicleDetailsEventArgs e)
        {
            string make, model, year, color, plate_number = "";
            make = pref.GetString("make", "");
            model = pref.GetString("model", "");
            year = pref.GetString("year", "");
            color = pref.GetString("color", "");
            plate_number = pref.GetString("plate_number", "");

            if(make != e.make || model != e.model || plate_number != e.platenumber || year != e.year)
            {
                DatabaseReference vehicleRef = database.GetReference("drivers/" + phone_s + "/vehicle_details");
                HashMap map = new HashMap();
                map.Put("make", e.make);
                map.Put("model", e.model);
                map.Put("year", e.year);
                map.Put("color", e.color);
                map.Put("plate_number", e.platenumber);
                vehicleRef.SetValue(map);

                Toast.MakeText(Application.Context, "Vehicle details successfully saved", ToastLength.Short).Show();
                edit.PutString("make", e.make);
                edit.PutString("model", e.model);
                edit.PutString("year", e.year);
                edit.PutString("color", e.color);
                edit.PutString("platenumber", e.platenumber);
                edit.Apply();
               
            }
        }

        private void Btnpersonal_Click(object sender, EventArgs e)
        {

            personaldialogue = new PersonalDetailsDialogue();
            var trans = FragmentManager.BeginTransaction();
            personaldialogue.Show(trans, "alert");
            personaldialogue.OnSaveProfileDetails += Personaldialogue_OnSaveProfileDetails;
        }

        private void Personaldialogue_OnSaveProfileDetails(object sender, OnSaveProfileDetailsEventArgs e)
        {
            string firstname, lastname, email, city = "";
            firstname = pref.GetString("firstname", "");
            lastname = pref.GetString("lastname", "");
            email = pref.GetString("email", "");
            city = pref.GetString("city", "");

            if(firstname != e.firstname)
            {
                DatabaseReference firstnameRef = database.GetReference("drivers/" + phone_s + "/personal_details/first_name");
                firstnameRef.SetValue(e.firstname);
                edit.PutString("firstname", e.firstname);
                edit.Apply();
                txtaccountHeader.Text = e.firstname;
                firstnameRef.Dispose();
            }

            if(lastname != e.lastname)
            {
                DatabaseReference lastnameRef = database.GetReference("drivers/" + phone_s + "/personal_details/last_name");
                lastnameRef.SetValue(e.lastname);
                lastnameRef.Dispose();
                edit.PutString("lastname", e.lastname);
                edit.Apply();
            }

            if(email != e.email)
            {
                DatabaseReference emailRef = database.GetReference("drivers/" + phone_s + "/personal_details/email");
                emailRef.SetValue(e.email);
                emailRef.Dispose();
                edit.PutString("email", e.email);
                edit.Apply();
            }

            if (city != e.city)
            {
                DatabaseReference cityRef = database.GetReference("drivers/" + phone_s + "/personal_details/city");
                cityRef.SetValue(e.city);
                cityRef.Dispose();
                edit.PutString("city", e.city);
                edit.Apply();
            }
           
            Toast.MakeText(Application.Context, "Profile saved", ToastLength.Short).Show();
        }

        //DONE
        private void Btnbank_Click(object sender, EventArgs e)
        {
            accountname_s = pref.GetString("account_name", "");
            accountnumber_s = pref.GetString("account_number", "");
            bank_s = pref.GetString("bank_name", "");

            bankdialogue = new BankDetailsDailogue(accountname_s, accountnumber_s, bank_s);
            var trans = FragmentManager.BeginTransaction();
            bankdialogue.Show(trans, "alert");
            bankdialogue.OnSaveaccountDetails += Bankdialogue_OnSaveaccountDetails;
        }


        public void AlertInit(string body)
        {
            AppAlertDialogue appAlert = new AppAlertDialogue(body);
            appAlert.Cancelable = true;
            var trans1 = FragmentManager.BeginTransaction();
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
        // DONE
        private async void Bankdialogue_OnSaveaccountDetails(object sender, BankDetailsDailogue.OnSaveAccountDetailsEventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(Activity);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();

            if (string.IsNullOrEmpty(accountname_s) || string.IsNullOrEmpty(accountnumber_s)|| string.IsNullOrEmpty(bank_s) || accountname_s != e.accname || accountnumber_s != e.accnumber || bank_s != e.bank)
            {
                string response = "";
                await Task.Run(() =>
                {
                    response = webHelpers.AddAccount(e.accname, e.accnumber, e.code, e.bank, phone_s);
                    Console.WriteLine(response);
                });

                if (string.IsNullOrEmpty(response))
                {
                    Toast.MakeText(Application.Context, "Oops something went wrong please try again", ToastLength.Short).Show();
                    progress.Dismiss();
                    return;
                }

                if(response.Contains("fail"))
                {
                    AlertInit("Account information is incorrect");
                    progress.Dismiss();
                    return;
                }
                else
                {
                    edit.PutString("account_name", e.accname);
                    edit.PutString("account_number", e.accnumber);
                    edit.PutString("bank_name", e.bank);
                    edit.PutString("bank_code", e.code);
                    edit.Apply();

                    AlertInit("Your account information was added successfully");
                }

            }
            else
            {
               
            }

            progress.Dismiss();
           // e.accname;
        }

        private void Btnsupport_Click(object sender, EventArgs e)
        {
            supportdialogue = new SupportDialogue();
            var trans = FragmentManager.BeginTransaction();
            supportdialogue.Show(trans, "alert");
            supportdialogue.CreateTicket += Supportdialogue_CreateTicket;
            supportdialogue.ViewTickets += Supportdialogue_ViewTickets;
        }

        private void Supportdialogue_ViewTickets(object sender, EventArgs e)
        {
            supportdialogue.Dismiss();
            Intent intent = new Intent(Application.Context, typeof(ViewTicketActivity));
            StartActivity(intent);
        }

        private void Supportdialogue_CreateTicket(object sender, EventArgs e)
        {
            supportdialogue.Dismiss();
            Intent intent = new Intent(Application.Context, typeof(NewTicketActivity));
            StartActivity(intent);
        }

        public void AccountStatus()
        {
            DatabaseReference accountRef = database.GetReference("drivers/" + phone_s + "/account_status");
            AccountStatusValueEventListener account_listener = new AccountStatusValueEventListener();
            accountRef.AddValueEventListener(account_listener);
            account_listener.AccountStatus += Account_listener_AccountStatus;
            account_listener.DriverAccountDeleted += Account_listener_DriverAccountDeleted;
        }

        private void Account_listener_DriverAccountDeleted(object sender, EventArgs e)
        {
            Toast.MakeText(Application.Context, "This account no longer exist, Please register with KiniTaxi", ToastLength.Short).Show();
            Activity.FinishAffinity();
            edit.Clear().Apply();
            Intent intent = new Intent(Application.Context, typeof(GetStartedActivity));
            StartActivity(intent);
        }

        private void Account_listener_AccountStatus(object sender, AccountStatusValueEventListener.AccountStatusEventArgs e)
        {
            UpdateToken();
            edit.PutString("account_status", e.account_status);
            edit.Apply();
        }

        public void UpdateToken()
        {
            string token = pref.GetString("apptoken", "");
            if (!string.IsNullOrEmpty(token))
            {
                DatabaseReference tokenRef = database.GetReference("drivers/" + phone_s + "/token");
                tokenRef.SetValue(token);
            }
        }
    }
}