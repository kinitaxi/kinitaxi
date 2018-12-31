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
using SupportActionBar = Android.Support.V7.App.ActionBar;
using Calligraphy;

namespace Acxi.Activities
{
    [Activity(Label = "AddCardActivity", Theme ="@style/AcxiTheme1", MainLauncher = false)]
    public class AddCardActivity : AppCompatActivity
    {
        SupportWidget.Toolbar mToolbar;
        EditText cardnumber;
        EditText cardexpiry;
        EditText cardcvv;
        string lasttext = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
      .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
      .SetFontAttrId(Resource.Attribute.fontPath)
      .Build());

            SetContentView(Resource.Layout.addcard_payment);
   
            mToolbar = (SupportWidget.Toolbar)FindViewById(Resource.Id.addcard_paymentToolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Add card";
            SupportActionBar ab = SupportActionBar;
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);
            ab.SetDisplayHomeAsUpEnabled(true);

            cardnumber = (EditText)FindViewById(Resource.Id.txt_cardnum_addcard);
            cardexpiry = (EditText)FindViewById(Resource.Id.txt_cardexp_addcard);
            cardcvv = (EditText)FindViewById(Resource.Id.txt_cardCVV_addcard);
           
            cardexpiry.TextChanged += Cardexpiry_TextChanged;
            cardexpiry.BeforeTextChanged += Cardexpiry_BeforeTextChanged;
        }

        private void Cardexpiry_BeforeTextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            if(cardexpiry.Text.Length != 0)
            {
                lasttext = cardexpiry.Text.Substring(cardexpiry.Text.Length - 1, 1);
            }
        }

        private void Cardexpiry_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            
            if(cardexpiry.Text.Length == 2 && lasttext != "/")
            {
                cardexpiry.Text = cardexpiry.Text + "/";
                cardexpiry.SetSelection(3);
            }
            else if (cardexpiry.Text.Length == 2 && lasttext == "/")
            {
                cardexpiry.Text = cardexpiry.Text.Substring(0, 1);
                cardexpiry.SetSelection(1);
            }
            Console.WriteLine("TextChanged = " + cardexpiry.Text.Length.ToString());
        }

        protected override void AttachBaseContext(Context context)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(context));
        }
    }
}