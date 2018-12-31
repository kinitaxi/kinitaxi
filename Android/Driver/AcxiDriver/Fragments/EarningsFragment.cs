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
using Firebase.Database;
using Firebase;
using Acxi.Helpers;
using Java.Util;
using System.Threading.Tasks;
using System.Threading;
using AcxiDriver.EventListeners;
using AcxiDriver.Activities;
using AcxiDriver.DataModels;
using Newtonsoft.Json;

namespace AcxiDriver.Fragments
{
    public class EarningsFragment : Android.Support.V4.App.Fragment
    {
        Button btnwithdraw;

        TextView txttotal_earnings;
        TextView txtbalance_earnings;
        TextView txtpending_wthdrwal;
        TextView txttripcount;
        LinearLayout btntrips;

        WithdrawalDialogue withdrawal_dialogue;
        EarningsValueEventListener earningslistener;
        PendingWithrawalValuEventListener withdrawal_listener;
        FirebaseDatabase database;
        DatabaseReference earnref;
        DatabaseReference withdrawref;
        DatabaseReference Tripsref;
        TripCountValueEventListener trips_listener;
        TicketKeysValueEventListener ticketkeys_listener;
        TicketDetailsValueEventListener ticketdetails_listener;

        List<TicketDetailsFull> ListSupportDetails = new List<TicketDetailsFull>();
        List<string> SupportKeys;
        string phone;

       
        double earning_overall = 0;
        double earning_balance = 0;
        double pending_withdrawal = 0;

        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        HelperFunctions helpers = new HelperFunctions();
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            edit = pref.Edit();
            phone = pref.GetString("phone", "");

            database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
            earnref = database.GetReference("driverEarnings/" + phone);

            earningslistener = new EarningsValueEventListener();
            earnref.AddValueEventListener(earningslistener);
            earningslistener.EarningChanged += Earningslistener_EarningChanged;

            withdrawref = database.GetReference("withdrawalRequests/" + phone);
            withdrawal_listener = new PendingWithrawalValuEventListener();
            withdrawref.AddValueEventListener(withdrawal_listener);
            withdrawal_listener.PendingChanged += Withdrawal_listener_PendingChanged;
            
            trips_listener = new TripCountValueEventListener();
            Tripsref = database.GetReference("drivers/" + phone + "/trips");
            Tripsref.AddValueEventListener(trips_listener);
            trips_listener.OnTripretrieved += Trips_listener_OnTripretrieved;
            // Query query = Tripsref.OrderByValue().EqualTo("");

