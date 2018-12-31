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
using AcxiDriver.Adapter;

namespace AcxiDriver.Dialogue
{
    public class ListDialogue : Android.Support.V4.App.DialogFragment
    {
        private ListView mListView;
        private List<string> ListBanks = new List<string>();
        TextInputLayout txtwhich;
        List<string> ListBankCodes = new List<string>();

        public ListDialogue(TextInputLayout txtsender)
        {
            txtwhich = txtsender;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.stringdialogue, container, false);

            InitBankList();
            mListView = view.FindViewById<ListView>(Resource.Id.Listview1);
            StringListAdapter adapter = new StringListAdapter(Application.Context, ListBanks);
            mListView.Adapter = adapter;
            mListView.ItemClick += MListView_ItemClick; ;

            return view;
        }

        private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            txtwhich.EditText.Text = ListBanks[e.Position];
            txtwhich.EditText.Tag = ListBankCodes[e.Position];
            this.Dismiss();
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