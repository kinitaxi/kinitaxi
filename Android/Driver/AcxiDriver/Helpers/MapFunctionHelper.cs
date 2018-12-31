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
using Android.Gms.Maps.Model;
using System.Threading.Tasks;
using System.Net.Http;

namespace Acxi.Helpers
{
    class MapFunctionHelper
    {

        public string getMapsApiDirectionsUrl(LatLng location, LatLng destination, string directionKey)
        {
            // Origin of route
            string str_origin = "origin=" + location.Latitude + "," + location.Longitude;

            // Destination of route
            string str_dest = "destination=" + destination.Latitude + "," + destination.Longitude;

            // Sensor enabled
            string sensor = "sensor=false";
            string mode = "mode=driving";

            // Building the parameters to the web service
            string parameters = str_origin + "&" + str_dest + "&" + "&" + mode;

            // Output format
            string output = "json";

            
            string key = "&key=" + directionKey;

            // Building the url to the web service
            string url = "https://maps.googleapis.com/maps/api/directions/" + output + "?" + parameters + key;
            return url;
        }

        public string GetGeocodeUrl(double lat, double lng, string key)
        {
            string url = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + lat + "," + lng + "&key=" + key;
            return url;
        }
    }
}