            GetSupportKeys();
        }

      

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.earnings, container, false);
            txtbalance_earnings = (TextView)view.FindViewById(Resource.Id.txtbalance);
            txttotal_earnings = (TextView)view.FindViewById(Resource.Id.txttotalearnings);
            txtbalance_earnings = (TextView)view.FindViewById(Resource.Id.txtbalance);
            txtpending_wthdrwal = (TextView)view.FindViewById(Resource.Id.txtpending_withdrawal);

            txttripcount = (TextView)view.FindViewById(Resource.Id.txttripcount);
            btntrips = (LinearLayout)view.FindViewById(Resource.Id.laytrips);

            btntrips.Click += Btntrips_Click;
            btnwithdraw = (Button)view.FindViewById(Resource.Id.btnwithdraw);
            btnwithdraw.Click += Btnwithdraw_Click;
            txtpending_wthdrwal.Click += Txtpending_wthdrwal_Click;
          
            return view;
        }

        private void Btntrips_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(TripsActivity)));
        }

        private void Txtpending_wthdrwal_Click(object sender, EventArgs e)
        {
            if (pending_withdrawal > 0)
            {
                AppAlertDialogue alert = new AppAlertDialogue("Cancel pending withdrawal");
                var trans = FragmentManager.BeginTransaction();
                alert.Show(trans, "alert");

                alert.AlertOk += (o, g) =>
                {
                    if (withdrawref != null)
                    {
                        earning_balance += pending_withdrawal;
                        earnref.Child("earning_unpaid").SetValue(earning_balance.ToString());
                        withdrawref.RemoveValue();
                        alert.Dismiss();
                    }
                };
                alert.AlertCancel += (o, j) =>
                {
                    alert.Dismiss();
                };
            }
           
        }

        private void Trips_listener_OnTripretrieved(object sender, TripCountValueEventListener.TripCountEventArgs e)
        {
            if(txttripcount != null)
            {
                txttripcount.Text = e.Trips.ToString();
            }
        }
        private void Earningslistener_EarningChanged(object sender, EarningsValueEventListener.OnEarningValue e)
        {
            earning_overall = e.earningOverall;
            earning_balance = e.earningBalance;

            txttotal_earnings.Text = helpers.CurrencyConvert(e.earningOverall);
            txtbalance_earnings.Text = helpers.CurrencyConvert(e.earningBalance);
        }


        private void Withdrawal_listener_PendingChanged(object sender, PendingWithrawalValuEventListener.PendingEventArgs e)
        {
            pending_withdrawal = e.amount;
            txtpending_wthdrwal.Text = helpers.CurrencyConvert(e.amount);
        }


        private void Btnwithdraw_Click(object sender, EventArgs e)
        {
            if(earning_balance >= 1000 )
            {
                if(pending_withdrawal == 0)
                {
                    withdrawal_dialogue = new WithdrawalDialogue(earning_balance);
                    var trans = FragmentManager.BeginTransaction();
                    withdrawal_dialogue.Show(trans, "withdraw");
                    withdrawal_dialogue.OnWithdrawal += Withdrawal_dialogue_OnWithdrawal;
                }
                else
                {
                    Toast.MakeText(Application.Context, "Your have a pending withdrawal order", ToastLength.Long).Show();
                }

            }
            else
            {
                Toast.MakeText(Application.Context, "Your balance need to be at least ₦1000", ToastLength.Long).Show();
            }
           
        }

        private void Withdrawal_dialogue_OnWithdrawal(object sender, OnWithdrawEvenArgs e)
        {
            if (withdrawal_dialogue != null)
            {
                withdrawal_dialogue.Dismiss();
            }

            if(e.amount_w >= 1000)
            {
                string timestamp = helpers.GetTimeStampNow().ToString();
                HashMap map = new HashMap();
                map.Put("created_at", timestamp);
                map.Put("amount", e.amount_w.ToString());

                if (withdrawref != null)
                {
                    withdrawref.SetValue(map);
                    earning_balance -= e.amount_w;
                    earnref.Child("earning_unpaid").SetValue(earning_balance.ToString());
                }
            }
           
        }

        public void GetSupportKeys()
        {
            DatabaseReference keyref = database.GetReference("drivers/" + phone + "/tickets");
            ticketkeys_listener = new TicketKeysValueEventListener();
            keyref.AddValueEventListener(ticketkeys_listener);
            ticketkeys_listener.OnTickeyKeys += Ticketkeys_listener_OnTickeyKeys;
            ticketkeys_listener.TicketEmpty += Ticketkeys_listener_TicketEmpty;
        }

        private void Ticketkeys_listener_TicketEmpty(object sender, EventArgs e)
        {
            edit.PutString("support", "");
            edit.Apply();
        }

        private void Ticketkeys_listener_OnTickeyKeys(object sender, TicketKeysValueEventListener.TicketKeysEventArgs e)
        {
            SupportKeys = e.listKey;
            ListSupportDetails.Clear();

            if(SupportKeys.Count == 0)
            {
                edit.PutString("support", "");
                edit.Apply();
            }
               foreach(string supportkey in SupportKeys)
            {
                DatabaseReference thisticketRef = database.GetReference("driversSupport/" + supportkey);
                ticketdetails_listener = new TicketDetailsValueEventListener();
                thisticketRef.AddListenerForSingleValueEvent(ticketdetails_listener);
                ticketdetails_listener.OnTicketDetails += Ticketdetails_listener_OnTicketDetails;

            }
        }

        private void Ticketdetails_listener_OnTicketDetails(object sender, TicketDetailsValueEventListener.TicketDetailsEventArgs e)
        {
            ListSupportDetails.Add(new TicketDetailsFull { category = e.category, created_at = e.created_at, message = e.message, status = e.status, title = e.title });
            if(ListSupportDetails.Count == SupportKeys.Count)
            {
                var list  = ListSupportDetails.OrderByDescending(o => o.created_at).ToList();
                string jstr = JsonConvert.SerializeObject(list);
                edit.PutString("support", jstr);
                edit.Apply();
            }

            
        }
    }

}