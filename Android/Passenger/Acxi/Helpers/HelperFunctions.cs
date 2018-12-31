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
using System.Security.Cryptography;
using Android.Gms.Maps.Model;

namespace Acxi.Helpers
{
    public class HelperFunctions
    {
        public string PreferedCard()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
            string prefered_card = pref.GetString("preferred_card", "");

            if (string.IsNullOrEmpty(prefered_card))
            {
                string card1 = pref.GetString("card1", "");
                string card2 = pref.GetString("card2", "");
                string card3 = pref.GetString("card3", "");
                if (!string.IsNullOrEmpty(card1))
                {
                    prefered_card = card1;
                }
                else if (!string.IsNullOrEmpty(card2))
                {
                    prefered_card = card2;
                }
                else if (!string.IsNullOrEmpty(card3))
                {
                    prefered_card = card3;
                }
            }
            return prefered_card;
        }
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

        public double mEstimateFares(double distance, double duration, double basefare, double distancefare, double timefare)
        {
            double km = (distance / 1000) * distancefare;
            double mins = (duration / 60) * timefare;
            double amount = km + mins + basefare;
            double fare = Math.Floor(amount / 10) * 10;
            return fare;
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

        public string DateDifference(double timestamp)
        {
            string result = "";
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            date = date.AddSeconds(timestamp);
            var days = Math.Round((DateTime.Now - date).TotalDays, 0);
            var weeks = Math.Round((DateTime.Now - date).TotalDays / 7, 0);
            var months = Math.Round((DateTime.Now - date).TotalDays / 30, 0);

            if (days > 0 && days < 7)
            {
                if(days ==1)
                {
                    result = "1 day";
                }
                else
                {
                    result = days.ToString() + " days";
                }
            }
            else if(weeks >0 && days <= 4)
            {
                weeks = Math.Round(weeks, 0);
                if (weeks == 1)
                {
                    result = "1 week";
                }
                else
                {
                    result = weeks.ToString() + " weeks";
                }
            }
            else if(months > 0)
            {
                months = Math.Round(months, 0);
                if(months == 1)
                {
                    result = "1 month";
                }
                else
                {
                    result = weeks.ToString() + " months";
                }
            }

            return result;
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
                try
                {
                    Bitmap decodedByte = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);
                    imgProfile.SetImageBitmap(decodedByte);
                }
                catch
                {
                    return;
                }
               
            }

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

        public string Encrypt( string text)
        {
            string hashstring = "";
            string hash = "acxiapp2018";
            byte[] data = UTF8Encoding.UTF8.GetBytes(text);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDES.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    hashstring = Convert.ToBase64String(results, 0, results.Length);
                }

            }
            return hashstring;
        }

        public string Decrypt(string hashed)
        {
            string returnstring = "";
            string hash = "acxiapp2018";
            byte[] data = Convert.FromBase64String(hashed);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
                using (TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDES.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    returnstring = UTF8Encoding.UTF8.GetString(results);
                }
            }
            return returnstring;
        }


    }
}