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
using System.Globalization;
using System.IO;
using Refractored.Controls;
using Android.Util;
using Android.Graphics;
using Java.Nio;
using Android.Gms.Maps.Model;

namespace Acxi.Helpers
{
    public class HelperFunctions
    {
        public string generatePinCode()
        {
            Random rnd = new Random();
            char[] allowchars = "0123456789".ToCharArray();
            string sResult = "";
            for (int i = 0; i <= 4; i++)
            {
                sResult += allowchars[rnd.Next(0, allowchars.Length)];
            }
            return sResult;
        }

        public string FormatNumber(double distance)
        {
            string unit = "m";
            if (distance < 1)
            {
                distance *= 1000;
                unit = "mm";
            }
            else if (distance > 1000)
            {
                distance /= 1000;
                unit = "km";
            }

            return distance.ToString() + " " + unit;
        }

        public double EstimateFares(double distance)
        {
            string fares = "";
            int km = (int) distance / 1000;
            double amount = (km * 60) + 360;
           // fares = amount.ToString("₦#,##0.00", CultureInfo.InvariantCulture);

            return amount;
        }
        public string Estimatefares2( double distance, int stops, int minutes)
        {
            string fares = "";
            int km = (int)distance / 1000;
            double stopcharge = 250;
            double basefare = 360;
            double timecharge = 6;

            double stopfares = stopcharge * stops;
            double amount = ((km * 60) + basefare) + stopfares;

            fares = amount.ToString("₦#,##0.00", CultureInfo.InvariantCulture);

            return fares;
        }

        public string CurrencyConvert(double amount)
        {
            string fares = "";
            fares = amount.ToString("₦#,##0.00", CultureInfo.InvariantCulture);
            return fares;
        }

        public double GetTimeStampNow()
        {
            var epoc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            var delta = (DateTime.Now) - epoc;
            var ticks = (double)delta.TotalSeconds;
            return ticks;
        }

        public string ConvertToDate(double timestamp)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            date = date.AddSeconds(timestamp);
            string theDate = date.ToString("MMMM dd, yyyy. hh:MM tt");
            return theDate;
        }

        public float bearingBetweenLocations(LatLng latLng1, LatLng latLng2)
        {

            double PI = 3.14159;
            double lat1 = latLng1.Latitude * PI / 180;
            double long1 = latLng1.Longitude * PI / 180;
            double lat2 = latLng2.Latitude * PI / 180;
            double long2 = latLng2.Longitude * PI / 180;

            double dLon = (long2 - long1);

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1)
                    * Math.Cos(lat2) * Math.Cos(dLon);

            double brng = Math.Atan2(y, x);
            brng = Java.Lang.Math.ToDegrees(brng);
            brng = (brng + 360) % 360;
            return float.Parse(brng.ToString());
        }

        public void SetProfileImage(CircleImageView imgProfile, string base64)
        {
            string imagebase64 = base64;
            if (!string.IsNullOrEmpty(imagebase64))
            {
                string base64String = imagebase64;
                string base64Image = imagebase64.Split(',')[1];

                byte[] decodedString = Base64.Decode(base64Image, Base64.Default);
                Bitmap decodedByte = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);

                imgProfile.SetImageBitmap(decodedByte);
            }

        }

        public void SetProfileImage1(CircleImageView imgProfile, string base64)
        {
            string imagebase64 = base64;
            if (!string.IsNullOrEmpty(imagebase64))
            {
                try
                {
                    string base64Image = imagebase64;
                    byte[] decodedString = Base64.Decode(base64Image, Base64.Default);
                    Bitmap decodedByte = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);
                    imgProfile.SetImageBitmap(decodedByte);
                }
                catch
                {

                }
               
            }

        }

        public void SetProfileImage2(ImageView imgProfile, string base64)
        {
            string imagebase64 = base64;
            if (!string.IsNullOrEmpty(imagebase64))
            {
                string base64Image = imagebase64;
                byte[] decodedString = Base64.Decode(base64Image, Base64.Default);
                Bitmap decodedByte = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);
                imgProfile.SetImageBitmap(decodedByte);
            }

        }

        public double returnDouble(string str)
        {
            return double.Parse(str.Substring(1, str.Length - 1));
        }

        public string ConvertBitmapToBase64(Bitmap image)
        {
            //string basestring = "";
            //using (var stream = new MemoryStream())
            //{
            //    image.Compress(Bitmap.CompressFormat.Jpeg, 0, stream);

            //    var bytes = stream.ToArray();
            //    var str = Convert.ToBase64String(bytes);
            //    basestring = str;
            //}
            //return basestring;

           var array = new byte[image.Width * image.Height * 4];
            ByteBuffer dst = ByteBuffer.Wrap(array);
            image.CopyPixelsToBuffer(dst);
            var str = Convert.ToBase64String(array);
            return str;
        }

        public string ConvertImagePathToBase64(string filepath)
        {
            string base64ImageRepresentation = "";
            try
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(filepath);
                base64ImageRepresentation = Convert.ToBase64String(imageArray);
            }
            catch
            {

            }
           
            return base64ImageRepresentation;
        }
        public string GenerateRandomString(int ilenght)
        {
            Random rnd = new Random();
            char[] allowchars = "ABCDEFGHIJKLOMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            string sResult = "";
            for (int i = 0; i <= ilenght; i++)
            {
                sResult += allowchars[rnd.Next(0, allowchars.Length)];
            }

            return sResult;
        }
    }
}