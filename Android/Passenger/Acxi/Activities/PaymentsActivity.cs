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
using Calligraphy;
using Acxi.DialogueFragment;
using Android.Graphics;
using System.Threading.Tasks;
using System.Threading;
using Acxi.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Firebase.Database;
using Firebase;
using Plugin.Connectivity;

namespace Acxi.Activities
{
    [Activity(Label = "Payments", Theme = "@style/AcxiTheme1", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class PaymentsActivity : AppCompatActivity
    {
        SupportWidget.Toolbar mToolbar;
        LinearLayout btnaddcard;
        LinearLayout btnaddcard1;
        LinearLayout btnaddcard2;

        ImageView imgcard;
        ImageView imgcard1;
        ImageView imgcard2;
        ImageView imgcash;

        TextView txtcardtext;
        TextView txtcardtext1;
        TextView txtcardtext2;

        AddCardDialogue addcard_dialogue;
        OtpDialogue otp_dialogue;
        string cardadd_response = "";
        string otpconfirm_response = "";

        HelperFunctions helpers = new HelperFunctions();
        WebRequestHelpers webhelpers = new WebRequestHelpers();

        //APP DATA
        ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor edit;

        FirebaseDatabase database;
        string phone_s;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
                .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                 .Build());

            SetContentView(Resource.Layout.payments);

            mToolbar = (SupportWidget.Toolbar)FindViewById(Resource.Id.paymentsToolbar);
            SetSupportActionBar(mToolbar);
            SupportActionBar.Title = "Payments";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_action_arrow_back);

            btnaddcard = (LinearLayout)FindViewById(Resource.Id.lay_addcard);
            imgcard = (ImageView)FindViewById(Resource.Id.imgcard);
            txtcardtext = (TextView)FindViewById(Resource.Id.txtcardtext);

            btnaddcard1 = (LinearLayout)FindViewById(Resource.Id.lay_addcard1);
            imgcard1 = (ImageView)FindViewById(Resource.Id.imgcard1);
            txtcardtext1 = (TextView)FindViewById(Resource.Id.txtcardtext1);

            btnaddcard2 = (LinearLayout)FindViewById(Resource.Id.lay_addcard2);
            imgcard2 = (ImageView)FindViewById(Resource.Id.imgcard2);
            txtcardtext2 = (TextView)FindViewById(Resource.Id.txtcardtext2);
            imgcash = (ImageView)FindViewById(Resource.Id.imgcash_addcard);

            txtcardtext.Click += Txtcardtext_Click;
            txtcardtext1.Click += Txtcardtext1_Click;
            txtcardtext2.Click += Txtcardtext2_Click;

            btnaddcard.Click += Txtcardtext_Click;
            btnaddcard1.Click += Txtcardtext1_Click;
            btnaddcard2.Click += Txtcardtext2_Click;

            imgcard.Click += Imgcard_Click;
            imgcard1.Click += Imgcard1_Click;
            imgcard2.Click += Imgcard2_Click;

            edit = pref.Edit();
            phone_s = pref.GetString("phone", "");
            try
            {
                database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
            }
            catch
            {

            }
            DisplayCards();
        }

