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
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Acxi.DataModels;

namespace Acxi.Helpers
{
    public static class Extensions
    {
        public static byte[] ReadAllBytes(this BinaryReader reader)
        {
            const int bufferSize = 4096;
            using (var ms = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ms.Write(buffer, 0, count);
                }
                return ms.ToArray();
            }
        }
    }

    class WebRequestHelpers
    {
        string api_endpoint = "http://api.kinitaxi.com/index.php/api/";
        public string GetDirectionJson(string str)
        {
            string url = api_endpoint + "rides/direction";
            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("url", str);
            string jsonstr = JsonConvert.SerializeObject(tosend);
            string strr = "";

            try
            {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonstr);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();
                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {

            }
            return strr;
        }

        public async Task<string> GetDirectionJsonAsync(string url)
        {
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        public string otp(string phone)
        {
            string url = api_endpoint + "messaging/ragnarok";

            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("recipient", phone);

            string jsonstr = JsonConvert.SerializeObject(tosend);
            string strr = "";
            try
           {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonstr);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();
                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {

            }

            return strr;
        }

        public string CommitRegistration(string firstname, string lastname, string photourl, string email, string phone, string token, string created_at, string overide)
        {
            string url = api_endpoint + "users/register";

            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("first_name", firstname);
            tosend.Add("last_name", lastname);
            tosend.Add("created_at", created_at);
            tosend.Add("photo_url", photourl);
            if (overide == "yes")
            {
                tosend.Add("override", "true");
            }
            if (!string.IsNullOrEmpty(email))
            {
                tosend.Add("email", email);
            }
            tosend.Add("phone", phone);
            // tosend.Add("token", token);

            string jsonstr = JsonConvert.SerializeObject(tosend);
            string strr = "";
            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = System.Net.WebRequest.Create(url);
            request.Timeout = int.MaxValue;
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonstr);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Stream datastream = request.GetRequestStream();
            datastream.Write(byteArray, 0, byteArray.Length);
            datastream.Close();

            System.Net.WebResponse response = default(System.Net.WebResponse);
            response = request.GetResponse();
            Stream received = response.GetResponseStream();
            StreamReader mystream = new StreamReader(received);
            strr = mystream.ReadToEnd();

            return strr;
        }

        public string AddCard(string card_no, string cvv, string expiry_month, string expiry_year, string pin)
        {
            string url = api_endpoint + "billing/add_card";

            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("card_no", card_no);
            tosend.Add("cvv", cvv);
            tosend.Add("expiry_month", expiry_month);
            tosend.Add("expiry_year", expiry_year);
            tosend.Add("pin", pin);


            string jsonstr = JsonConvert.SerializeObject(tosend);
            string strr = "";

            try
            {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonstr);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {

            }

            return strr;
        }

        public string CompleteAddCard(string reference, string type, string value)
        {
            string url = api_endpoint + "billing/complete_add_card";

            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("ref", reference);
            if (type == "send_otp")
            {
                tosend.Add("value", value);
                tosend.Add("type", "otp");
            }
            else if (type == "send_phone")
            {
                tosend.Add("type", "phone");
                tosend.Add("value", value);
            }
            else if (type == "send_birthday")
            {
                tosend.Add("type", "birthday");
                tosend.Add("value", value);
            }
            else if (type == "send_pin")
            {
                tosend.Add("type", "pin");
                tosend.Add("value", value);
            }

            string jsonstr = JsonConvert.SerializeObject(tosend);
            string strr = "";

            try
            {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonstr);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {

            }


            return strr;
        }

        public string BillCard(string email, string pin, string amount, string ride_id, string auth_code)
        {
            string url = api_endpoint + "billing/charge_card";
            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("email", email);
            tosend.Add("pin", pin);
            tosend.Add("amount", amount);
            tosend.Add("ride_id", ride_id);
            tosend.Add("auth_code", auth_code);

            string jsonstr = JsonConvert.SerializeObject(tosend);
            string strr = "";
            try
            {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonstr);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {

            }


            return strr;

        }


        public string NotifyDriver(string mdata, string token, string skey)
        {
            string url = "https://fcm.googleapis.com/fcm/send";

            Firemessage tosend = new Firemessage();
            Firemessage.Data data = new Firemessage.Data();
            data.ride = mdata;
            data.type = "ride";

            tosend.to = token;
            tosend.data = data;

            string jsonstr = JsonConvert.SerializeObject(tosend);
            string strr = "";
            string serverKey = skey;
            try
            {

                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "POST";
                request.PreAuthenticate = true;
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonstr);
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", serverKey);
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {
                strr = "error";
            }

            return strr;
        }



        public string ResendReceipt(string email, string ride_id)
        {
            string url = api_endpoint + "rides/send_receipt";
            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("email", email);
            tosend.Add("ride_id", ride_id);
            string jsonstr = JsonConvert.SerializeObject(tosend);
            string strr = "";

            try
            {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(jsonstr);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {

            }
            return strr;
        }

        public string GetRideHistory(string phone, string page)
        {
            string url = api_endpoint + "users/ride_history?user_id=" + phone + "&page_no=" + page;
            string jstr = "";

            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = System.Net.WebRequest.Create(url);
            request.Timeout = int.MaxValue;
            request.Method = "GET";

            System.Net.WebResponse response = default(System.Net.WebResponse);
            response = request.GetResponse();
            Stream received = response.GetResponseStream();
            StreamReader mystream = new StreamReader(received);
            jstr = mystream.ReadToEnd();

            return jstr;
        }


        public string GetRideInfo(string ride_id)
        {
            string url = api_endpoint + "rides/ride?ride_id=" + ride_id;
            string jstr = "";

            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = System.Net.WebRequest.Create(url);
            request.Timeout = int.MaxValue;
            request.Method = "GET";

            System.Net.WebResponse response = default(System.Net.WebResponse);
            response = request.GetResponse();
            Stream received = response.GetResponseStream();
            StreamReader mystream = new StreamReader(received);
            jstr = mystream.ReadToEnd();

            return jstr;
        }

        public async Task<string> userexistence(string phone)
        {
            string url = api_endpoint + "users/check_user?phone=" + phone;
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }


        public async Task<string> userprofilejson(string phone)
        {
            string url = api_endpoint + "users/users?user=" + phone;
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        public string userprofilejson_main(string phone)
        {
            string url = api_endpoint + "users/users?user=" + phone;
            string jstr = "";

            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = System.Net.WebRequest.Create(url);
            request.Timeout = int.MaxValue;
            request.Method = "GET";

            System.Net.WebResponse response = default(System.Net.WebResponse);
            response = request.GetResponse();
            Stream received = response.GetResponseStream();
            StreamReader mystream = new StreamReader(received);
            jstr = mystream.ReadToEnd();
            return jstr;

        }

        public async Task<string> downloadimage(string url)
        {
            HttpClient client = new HttpClient();
            var bytes = await client.GetByteArrayAsync(url);
            return "image/jpeg;base64," + Convert.ToBase64String(bytes);
        }

        public string downloadImage1(string url)
        {
            string imgstr = "";
            try
            {
                if (!url.Contains("http"))
                {
                    url = "https://" + url;
                }
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "GET";
                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                var reader = new BinaryReader(response.GetResponseStream());
                byte[] arbyte = reader.ReadAllBytes();
                imgstr = "image/jpeg;base64," + Convert.ToBase64String(arbyte);

            }
            catch
            {

            }

            return imgstr;
        }

        public async Task<string> AssignDriver(string ride_id)
        {
            string url = api_endpoint + "rides/assign_driver?ride_id=" + ride_id;
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        public async Task<string> FindDriver(string ride_id)
        {
            string url = api_endpoint + "rides/find_driver?ride_id=" + ride_id;
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        public string AssignDriver1(string ride_id)
        {
            string url = api_endpoint + "rides/assign_driver?ride_id=" + ride_id;
            string strr = "";
            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = System.Net.WebRequest.Create(url);
            request.Timeout = int.MaxValue;
            request.Method = "GET";

            System.Net.WebResponse response = default(System.Net.WebResponse);
            response = request.GetResponse();
            Stream received = response.GetResponseStream();
            StreamReader mystream = new StreamReader(received);
            strr = mystream.ReadToEnd();
            return strr;
        }

        public string AssignDriver2(string json)
        {
            string url = api_endpoint + "rides/find_driver";
            string strr = "";

            try
            {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(json);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {

            }

            return strr;
        }
    }
}