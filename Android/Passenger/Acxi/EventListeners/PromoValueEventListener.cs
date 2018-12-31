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

namespace Acxi.EventListeners
{
    public class PromoValueEventListener : Java.Lang.Object, IValueEventListener
    {
        string type;
        string phone;
        public class PromoValidEventArgs : EventArgs
        {
            public string amount { get; set; }
        }
        public event EventHandler PromoInvalid;

        public event EventHandler<PromoValidEventArgs> PromoValid;

        public event EventHandler PromoGeneralused;

        public void OnCancelled(DatabaseError error)
        {
            //
        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Value != null)
            {
                if (type == "general")
                {

                    if (snapshot.Child("users").Child(phone).Value == null)
                    {

                        if (snapshot.Child("amount").Value != null)
                        {
                            string mamount = snapshot.Child("amount").Value.ToString();
                            PromoValid.Invoke(this, new PromoValidEventArgs { amount = mamount });
                        }
                        else
                        {
                            PromoInvalid.Invoke(this, new EventArgs());
                        }

                    }
                    else
                    {
                        PromoGeneralused?.Invoke(this, new EventArgs());
                    }

                }
                else
                {
                    PromoValid.Invoke(this, new PromoValidEventArgs { amount = snapshot.Value.ToString() });
                }

            }
            else
            {
                PromoInvalid.Invoke(this, new EventArgs());
            }
        }

        public PromoValueEventListener(string promotype, string mphone)
        {
            type = promotype;
            phone = mphone;
        }
    }
}