        public void DisplayCards()
        {
            string card1 = pref.GetString("card1", "");
            string card2 = pref.GetString("card2", "");
            string card3 = pref.GetString("card3", "");
            string preferred_card = pref.GetString("preferred_card", "");

            if (!string.IsNullOrEmpty(card1))
            {
                // txtcardtext.Text = f.cardnumber.Substring(0, 4) + "************" + f.cardnumber.Substring(f.cardnumber.Length - 4, 4);
                var deser = JObject.Parse(card1);
                string cardnumber1 = deser["card_no"].ToString();
                txtcardtext.Text = cardnumber1.Substring(0, 4) + "************" + cardnumber1.Substring(cardnumber1.Length - 4, 4);
            }

            if (!string.IsNullOrEmpty(card2))
            {
                var deser = JObject.Parse(card2);
                string cardnumber2 = deser["card_no"].ToString();
                txtcardtext1.Text = cardnumber2.Substring(0, 4) + "************" + cardnumber2.Substring(cardnumber2.Length - 4, 4);
            }

            if (!string.IsNullOrEmpty(card3))
            {
                var deser = JObject.Parse(card3);
                string cardnumber3 = deser["card_no"].ToString();
                txtcardtext2.Text = cardnumber3.Substring(0, 4) + "************" + cardnumber3.Substring(cardnumber3.Length - 4, 4);
            }

            if (preferred_card == "card1")
            {
                imgcard.SetColorFilter(Color.Rgb(70, 188, 82));
                imgcash.SetColorFilter(Color.Rgb(243, 243, 243));
            }
            else if (preferred_card == "card2")
            {
                imgcard1.SetColorFilter(Color.Rgb(70, 188, 82));
                imgcash.SetColorFilter(Color.Rgb(243, 243, 243));
            }
            else if (preferred_card == "card3")
            {
                imgcard2.SetColorFilter(Color.Rgb(70, 188, 82));
                imgcash.SetColorFilter(Color.Rgb(243, 243, 243));
            }
        }
        private void Imgcard2_Click(object sender, EventArgs e)
        {
            if (txtcardtext2.Text.Contains("***"))
            {
                imgcard2.SetColorFilter(Color.Rgb(70, 188, 82));
                imgcard.SetColorFilter(Color.Rgb(243, 243, 243));
                imgcard1.SetColorFilter(Color.Rgb(243, 243, 243));
                imgcash.SetColorFilter(Color.Rgb(243, 243, 243));

                edit.PutString("preferred_card", "card3");
                edit.Apply();

                DatabaseReference prefcard_ref = database.GetReference("users/" + phone_s + "/cards/preferred_card");
                prefcard_ref.SetValue("card3");
            }
            else
            {
                Toast.MakeText(this, "Please provide card details", ToastLength.Short).Show();
            }
        }

        private void Imgcard1_Click(object sender, EventArgs e)
        {
            if (txtcardtext1.Text.Contains("***"))
            {
                imgcard1.SetColorFilter(Color.Rgb(70, 188, 82));
                imgcard.SetColorFilter(Color.Rgb(243, 243, 243));
                imgcard2.SetColorFilter(Color.Rgb(243, 243, 243));
                imgcash.SetColorFilter(Color.Rgb(243, 243, 243));
                edit.PutString("preferred_card", "card2");
                edit.Apply();

                DatabaseReference prefcard_ref = database.GetReference("users/" + phone_s + "/cards/preferred_card");
                prefcard_ref.SetValue("card2");
            }
            else
            {
                Toast.MakeText(this, "Please provide card details", ToastLength.Short).Show();
            }
        }

        private void Imgcard_Click(object sender, EventArgs e)
        {

            if (txtcardtext.Text.Contains("***"))
            {
                imgcard.SetColorFilter(Color.Rgb(70, 188, 82));
                imgcard1.SetColorFilter(Color.Rgb(243, 243, 243));
                imgcard2.SetColorFilter(Color.Rgb(243, 243, 243));
                imgcash.SetColorFilter(Color.Rgb(243, 243, 243));
                edit.PutString("preferred_card", "card1");
                edit.Apply();

                DatabaseReference prefcard_ref = database.GetReference("users/" + phone_s + "/cards/preferred_card");
                prefcard_ref.SetValue("card1");
            }
            else
            {
                Toast.MakeText(this, "Please provide card details", ToastLength.Short).Show();
            }
        }

        public void AlertInit(string body)
        {
            AppAlertDialogue appAlert = new AppAlertDialogue(body);
            appAlert.Cancelable = true;
            var trans1 = SupportFragmentManager.BeginTransaction();
            appAlert.Show(trans1, "appalert");

            appAlert.AlertCancel += (i, h) =>
            {
                appAlert.Dismiss();
                appAlert = null;
            };

            appAlert.AlertOk += (y, t) =>
            {
                appAlert.Dismiss();
                appAlert = null;
            };

        }

