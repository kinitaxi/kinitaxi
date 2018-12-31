using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AcxiDriver.Adapter;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AcxiDriver.Dialogue
{
    public class ListDialogueMake : Android.Support.V4.App.DialogFragment
    {
        private ListView mListView;
        private List<string> ListOfMake = new List<string>();
        TextInputLayout txtwhich;
        List<string> ListBankCodes = new List<string>();

        public ListDialogueMake(TextInputLayout txtsender)
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
            TextView txtheader = (TextView)view.FindViewById(Resource.Id.txtDialog_header);
            txtheader.Text = "Select Make";
            StringListAdapter adapter = new StringListAdapter(Application.Context, ListOfMake);
            mListView.Adapter = adapter;
            mListView.ItemClick += MListView_ItemClick; ;

            return view;
        }

        private void MListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            txtwhich.EditText.Text = ListOfMake[e.Position];
            // txtwhich.EditText.Tag = ListBankCodes[e.Position];
            this.Dismiss();
        }

        public void InitBankList()
        {
            string[] makes = {"Toyota", "Honda","Nissan", "Mercedes-Benz", "Hyundai", "Ford", "Acura", "Audi", "BMW", "Kia", "Lexus", "Mazda", "Chevrolet", "Volkswagen", "Infiniti", "Mitsubishi", "Land Rover", "Peugeot",
                "Chrysler", "Daewoo", "Dodge", "Isuzu", "Jaguar", "Lincoln", "Pontiac", "Porsche", "Renault", "Suzuki", "Volvo"};
            ListOfMake = makes.ToList<string>();
        }


    }
}