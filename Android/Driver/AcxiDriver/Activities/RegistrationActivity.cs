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
using UK.CO.Chrisjenx.Calligraphy;
using AcxiDriver.Dialogue;
using Java.IO;
using Android.Graphics;
using Android.Provider;
using Android.Content.PM;
using Acxi.Helpers;
using Firebase.Database;
using Firebase;
using System.Threading.Tasks;
using System.Threading;
using Android.Database;
using Java.Util;
using Plugin.Media;

namespace AcxiDriver.Activities
{
    [Activity(Label = "RegistrationActivity",MainLauncher =false, Theme ="@style/AcxiTheme1")]
    public class RegistrationActivity : AppCompatActivity, IValueEventListener
    {
        LinearLayout btnlicense;
        LinearLayout btnprofilepix;
        LinearLayout btnworthiness;
        LinearLayout btnpersonaldetails;
        LinearLayout btnvehicle;

        ImageView imgcheckpix;
        ImageView imgcheckpersonaldetails;
        ImageView imgcheckvehicle;
        ImageView imgchecklicense;
        ImageView imgcheckworthiness;

        Button btnregister;

        DocumentUploadDialogue documentdialogue;
        PersonalProfileRegistrationDialogue profiledialogue;
        VehicleDetailsDialogue vehicledialogue;
        Android.Support.V7.Widget.Toolbar mtoolbar;

        string phoneID;
        //TRACKS WHICH IMAGE WAS TAKEN FROM ACTIVITY;
        string whichimage = "";
        public static readonly int PickImageId = 1000;

        FirebaseDatabase database;

        ISharedPreferences pref = Application.Context.GetSharedPreferences("user", FileCreationMode.Private);
        ISharedPreferencesEditor edit;
        bool isvehicle =false, isprofilepix= false, ispersonal =false, isworthiness= false, islicense = false;
        WebRequestHelpers webHelpers = new WebRequestHelpers();
        HelperFunctions helpers = new HelperFunctions();
        string firstname_s, lastname_s, phone_s, email_s;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CalligraphyConfig.InitDefault(new CalligraphyConfig.Builder()
     .SetDefaultFontPath("Fonts/Lato-Regular.ttf")
  .SetFontAttrId(Resource.Attribute.fontPath)
  .Build());

            SetContentView(Resource.Layout.registration);
            mtoolbar = (Android.Support.V7.Widget.Toolbar)FindViewById(Resource.Id.registrationToolbar);
            SetSupportActionBar(mtoolbar);
            SupportActionBar.Title = "Driver Registration";
            Android.Support.V7.App.ActionBar ab = SupportActionBar;
            ab.SetDisplayHomeAsUpEnabled(true);
            ab.SetHomeAsUpIndicator(Resource.Mipmap.ic_arrow_back_white);

            imgchecklicense = (ImageView)FindViewById(Resource.Id.imgchecklicense_reg);
            imgcheckpersonaldetails = (ImageView)FindViewById(Resource.Id.imgcheckpersonal_reg);
            imgcheckpix = (ImageView)FindViewById(Resource.Id.imgcheckpicture_reg);
            imgcheckvehicle = (ImageView)FindViewById(Resource.Id.imgcheckvehicle_reg);
            imgcheckworthiness = (ImageView)FindViewById(Resource.Id.imgcheckworthiness_reg);

            btnregister = (Button)FindViewById(Resource.Id.btncomplete_reg);
            btnlicense = (LinearLayout)FindViewById(Resource.Id.laylicense_reg);
            btnprofilepix = (LinearLayout)FindViewById(Resource.Id.layprofilepix_reg);
            btnworthiness = (LinearLayout)FindViewById(Resource.Id.layworthiness_reg);
            btnpersonaldetails = (LinearLayout)FindViewById(Resource.Id.layprofiledetails_reg);
            btnvehicle = (LinearLayout)FindViewById(Resource.Id.layvehicledetails_reg);
            btnregister.Click += Btnregister_Click;
            btnlicense.Click += Btnlicense_Click;
            btnprofilepix.Click += Btnprofilepix_Click;
            btnworthiness.Click += Btnworthiness_Click;
            btnpersonaldetails.Click += Btnpersonaldetails_Click;
            btnvehicle.Click += Btnvehicle_Click;

