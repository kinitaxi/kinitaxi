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
                strr = "failed";
            }
           

            return strr;
        }

        public string  CommitRegistration(string firstname, string lastname, string password, string photourl, string email, string phone, string token)
        {
            string url = api_endpoint + "users/register";

            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("first_name", firstname);
            tosend.Add("last_name", lastname);
            tosend.Add("password", phone);
            tosend.Add("photo_url", photourl);
            tosend.Add("email", email);
            tosend.Add("phone", phone);
            tosend.Add("token", token);

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

        public string UploadFile(string baseString, string type, string phone)
        {
            string url = api_endpoint + "drivers/upload";
            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("phone", phone);
            tosend.Add("type", type );
            tosend.Add("file_string", baseString);

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
                strr = "error";
            }
           
            return strr;
        }


        public string AddAccount(string name, string accountnumber, string bankcode, string bank, string phone)
        {
            string url = api_endpoint + "drivers/add_account";
            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("name", name);
            tosend.Add("account_number", accountnumber);
            tosend.Add("bank_code", bankcode);
            tosend.Add("bank", bank);
            tosend.Add("phone", phone);

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
                strr = "error";
            }

            return strr;
        }



        public string CheckAccountStatus(string phone)
        {
            string url = api_endpoint + "drivers/check_status?phone=" + phone;
            //var handler = new HttpClientHandler();
            //HttpClient client = new HttpClient(handler);
            //string result = await client.GetStringAsync(url);
            //return result;

            string strr = "";
            try
            {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Method = "GET";
                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {
                strr = "timeout";
            }

            return strr;
        }


        public string GetTripHistory(string phone, string page)
        {
            string url = api_endpoint + "drivers/trip_history?driver_id=" + phone + "&page_no=" + page;
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

        public string SendEmail( string email)
        {
            string url = api_endpoint + "drivers/send_email";

            Dictionary<string, string> tosend = new Dictionary<string, string>();
            tosend.Add("email", email);
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


        public string userexistence(string phone)
        {
            string url = api_endpoint + "drivers/check_driver?phone=" + phone;
            //var handler = new HttpClientHandler();
            //HttpClient client = new HttpClient(handler);
            //string result = await client.GetStringAsync(url);
            //return result;

            string strr = "";
            try
            {
                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Method = "GET";
                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                Stream received = response.GetResponseStream();
                StreamReader mystream = new StreamReader(received);
                strr = mystream.ReadToEnd();
            }
            catch
            {
                strr = "timeout";
            }
          
            return strr;
        }

        public async Task<string> userexist( string phone)
        {
            string url = api_endpoint + "drivers/check_driver?phone=" + phone;
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }

        public string userprofilejson(string phone)
        {
            string jsonstr = "";
            return jsonstr;
        }

        public string downloadImage(string url)
        {
            string returnstring = "";
            try
            {
                if (!url.Contains("http"))
                {
                    url = "http://" + url;
                }

                System.Net.WebRequest request = default(System.Net.WebRequest);
                request = System.Net.WebRequest.Create(url);
                request.Timeout = int.MaxValue;
                request.Method = "GET";
                System.Net.WebResponse response = default(System.Net.WebResponse);
                response = request.GetResponse();
                var reader = new BinaryReader(response.GetResponseStream());
                byte[] arbyte = reader.ReadAllBytes();
                returnstring = Convert.ToBase64String(arbyte);
            }
            catch
            {

            }
            return returnstring;
           
        }

        public async Task<string> AssignDriver (string ride_id)
        {
            string url = api_endpoint + "rides/assign_driver?" + ride_id;
            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient(handler);
            string result = await client.GetStringAsync(url);
            return result;
        }
    }
}