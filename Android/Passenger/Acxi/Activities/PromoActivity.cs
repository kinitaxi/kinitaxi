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
using Calligraphy;
using Android.Support.V7.Widget;
using Firebase.Database;
using Android.Support.Design.Widget;
using Acxi.EventListeners;
using Acxi.DialogueFragment;
using Acxi.Helpers;
using Firebase;

namespace Acxi.Activities
{
    [Activity(Label = "PromoActivity", Theme = "@style/AcxiTheme1")]
    public class PromoActivity : AppCompatActivity
    {


        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        PromoValueEventListener promo_listener;
        HelperFunctions helper = new HelperFunctions();
        FirebaseDatabase database;
        DatabaseReference promoref;
        TextInputLayout txtpromocode;
        TextView txtpromobalance;
        Button btnredeem_promo;
        ProgressDialog progress;
        double promobalance;
        Android.Support.V7.Widget.Toolbar mtoolbar;
        string phone;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                    .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                    .SetFontAttrId(Resource.Attribute.fontPath)
                    .Build());

            SetContentView(Resource.Layout.promo);

            promobalance = double.Parse(Intent.GetStringExtra("promo"));
            database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);

            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.promoToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Promo";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);

            txtpromocode = (TextInputLayout)FindViewById(Resource.Id.txtpromocode);
            txtpromobalance = (TextView)FindViewById(Resource.Id.txtpromo_balance);
            btnredeem_promo = (Button)FindViewById(Resource.Id.btnredeem_promo);
            btnredeem_promo.Click += Btnredeem_promo_Click;
            promobalance = double.Parse(Intent.GetStringExtra("promo"));
            txtpromobalance.Text = helper.CurrencyConvert(promobalance);
            progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetCancelable(false);
            progress.SetMessage("Please wait....");
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            phone = pref.GetString("phone", "");
        }

        private void Btnredeem_promo_Click(object sender, EventArgs e)
        {
            promoref = database.GetReference("promoGeneral/" + txtpromocode.EditText.Text);
            promo_listener = new PromoValueEventListener("general", phone);
            promoref.AddValueEventListener(promo_listener);
            promo_listener.PromoInvalid += Promo_listener_PromoInvalid;
            promo_listener.PromoValid += Promo_listener_PromoValid;
            promo_listener.PromoGeneralused += Promo_Listener_PromoGeneralused;
            progress.Show();
        }


        void Promo_Listener_PromoGeneralused(object sender, EventArgs e)
        {
            progress.Dismiss();
            AppAlertDialogue appalert = new AppAlertDialogue("Promo code has already been used ");
            appalert.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            appalert.Show(trans, "alert");
            appalert.AlertOk += (o, p) =>
            {
                appalert.Dismiss();
                txtpromocode.EditText.Text = "";
            };
            appalert.AlertCancel += (o, j) =>
            {
                appalert.Dismiss();
                txtpromocode.EditText.Text = "";
            };
        }


        private void Promo_listener_PromoValid(object sender, PromoValueEventListener.PromoValidEventArgs e)
        {
            DatabaseReference newpromoref = database.GetReference("users/" + phone + "/wallet/promo_wallet");
            DatabaseReference usedPromoRef = database.GetReference("promoGeneral/" + txtpromocode.EditText.Text + "/users/" + phone);
            usedPromoRef.SetValue(true);

            double newpromobalance = promobalance + double.Parse(e.amount);
            newpromoref.SetValue(newpromobalance);
            promobalance = newpromobalance;

            txtpromobalance.Text = helper.CurrencyConvert(newpromobalance);

            if (promoref != null)
            {
                promoref.RemoveEventListener(promo_listener);
                // promoref.RemoveValue();
            }

            progress.Dismiss();
            AppAlertDialogue appalert = new AppAlertDialogue("Your promo wallet has been credited with " + helper.CurrencyConvert(double.Parse(e.amount)));
            appalert.Cancelable = false;
            var trans = SupportFragmentManager.BeginTransaction();
            appalert.Show(trans, "alert");
            appalert.AlertOk += (o, p) =>
            {
                appalert.Dismiss();
            };
            appalert.AlertCancel += (o, j) =>
            {
                appalert.Dismiss();
            };
            txtpromocode.EditText.Text = "";
        }

        private void Promo_listener_PromoInvalid(object sender, EventArgs e)
        {
            promoref.RemoveEventListener(promo_listener);
            promo_listener = null;

            DatabaseReference singlepromoref = database.GetReference("promo/" + txtpromocode.EditText.Text);
            PromoValueEventListener mypromo_listener = new PromoValueEventListener("single", phone);
            singlepromoref.AddValueEventListener(mypromo_listener);
            mypromo_listener.PromoInvalid += (o, t) => {

                progress.Dismiss();
                AppAlertDialogue appalert = new AppAlertDialogue("This PROMO CODE is invalid");
                appalert.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction();
                appalert.Show(trans, "alert");

                appalert.AlertOk += (q, w) =>
                {
                    appalert.Dismiss();
                };

                appalert.AlertOk += (y, u) =>
                {
                    appalert.Dismiss();
                };
                txtpromocode.EditText.Text = "";
            };

            mypromo_listener.PromoValid += (z, x) => {


                string phone = pref.GetString("phone", "");
                DatabaseReference newpromoref = database.GetReference("users/" + phone + "/wallet/promo_wallet");

                double newpromobalance = promobalance + double.Parse(x.amount);
                newpromoref.SetValue(newpromobalance);
                txtpromobalance.Text = helper.CurrencyConvert(newpromobalance);

                if (singlepromoref != null)
                {
                    singlepromoref.RemoveEventListener(mypromo_listener);
                    singlepromoref.RemoveValue();
                }

                progress.Dismiss();
                AppAlertDialogue appalert = new AppAlertDialogue("Your promo wallet has been credited with " + helper.CurrencyConvert(double.Parse(x.amount)));
                appalert.Cancelable = false;
                var trans = SupportFragmentManager.BeginTransaction();
                appalert.Show(trans, "alert");
                appalert.AlertOk += (o, p) =>
                {
                    appalert.Dismiss();
                };
                appalert.AlertCancel += (o, j) =>
                {
                    appalert.Dismiss();
                };
                txtpromocode.EditText.Text = "";
            };


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
        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }
    }
}