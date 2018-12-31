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
using Firebase.Database;
using Acxi.Helpers;

namespace Acxi.EventListeners
{
    public class ProfileValueEventListener : Java.Lang.Object, IValueEventListener
    {
        HelperFunctions helpers = new HelperFunctions();
        public event EventHandler<WalletBalanceEventArgs> OnWalletBalance;
        public event EventHandler<AddressesEventArgs> SavedPlacesAddress;
        public event EventHandler<CardsEventArgs> SavedCards;
        public event EventHandler<OnTripEventArgs> isTripOngoing;
        public event EventHandler UserAccountDeleted;
        public event EventHandler UserAccountVerified;

        public class OnTripEventArgs : EventArgs
        {
            public string ongoing { get; set; }
        }
        public class WalletBalanceEventArgs : EventArgs
        {
            public string promo_wallet { get; set; }
            public string ride_wallet { get; set; }
        }


        public class AddressesEventArgs : EventArgs
        {
            public string homeaddress { get; set; }
            public string homelat { get; set; }
            public string homelng { get; set; }
            public string workaddress { get; set; }
            public string worklat { get; set; }
            public string worklng { get; set; }
            public string others { get; set; }
        }
        public class CardsEventArgs : EventArgs
        {
            public string card1 { get; set; }
            public string card2 { get; set; }
            public string card3 { get; set; }
            public string preferred_card { get; set; }

        }
        public void OnCancelled(DatabaseError error)
        {
            //
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Child("wallet").Value != null)
            {
                string promowallet = "";
                string ride_wallet = "";
                if (snapshot.Child("wallet").Child("promo_wallet").Value != null)
                {
                    promowallet = snapshot.Child("wallet").Child("promo_wallet").Value.ToString();
                }

                if (snapshot.Child("wallet").Child("ride_wallet").Value != null)
                {
                    ride_wallet = snapshot.Child("wallet").Child("ride_wallet").Value.ToString();
                }

                OnWalletBalance.Invoke(this, new WalletBalanceEventArgs { ride_wallet = ride_wallet, promo_wallet = promowallet });
            }


            if (snapshot.Child("savedplaces").Value != null)
            {

                string homeaddress = "";
                string homelat = "";
                string homelng = "";
                string workaddress = "";
                string worklat = "";
                string worklng = "";
                string others = "";

                if (snapshot.Child("savedplaces").Child("home").Value != null)
                {
                    homeaddress = snapshot.Child("savedplaces").Child("home").Child("address").Value.ToString();
                    homelat = snapshot.Child("savedplaces").Child("home").Child("latitude").Value.ToString();
                    homelng = snapshot.Child("savedplaces").Child("home").Child("longitude").Value.ToString();
                }

                if (snapshot.Child("savedplaces").Child("work").Value != null)
                {
                    workaddress = snapshot.Child("savedplaces").Child("work").Child("address").Value.ToString();
                    worklat = snapshot.Child("savedplaces").Child("work").Child("latitude").Value.ToString();
                    worklng = snapshot.Child("savedplaces").Child("work").Child("longitude").Value.ToString();
                }

                if (snapshot.Child("savedplaces").Child("others").Value != null)
                {
                    others = snapshot.Child("savedplaces").Child("others").Value.ToString();
                }

                SavedPlacesAddress.Invoke(this, new AddressesEventArgs { homeaddress = homeaddress, homelat = homelat, homelng = homelng, workaddress = workaddress, worklat = worklat, worklng = worklng, others = others });
            }

            if (snapshot.Child("cards").Value != null)
            {
                string card1 = "", card2 = "", card3 = "", prefered_card = "";

                if (snapshot.Child("cards").Child("card1").Value != null)
                {
                    card1 = helpers.Decrypt(snapshot.Child("cards").Child("card1").Value.ToString());
                }

                if (snapshot.Child("cards").Child("card2").Value != null)
                {
                    card2 = helpers.Decrypt(snapshot.Child("cards").Child("card2").Value.ToString());
                }

                if (snapshot.Child("cards").Child("card3").Value != null)
                {
                    card3 = helpers.Decrypt(snapshot.Child("cards").Child("card3").Value.ToString());
                }

                if (snapshot.Child("cards").Child("preferred_card").Value != null)
                {
                    prefered_card = snapshot.Child("cards").Child("preferred_card").Value.ToString();
                }

                SavedCards.Invoke(this, new CardsEventArgs { card1 = card1, card2 = card2, card3 = card3, preferred_card = prefered_card });
            }

            if (snapshot.Child("ongoing").Value != null)
            {
                isTripOngoing.Invoke(this, new OnTripEventArgs { ongoing = snapshot.Child("ongoing").Value.ToString() });
            }

            if (snapshot.Child("created_at").Value == null && snapshot.Child("email").Value == null && snapshot.Child("first_name").Value == null)
            {
                UserAccountDeleted.Invoke(this, new EventArgs());
            }
            else
            {
                UserAccountVerified.Invoke(this, new EventArgs());
            }

        }
    }
}