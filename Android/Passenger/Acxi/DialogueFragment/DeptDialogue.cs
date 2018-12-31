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
using Acxi.Helpers;

namespace Acxi.DialogueFragment
{
    public class DeptDialogue : Android.Support.V4.App.DialogFragment
    {
        double debt;
        TextView txtdept;
        Button btnproceed;
        HelperFunctions helpers = new HelperFunctions();
        public event EventHandler OnDebtDeclined;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.dept_dialogue, container, false);

            txtdept =(TextView) view.FindViewById(Resource.Id.txtdept);
            txtdept.Text = "You have an oustanding charges of " + helpers.CurrencyConvert(debt);
            btnproceed = (Button)view.FindViewById(Resource.Id.btnproceed_debt);
            btnproceed.Click += Btnproceed_Click;
            return view;
        }

        public DeptDialogue (double amount_owed)
        {
            debt = amount_owed; 
        }

        private void Btnproceed_Click(object sender, EventArgs e)
        {
            OnDebtDeclined.Invoke(this, new EventArgs());
            this.Dismiss();
        }
    }
}