        public void SaveCard(TextView txt, string whichcard)
        {
            Dictionary<string, string> thisCard = new Dictionary<string, string>();
            addcard_dialogue = new AddCardDialogue();
            var trans = SupportFragmentManager.BeginTransaction();
            addcard_dialogue.Show(trans, "add_card");
            addcard_dialogue.SaveCardDetails += async (o, f) =>
            {
                addcard_dialogue.Dismiss();

                if (!CrossConnectivity.Current.IsConnected)
                {
                    AlertInit("Internet connectivity is not available");
                    return;
                }

                ProgressDialog progress = new ProgressDialog(this);
                progress.Indeterminate = true;
                progress.SetProgressStyle(ProgressDialogStyle.Spinner);
                progress.SetMessage("Please wait...");
                progress.SetCancelable(false);


                progress.Show();

                await Task.Run(() =>
                {
                    cardadd_response = webhelpers.AddCard(f.cardnumber, f.cvv, f.expmonth, f.expyear, f.pin);
                });
                progress.Dismiss();

                if (cardadd_response.Length > 200)
                {
                    AlertInit("Oops! something went erong please try again");
                    return;
                }

                if (string.IsNullOrEmpty(cardadd_response))
                {
                    AlertInit("Oops! something went erong please try again");
                    return;
                }

                char[] MyChar = { '"' };
                cardadd_response = cardadd_response.Trim(MyChar);
                Console.WriteLine(cardadd_response);
                thisCard.Add("card_no", f.cardnumber);
                thisCard.Add("cvv", f.cvv);
                thisCard.Add("expiry_month", f.expmonth);
                thisCard.Add("expiry_year", f.expyear);
                thisCard.Add("pin", f.pin);


                if (cardadd_response.Contains("failed"))
                {
                    Toast.MakeText(this, "Unable to authorize card, please try another card", ToastLength.Short).Show();
                    return;
                }

                var real = JObject.Parse(cardadd_response);
                string reference = "";
                string status = "";

                if (!string.IsNullOrEmpty(real["ref"].ToString()))
                {
                    reference = real["ref"].ToString();
                }
                if (!string.IsNullOrEmpty(real["status"].ToString()))
                {
                    status = real["status"].ToString();
                }


                if (status == "send_otp" || status == "send_phone" || status == "send_birthday")
                {
                    otp_dialogue = new OtpDialogue(status);
                    var trans1 = SupportFragmentManager.BeginTransaction();
                    otp_dialogue.Cancelable = false;
                    otp_dialogue.Show(trans1, "OTP");
                    otp_dialogue.OnOtpProvided += async (t, k) =>
                    {
                        progress.Show();
                        await Task.Run(() =>
                        {
                            otpconfirm_response = webhelpers.CompleteAddCard(reference, status, k.status_value);
                        });


                        if (string.IsNullOrEmpty(otpconfirm_response))
                        {
                            Toast.MakeText(this, "Something went wrong, please try again", ToastLength.Short).Show();
                            return;
                        }

                        otpconfirm_response = otpconfirm_response.Trim('"');
                        if (otpconfirm_response == "failed")
                        {
                            progress.Dismiss();
                            AlertInit("Unable to authorize card, please try another card");

                            return;
                        }
                        else if (otpconfirm_response.Contains("reusable"))
                        {
                            progress.Dismiss();
                            AlertInit("Unable to authorize card, please try another card");
                            return;
                        }
                        else if (otpconfirm_response.Length > 15)
                        {
                            progress.Dismiss();
                            AlertInit("Unable to authorize card, please try another card");
                            return;
                        }


                        //SAVE CARD
                        if (!string.IsNullOrEmpty(otpconfirm_response))
                        {
                            Toast.MakeText(this, "Card details saved successfully", ToastLength.Short).Show();
                            txt.Text = f.cardnumber.Substring(0, 4) + "************" + f.cardnumber.Substring(f.cardnumber.Length - 4, 4);
                            if (thisCard.Count > 0)
                            {
                                thisCard.Add("auth_code", otpconfirm_response);
                                string cardinfo = JsonConvert.SerializeObject(thisCard);
                                edit.PutString(whichcard, cardinfo);
                                edit.Apply();

                                string card_encrypted = helpers.Encrypt(cardinfo);
                                if (database != null)
                                {
                                    DatabaseReference cardref = database.GetReference("users/" + phone_s + "/cards/" + whichcard);
                                    cardref.SetValue(card_encrypted);
                                }
                            }

                            progress.Dismiss();
                        }
                        else
                        {
                            progress.Dismiss();
                            AlertInit("Unable to authorize card, please try another card");
                            return;
                        }

                        progress.Dismiss();
                    };

                }
                else if (status == "send_pin")
                {
                    progress.Show();
                     await Task.Run(() =>
                     {
                    otpconfirm_response = webhelpers.CompleteAddCard(reference, status, f.pin);
                    });

                    if(otpconfirm_response.Length > 200)
                    {
                        Toast.MakeText(this, "Something went wrong, please try again", ToastLength.Short).Show();
                        return;
                    }

                    if (string.IsNullOrEmpty(otpconfirm_response))
                    {
                        Toast.MakeText(this, "Something went wrong, please try again", ToastLength.Short).Show();
                        return;
                    }

                    otpconfirm_response = otpconfirm_response.Trim('"');
                    if (otpconfirm_response == "failed")
                    {
                        progress.Dismiss();
                        AlertInit("Unable to authorize card, please try another card");
                        return;
                    }
                    else if (otpconfirm_response.Contains("reusable"))
                    {
                        progress.Dismiss();
                        AlertInit("Unable to authorize card, please try another card");
                        return;
                    }
                    else if (otpconfirm_response.Length > 15)
                    {
                        progress.Dismiss();
                        AlertInit("Unable to authorize card, please try another card");
                        return;
                    }


                    //SAVE CARD
                    if (!string.IsNullOrEmpty(otpconfirm_response))
                    {
                        Toast.MakeText(this, "Card details saved successfully", ToastLength.Short).Show();
                        txt.Text = f.cardnumber.Substring(0, 4) + "************" + f.cardnumber.Substring(f.cardnumber.Length - 4, 4);
                        if (thisCard.Count > 0)
                        {
                            thisCard.Add("auth_code", otpconfirm_response);
                            string cardinfo = JsonConvert.SerializeObject(thisCard);
                            edit.PutString(whichcard, cardinfo);
                            edit.Apply();

                            string card_encrypted = helpers.Encrypt(cardinfo);
                            if (database != null)
                            {
                                DatabaseReference cardref = database.GetReference("users/" + phone_s + "/cards/" + whichcard);
                                cardref.SetValue(card_encrypted);
                            }
                        }

                        progress.Dismiss();
                    }
                    else
                    {
                        progress.Dismiss();
                        AlertInit("Unable to authorize card, please try another card");
                        return;
                    }

                    progress.Dismiss();
                }
                else if (status == "success")
                {
                    string auth_code = "";
                    if (!string.IsNullOrEmpty(real["auth_code"].ToString()))
                    {
                        auth_code = real["auth_code"].ToString();
                        Toast.MakeText(this, "Card details saved successfully", ToastLength.Short).Show();
                        txt.Text = f.cardnumber.Substring(0, 4) + "************" + f.cardnumber.Substring(f.cardnumber.Length - 4, 4);
                        if (thisCard.Count > 0)
                        {
                            thisCard.Add("auth_code", auth_code);
                            string cardinfo = JsonConvert.SerializeObject(thisCard);
                            edit.PutString(whichcard, cardinfo);
                            edit.Apply();


                            string card_encrypted = helpers.Encrypt(cardinfo);
                            if (database != null)
                            {
                                DatabaseReference cardref = database.GetReference("users/" + phone_s + "/cards/" + whichcard);
                                cardref.SetValue(card_encrypted);
                            }
                        }
                    }

                }
                else
                {
                    progress.Dismiss();
                    AlertInit("Unable to authorize card, please try another card");
                    return;
                }


            };


        }
        private void Txtcardtext2_Click(object sender, EventArgs e)
        {
            if (txtcardtext2.Text.Contains("card"))
            {
                SaveCard(txtcardtext2, "card3");
            }
        }

        private void Txtcardtext1_Click(object sender, EventArgs e)
        {
            if (txtcardtext1.Text.Contains("card"))
            {
                SaveCard(txtcardtext1, "card2");
            }
        }

        private void Txtcardtext_Click(object sender, EventArgs e)
        {
            if (txtcardtext.Text.Contains("card"))
            {
                SaveCard(txtcardtext, "card1");
            }
            
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