          //  phoneID = pref.GetString("phone", )
            phoneID = Intent.GetStringExtra("phone");
            if (string.IsNullOrEmpty(phoneID))
            {
                phoneID = pref.GetString("phone", "");
            }
            edit = pref.Edit();
            SetUpFirebase();
            DocumentStatus();
        }

        private async void Btnregister_Click(object sender, EventArgs e)
        {
            btnregister.Enabled = false;
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Please wait...");
            progress.SetTitle("Completing your registration");
            progress.SetCancelable(false);

           
          
            if (!islicense || !ispersonal || !isprofilepix || !isworthiness || !isvehicle)
            {
                Toast.MakeText(this, "Please provide all the information required for registration", ToastLength.Short).Show();
                btnregister.Enabled = true;
                return;
            }
            progress.Show();

            edit.PutString("login", "true");
            edit.Apply();
           phone_s = pref.GetString("phone", "");
            email_s = pref.GetString("email", "");
            if (!string.IsNullOrEmpty(phone_s))
            {
                DatabaseReference driverstatus = database.GetReference("drivers/" + phone_s + "/account_status");
                driverstatus.SetValue("pending");
            }
            Intent intent = new Intent(this, typeof(RegCompletedActivity));
            string response = "";
            await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(email_s))
                {
                    response = webHelpers.SendEmail(email_s);
                }
                else
                {
                    response = "no email";
                }
            });
            progress.Dismiss();

            if (response.Contains("suc"))
            {
                StartActivity(intent);
                FinishAffinity();
            }
            else
            {
                Toast.MakeText(this, "Registration was not completed, please try again", ToastLength.Short).Show();
            }

            btnregister.Enabled = true;
        }
        public void SetUpFirebase()
        {

            try
            {
                var options = new FirebaseOptions.Builder()
                 .SetApplicationId("kinitaxi-1007b")
                 .SetApiKey("AIzaSyBGEbUGOVZaP5DLh3UK-cM1kF-bw7e-YMI")
                 .SetDatabaseUrl("https://kinitaxi-1007b.firebaseio.com")
                 .SetStorageBucket("kinitaxi-1007b.appspot.com")
                 .Build();
                var app = Firebase.FirebaseApp.InitializeApp(this, options);
                database = FirebaseDatabase.GetInstance(app);
            }
            catch
            {
                try
                {
                    var app = Firebase.FirebaseApp.InitializeApp(this);
                    database = FirebaseDatabase.GetInstance(app);
                }
                catch
                {
                    database = FirebaseDatabase.GetInstance(FirebaseApp.Instance);
                }

            }
        }


        public void DocumentStatus()
        {
            string make = pref.GetString("make", "");
            if (!string.IsNullOrEmpty(make))
            {
                isvehicle = true;
                imgcheckvehicle.Visibility = ViewStates.Visible;
            }

            string firstname = pref.GetString("firstname", "");
            if (!string.IsNullOrEmpty(firstname))
            {
                ispersonal = true;
                imgcheckpersonaldetails.Visibility = ViewStates.Visible;
            }

            string license_status = pref.GetString("license_status", "");
            string profilepix_status = pref.GetString("profilepix_status", "");
            string worthiness_status = pref.GetString("worthiness_status", "");

            if (!string.IsNullOrEmpty(license_status))
            {
                if(license_status == "approved" || license_status == "pending")
                {
                    islicense = true;
                    imgchecklicense.Visibility = ViewStates.Visible;
                }
                else
                {
                    islicense = false;
                    imgchecklicense.SetImageResource(Resource.Mipmap.ic_report_problem_black_48dp);
                    imgchecklicense.SetColorFilter(Color.Rgb(251, 24, 24));
                    imgchecklicense.Visibility = ViewStates.Visible;

                }
            }

            if (!string.IsNullOrEmpty(profilepix_status))
            {
                if (profilepix_status == "approved" || profilepix_status == "pending")
                {
                    isprofilepix = true;
                    imgcheckpix.Visibility = ViewStates.Visible;
                }
                else
                {
                    isprofilepix = false;
                    imgcheckpix.SetImageResource(Resource.Mipmap.ic_report_problem_black_48dp);
                    imgcheckpix.SetColorFilter(Color.Rgb(251, 24, 24));
                    imgcheckpix.Visibility = ViewStates.Visible;

                }
            }

            if (!string.IsNullOrEmpty(worthiness_status))
            {
                if (worthiness_status == "approved" || worthiness_status == "pending")
                {
                    isworthiness = true;
                    imgcheckworthiness.Visibility = ViewStates.Visible;
                }
                else
                {
                    isworthiness = false;
                    imgcheckworthiness.SetImageResource(Resource.Mipmap.ic_report_problem_black_48dp);
                    imgcheckworthiness.SetColorFilter(Color.Rgb(251, 24, 24));
                    imgcheckworthiness.Visibility = ViewStates.Visible;

                }
            }


        }
        private void Btnvehicle_Click(object sender, EventArgs e)
        {
            vehicledialogue = new VehicleDetailsDialogue();
            var trans = SupportFragmentManager.BeginTransaction();
            vehicledialogue.Show(trans, "vehicle");
            vehicledialogue.OnSaveVehicleDetails += Vehicledialogue_OnSaveVehicleDetails;
        }

        private void Vehicledialogue_OnSaveVehicleDetails(object sender, OnSaveVehicleDetailsEventArgs e)
        {
            DatabaseReference vehicleRef = database.GetReference("drivers/" + phoneID + "/vehicle_details");

            HashMap map = new HashMap();
            map.Put("make", e.make);
            map.Put("model", e.model);
            map.Put("year", e.year);
            map.Put("color", e.color);
            map.Put("plate_number", e.platenumber);
            vehicleRef.SetValue(map);

            Toast.MakeText(this, "Vehicle details successfully saved", ToastLength.Short).Show();
            edit.PutString("make", e.make);
            edit.PutString("model", e.model);
            edit.PutString("year", e.year);
            edit.PutString("color", e.color);
            edit.PutString("platenumber", e.platenumber);
            edit.Apply();
            isvehicle = true;
            imgcheckvehicle.Visibility = ViewStates.Visible;

            DatabaseReference driverstatus = database.GetReference("drivers/" + phoneID + "/account_status");
            driverstatus.SetValue("pending");
            driverstatus.Dispose();
        }

        private void Btnpersonaldetails_Click(object sender, EventArgs e)
        {
            profiledialogue = new PersonalProfileRegistrationDialogue(phoneID);
            var trans = SupportFragmentManager.BeginTransaction();
            profiledialogue.Show(trans, "profile");
            profiledialogue.SavePersonalDetails += Profiledialogue_SavePersonalDetails;
        }

        private void Profiledialogue_SavePersonalDetails(object sender, OnSaveRegProfileEventArgs e)
        {
            DatabaseReference personalRef = database.GetReference("drivers/" + phoneID + "/personal_details");

            HashMap map = new HashMap();
            map.Put("created_at", helpers.GetTimeStampNow().ToString());
            map.Put("first_name", e.mfirstname);
            map.Put("last_name", e.mlastname);
            map.Put("phone", phoneID);
            map.Put("email", e.memail);
            map.Put("city", e.mcity);
            map.Put("invite", e.minvitecode);
            personalRef.SetValue(map);

            firstname_s = e.mfirstname;
            lastname_s = e.mlastname;
            email_s = e.memail;
            
            Toast.MakeText(this, "Personal profile successfully saved", ToastLength.Short).Show();
            edit.PutString("firstname", e.mfirstname);
            edit.PutString("lastname", e.mlastname);
            edit.PutString("phone", phoneID);
            edit.PutString("email", e.memail);
            edit.PutString("city", e.mcity);
            edit.PutString("invitecode", e.minvitecode);
            edit.Apply();
            ispersonal = true;
            imgcheckpersonaldetails.Visibility = ViewStates.Visible;

            DatabaseReference driverstatus = database.GetReference("drivers/" + phoneID + "/account_status");
            driverstatus.SetValue("pending");
            driverstatus.Dispose();
        }

        private void Btnworthiness_Click(object sender, EventArgs e)
        {
            documentdialogue = new DocumentUploadDialogue("worthiness");
            var trans = SupportFragmentManager.BeginTransaction();
            documentdialogue.Show(trans, "document");
            documentdialogue.TakeAPhoto += async (o, f) =>
            {
                //whichimage = "worthiness";
                //if (IsThereAnAppToTakePictures())
                //{
                //    CreateDirectoryForPictures();
                //    TakeAPicture();
                //}
                string imagename = helpers.GenerateRandomString(10);
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Toast.MakeText(this, "Camera is not available", ToastLength.Short).Show();
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    CompressionQuality = 50,
                    Directory = "Sample",
                    Name =  imagename + "test.jpg"
                });

                if(file != null)
                {
                    if (!string.IsNullOrEmpty(file.Path))
                    {
                        string imagestring = helpers.ConvertImagePathToBase64(file.Path);
                        saveworthiness(imagestring);
                    }
                }
               
            };

            documentdialogue.UploadAPhoto += async (o, r) =>
            {
                //whichimage = "worthiness";
                //Intent = new Intent();
                //Intent.SetType("image/*");
                //Intent.SetAction(Intent.ActionGetContent);
                //StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);

                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    Toast.MakeText(this, "Upload not supported", ToastLength.Short).Show();
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {

                });

                if (file != null)
                {
                    if (!string.IsNullOrEmpty(file.Path))
                    {
                        string imagestring = helpers.ConvertImagePathToBase64(file.Path);
                        saveworthiness(imagestring);
                    }
                }

            };
        }

        private void Btnprofilepix_Click(object sender, EventArgs e)
        {
            documentdialogue = new DocumentUploadDialogue("profile");
            var trans = SupportFragmentManager.BeginTransaction();
            documentdialogue.Show(trans, "pix");
            documentdialogue.TakeAPhoto += async (o, f) =>
            {
                //whichimage= "pix";
                //if (IsThereAnAppToTakePictures())
                //{
                //    CreateDirectoryForPictures();
                //    TakeAPicture();
                //}

                string imagename = helpers.GenerateRandomString(10);
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Toast.MakeText(this, "Camera is not available", ToastLength.Short).Show();
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    CompressionQuality = 50,
                    Directory = "Sample",
                    Name = imagename + "test.jpg"
                });

                if (file != null)
                {
                    if (!string.IsNullOrEmpty(file.Path))
                    {
                        string imagestring = helpers.ConvertImagePathToBase64(file.Path);
                        saveprofilepix(imagestring);
                    }
                }
            };

            documentdialogue.UploadAPhoto += async (o, r) =>
            {
                //whichimage = "pix";
                //Intent = new Intent();
                //Intent.SetType("image/*");
                //Intent.SetAction(Intent.ActionGetContent);
                //StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    Toast.MakeText(this, "Upload not supported", ToastLength.Short).Show();
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                  
                });

                if (file != null)
                {
                    if (!string.IsNullOrEmpty(file.Path))
                    {
                        string imagestring = helpers.ConvertImagePathToBase64(file.Path);
                        saveprofilepix(imagestring);
                    }
                }

            };

            
        }

        private void Btnlicense_Click(object sender, EventArgs e)
        {
            documentdialogue = new DocumentUploadDialogue("license");
            var trans = SupportFragmentManager.BeginTransaction();
            documentdialogue.Show(trans, "license");
            documentdialogue.TakeAPhoto += async (o, f) =>
            {
                //whichimage = "license";
                //if (IsThereAnAppToTakePictures())
                //{
                //    CreateDirectoryForPictures();
                //    TakeAPicture();
                //}

                string imagename = helpers.GenerateRandomString(10);
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    Toast.MakeText(this, "Camera is not available", ToastLength.Short).Show();
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    CompressionQuality = 50,
                    Directory = "Sample",
                    Name = imagename + "test.jpg"
                });

                if (file != null)
                {
                    if (!string.IsNullOrEmpty(file.Path))
                    {
                        string imagestring = helpers.ConvertImagePathToBase64(file.Path);
                        savelicense(imagestring);
                    }
                }
            };
            documentdialogue.UploadAPhoto += async (o, r) =>
            {
                //whichimage = "license";
                //Intent = new Intent();
                //Intent.SetType("image/*");
                //Intent.SetAction(Intent.ActionGetContent);
                //StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);

                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    Toast.MakeText(this, "Upload not supported", ToastLength.Short).Show();
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {

                });

                
                if (file != null)
                {
                    if (!string.IsNullOrEmpty(file.Path))
                    {
                        string imagestring = helpers.ConvertImagePathToBase64(file.Path);
                        savelicense(imagestring);
                    }
                }

            };
        }

        private void TakeAPicture()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
        Android.OS.Environment.GetExternalStoragePublicDirectory(
            Android.OS.Environment.DirectoryDocuments), "AcxiApp");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
              
            }
          
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;

        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            string imagestring = "nothing";
            HelperFunctions helpers = new HelperFunctions();
            //Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            //Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
            //mediaScanIntent.SetData(contentUri);
            //SendBroadcast(mediaScanIntent);

            if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
            {
                
                Android.Net.Uri uri = data.Data;
                string imgpath = getRealPathFromURI(uri);
                if (string.IsNullOrEmpty(imgpath))
                {
                    Toast.MakeText(this, "Something went wrong, try snapping the image instead", ToastLength.Short).Show();
                    return;
                }

                imagestring = helpers.ConvertImagePathToBase64(imgpath);

                if (string.IsNullOrEmpty(imagestring))
                {
                    return;
                }

                if (whichimage == "pix")
                {
                    saveprofilepix(imagestring);
                    imagestring = "";
                }
                else if(whichimage == "license")
                {
                    savelicense(imagestring);
                    imagestring = "";
                }
                else if(whichimage == "worthiness")
                {
                    saveworthiness(imagestring);
                    imagestring = "";
                }
            }
            else
            {
                if (whichimage == "pix")
                {
                    //int height = 200;
                    //int width = 200;
                    //App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                    ////string b = App._file.Path;
                    //if (App.bitmap != null)
                    //{
                    //    imagestring = helpers.ConvertBitmapToBase64(App.bitmap);
                    //    App.bitmap = null;
                    //}

                    if(!string.IsNullOrEmpty(App._file.Path))
                    {
                       
                        imagestring = helpers.ConvertImagePathToBase64(App._file.Path);
                        if (string.IsNullOrEmpty(imagestring))
                        {
                            return;
                        }
                        saveprofilepix(imagestring);
                        imagestring = "";
                    }
                    else
                    {
                        return;
                    }
                  
                }
                else
                {
                    if(App._file != null)
                    {
                        if (!string.IsNullOrEmpty(App._file.Path))
                        {
                            imagestring = helpers.ConvertImagePathToBase64(App._file.Path);
                            if (string.IsNullOrEmpty(imagestring))
                            {
                                return;
                            }
                            if (whichimage == "worthiness")
                            {
                                saveworthiness(imagestring);
                                imagestring = "";
                            }
                            else
                            {
                                savelicense(imagestring);
                                imagestring = "";
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                   
                }
            }

           

        }

        private string getRealPathFromURI(Android.Net.Uri contentUri)
        {
            string filename = "";
            string thepath = "";
            Android.Net.Uri filePathUri;
           
                ICursor cursor = this.ContentResolver.Query(contentUri, null, null, null, null);
                if (cursor.MoveToFirst())
                {
                int column_index = cursor.GetColumnIndex(MediaStore.Images.Media.InterfaceConsts.Data);//Instead of "MediaStore.Images.Media.DATA" can be used "_data"
                //int column_index = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data);//Instead of "MediaStore.Images.Media.DATA" can be used "_data"
                    filePathUri = Android.Net.Uri.Parse(cursor.GetString(column_index));
                    filename = filePathUri.LastPathSegment;
                    thepath = filePathUri.Path;
                }
          
          
            return thepath;
        }

        public  async void saveprofilepix( string imgbase)
        {
            
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetTitle("Uploading file");
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();
            string response = "";
            await Task.Run(() =>
            {
               response = webHelpers.UploadFile(imgbase, "profile_pic", phoneID);
               
            });

            if (response.Contains("suc"))
            {
                edit.PutString("profilepix", imgbase);
                edit.PutString("profilepix_status", "pending");
                edit.Apply();
                isprofilepix = true;
                imgcheckpix.SetImageResource(Resource.Mipmap.ic_check_black_48dp);
                imgcheckpix.SetColorFilter(Color.Rgb(70, 188, 82));
                imgcheckpix.Visibility = ViewStates.Visible;

                DatabaseReference driverstatus = database.GetReference("drivers/" + phoneID + "/account_status");
                driverstatus.SetValue("pending");
                driverstatus.Dispose();
            }
            else
            {
                Toast.MakeText(this, "We experience a slight glitch, please try again", ToastLength.Short).Show();
            }
           
            progress.Dismiss();
        }

        public async void saveworthiness(string imgbase)
        {
           
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetTitle("Uploading file");
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();

            string response = "";
            await Task.Run(() =>
            {
                response = webHelpers.UploadFile(imgbase, "worthiness", phoneID);
            });

            if (response.Contains("suc"))
            {
                edit.PutString("worthiness", imgbase);
                edit.PutString("worthiness_status", "pending");
                edit.Apply();
                isworthiness = true;
                imgcheckworthiness.SetImageResource(Resource.Mipmap.ic_check_black_48dp);
                imgcheckworthiness.SetColorFilter(Color.Rgb(70, 188, 82));
                imgcheckworthiness.Visibility = ViewStates.Visible;

                if (!string.IsNullOrEmpty(phone_s))
                {
                    DatabaseReference driverstatus = database.GetReference("drivers/" + phoneID + "/account_status");
                    driverstatus.SetValue("pending");
                    driverstatus.Dispose();
                }
            }
            else
            {
                Toast.MakeText(this, "We experience a slight glitch, please try again", ToastLength.Short).Show();
            }
            progress.Dismiss();

        }

        public async void savelicense(string imgbase)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetTitle("Uploading file");
            progress.SetMessage("Please wait...");
            progress.SetCancelable(false);
            progress.Show();
            string response = "";
            await Task.Run(() =>
            {
             response = webHelpers.UploadFile(imgbase, "license", phoneID);
            });
            if (response.Contains("suc"))
            {
                edit.PutString("license", imgbase);
                edit.PutString("license_status", "pending");
                edit.Apply();
                islicense = true;
                imgchecklicense.SetImageResource(Resource.Mipmap.ic_check_black_48dp);
                imgchecklicense.SetColorFilter(Color.Rgb(70, 188, 82));
                imgchecklicense.Visibility = ViewStates.Visible;

                DatabaseReference driverstatus = database.GetReference("drivers/" + phoneID + "/account_status");
                driverstatus.SetValue("pending");
                driverstatus.Dispose();
            }
            else
            {
                Toast.MakeText(this, "We experience a slight glitch, please try again", ToastLength.Short).Show();
            }
            progress.Dismiss();
        }
        public override void OnBackPressed()
        {
          

            if(!islicense || !ispersonal || !isworthiness ||!ispersonal || isvehicle)
            {
              

                AppAlertDialogue appAlert = new AppAlertDialogue("Registration is incomplete, Are you sure you want to exit");
                appAlert.Cancelable = true;
                var trans1 = SupportFragmentManager.BeginTransaction();
                appAlert.Show(trans1, "appalert");
                appAlert.AlertCancel += (o, e) =>
                {
                    appAlert.Dismiss();
                };

                appAlert.AlertOk += (w, f) =>
                {
                    base.OnBackPressed();
                    //this.Finish();
                };
            }
        }

        protected override void OnPause()
        {

            if (!islicense || !ispersonal || !isworthiness || !ispersonal || isvehicle)
            {


                AppAlertDialogue appAlert = new AppAlertDialogue("Registration is incomplete, Are you sure you want to minimize");
                appAlert.Cancelable = true;
                var trans1 = SupportFragmentManager.BeginTransaction();
                appAlert.Show(trans1, "appalert");
                appAlert.AlertCancel += (o, e) =>
                {
                    appAlert.Dismiss();
                };

                appAlert.AlertOk += (w, f) =>
                {
                    base.OnPause();
                };
            }
           
        }



        protected override void AttachBaseContext(Android.Content.Context @base)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(@base));
        }

        void IValueEventListener.OnCancelled(DatabaseError error)
        {
           // throw new NotImplementedException();
        }

        void IValueEventListener.OnDataChange(DataSnapshot snapshot)
        {
           
        }

        public static class App
        {
            public static File _file;
            public static File _dir;
            public static Bitmap bitmap;
        }

    }

    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }

}