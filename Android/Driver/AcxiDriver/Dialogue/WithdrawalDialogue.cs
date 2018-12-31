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
using Acxi.Helpers;

namespace AcxiDriver.Dialogue
{
    public class OnWithdrawEvenArgs : EventArgs
    {
        public double amount_w { get; set; }
    }
    public class WithdrawalDialogue : Android.Support.V4.App.DialogFragment
    {
        public event EventHandler<OnWithdrawEvenArgs> OnWithdrawal;
        TextInputLayout txtamount_withdraw;
        TextView txtwithdraw_bal;
        Button btncreate;

        double earning_balance = 0;

        HelperFunctions helpers = new HelperFunctions();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.withdrawal_dialogue, container, false);
            txtamount_withdraw = (TextInputLayout)view.FindViewById(Resource.Id.txtamount_withdraw);
            txtwithdraw_bal = (TextView)view.FindViewById(Resource.Id.txtbal_withdraw);
            btncreate = (Button)view.FindViewById(Resource.Id.btncreate_order);
            txtwithdraw_bal.Text = helpers.CurrencyConvert(earning_balance);

            btncreate.Click += Btncreate_Click;
            return view;
        }

        private void Btncreate_Click(object sender, EventArgs e)
        {
            if(earning_balance >= double.Parse(txtamount_withdraw.EditText.Text) && double.Parse(txtamount_withdraw.EditText.Text) >=1000)
            {
                OnWithdrawal.Invoke(Application.Context, new OnWithdrawEvenArgs { amount_w = double.Parse(txtamount_withdraw.EditText.Text) });
            }
            else if (double.Parse(txtamount_withdraw.EditText.Text) < 1000)
            {
                Toast.MakeText(Application.Context, "You can only make withdrawal above  ₦1000", ToastLength.Short).Show();

            }

            else
            {
                Toast.MakeText(Application.Context, "You can only withdraw an amount lesser than your balance", ToastLength.Short).Show();
            }
        }

        public WithdrawalDialogue(double balance)
        {
            earning_balance = balance;
        }
    }
}