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
using FR.Ganfra.Materialspinner;
using Acxi.Helpers;
using Firebase.Database;

namespace AcxiDriver.Dialogue
{
    public class BankDetailsDailogue : Android.Support.V4.App.DialogFragment
    {
        public class OnSaveAccountDetailsEventArgs : EventArgs
        {
            public string accname { get; set; }
            public string accnumber { get; set; }
            public string bank { get; set; }
            public string code { get; set; }
        }

        public event EventHandler<OnSaveAccountDetailsEventArgs> OnSaveaccountDetails;
        TextInputLayout txtname;
        TextInputLayout txtnumber;
        TextInputLayout txtbank;
        MaterialSpinner spinnerbank;
        Button btnsave;
        List<string> ListBanks = new List<string>();
        List<string> ListBankCodes = new List<string>();

        ArrayAdapter<string> adapter;
        string selectedbank = "";
        string selectedcode = "";
        HelperFunctions helpers = new HelperFunctions();
        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        FirebaseDatabase database;

        string accountname, accountnumber, bank_name;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.bankdetails_dialogue, container, false);
            txtname = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutname_bank);
            txtnumber = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutnumber_bank);
            txtbank = (TextInputLayout)view.FindViewById(Resource.Id.txtlayoutbank_bank);
            spinnerbank = (MaterialSpinner)view.FindViewById(Resource.Id.spinner_bank);
            txtbank.EditText.Focusable = false;
            txtbank.EditText.Clickable = true;
            txtbank.EditText.Click += EditText_Click;

            txtbank.EditText.Text = bank_name;
            txtname.EditText.Text = accountname;
            txtnumber.EditText.Text = accountnumber;
            btnsave = (Button)view.FindViewById(Resource.Id.btnsavebank);
            btnsave.Click += Btnsave_Click;


            //InitBankList();
            //adapter = new ArrayAdapter<string>(Application.Context, Android.Resource.Layout.SimpleSpinnerDropDownItem, ListBanks);
            //adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //spinnerbank.Adapter = adapter;
            //spinnerbank.ItemSelected += Spinnerbank_ItemSelected;
            return view;
        }
        public BankDetailsDailogue(string accname, string accnumber, string bankname)
        {
            accountname = accname;
            accountnumber = accnumber;
            bank_name = bankname;
        }

        private void EditText_Click(object sender, EventArgs e)
        {
            ListDialogue lst = new ListDialogue(txtbank);
            var trans = FragmentManager.BeginTransaction();
            lst.Show(trans, "lst");
        }

        private void Spinnerbank_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
           if(e.Position!= -1)
            {
                selectedbank = ListBanks[e.Position];
                selectedcode = ListBankCodes[e.Position];
            }
        }

        private void Btnsave_Click(object sender, EventArgs e)
        {
            string accountname = txtname.EditText.Text;
            string accountnumber = txtnumber.EditText.Text;
            string bankname = txtbank.EditText.Text;
            string bankcode = txtbank.EditText.Tag.ToString();

            if (string.IsNullOrEmpty(accountname))
            {
                Toast.MakeText(Application.Context, "Please provide your account name", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(accountnumber))
            {
                Toast.MakeText(Application.Context, "Please provide your account number", ToastLength.Short).Show();
                return;
            }
            else if (string.IsNullOrEmpty(bankname))
            {
                Toast.MakeText(Application.Context, "Please select your bank", ToastLength.Short).Show();
                return;
            }
            else
            {
                OnSaveaccountDetails.Invoke(this, new OnSaveAccountDetailsEventArgs { accname = accountname, accnumber = accountnumber, bank = bankname, code = bankcode });
                this.Dismiss();
            }

           
        }

        public void InitBankList()
        {
            ListBanks.Add("Access Bank");
            ListBankCodes.Add("044");

            ListBanks.Add("Citibank");
            ListBankCodes.Add("023");

            ListBanks.Add("First Bank of Nigeria");
            ListBankCodes.Add("011");

            ListBanks.Add("Diamond Bank");
            ListBankCodes.Add("063");

            ListBanks.Add("Ecobank Nigeria");
            ListBankCodes.Add("050");

            ListBanks.Add("Fidelity Bank Nigeria");
            ListBankCodes.Add("070");

            ListBanks.Add("First City Monument Bank");
            ListBankCodes.Add("214");

            ListBanks.Add("Guaranty Trust Bank");
            ListBankCodes.Add("058");

            ListBanks.Add("Heritage Bank Plc");
            ListBankCodes.Add("030");

            ListBanks.Add("Keystone Bank Limited");
            ListBankCodes.Add("082");

            ListBanks.Add("Skye Bank");
            ListBankCodes.Add("044");

            ListBanks.Add("Stanbic IBTC Bank Nigeria Limited");
            ListBankCodes.Add("221");

            ListBanks.Add("Standard Chartered Bank");
            ListBankCodes.Add("068");

            ListBanks.Add("Sterling Bank");
            ListBankCodes.Add("232");

            ListBanks.Add("Suntrust Bank Nigeria Limited");
            ListBankCodes.Add("100");

            ListBanks.Add("Union Bank of Nigeria");
            ListBankCodes.Add("032");

            ListBanks.Add("United Bank for Africa");
            ListBankCodes.Add("033");

            ListBanks.Add("Unity Bank Plc");
            ListBankCodes.Add("215");

            ListBanks.Add("Wema Bank");
            ListBankCodes.Add("035");

            ListBanks.Add("Zenith Bank");
            ListBankCodes.Add("057");
        }

    }
}