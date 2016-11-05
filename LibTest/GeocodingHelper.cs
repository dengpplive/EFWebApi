using System;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoogleGeocodingAPI
{

    public class GeocodingHelper
    {



        //double lat = Convert.ToDouble("25.0392");//纬度
        //double lng = Convert.ToDouble("121.525");//经度
        //string result = latLngToChineseAddress(lat, lng);
        //Response.Write(result);



        /// <summary> 
        /// 经纬度取得行政区 
        /// </summary> 
        /// <returns></returns> 
        public static string latLngToChineseDistrict(params double[] latLng)
        {
            string result = string.Empty;//要回传的字符串 
            string url =
                 "http://maps.googleapis.com/maps/api/geocode/json?latlng=" + string.Join(",", latLng) + "&sensor=true";
            string json = String.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            //指定语言，否则Google预设回传英文 
            request.Headers.Add("Accept-Language", "zh-tw");
            using (var response = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    json = sr.ReadToEnd();
                }
            }
            RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(json);

            result = rootObj.results[0].address_components
                .Where(c => c.types[0] == "locality" && c.types[1] == "political").FirstOrDefault().long_name;

            return result;

        }

        /// <summary> 
        /// 经纬度转中文地址：https://developers.google.com/maps/documentation/geocoding/?hl=zh-TW#ReverseGeocoding 
        /// </summary>  www.it165.net
        /// <param name="latLng"></param> 
        public static string latLngToChineseAddress(params double[] latLng)
        {
            string url =
                 "http://maps.googleapis.com/maps/api/geocode/json?latlng=" + string.Join(",", latLng) + "&sensor=true";
            string json = String.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            //指定语言，否则Google预设回传英文 
            request.Headers.Add("Accept-Language", "zh-tw");
            using (var response = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    json = sr.ReadToEnd();
                }
            }
            RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(json);

            return rootObj.results[0].formatted_address;

        }

        public class AddressComponent
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public List<string> types { get; set; }
        }
        public class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
        public class Northeast
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
        public class Southwest
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
        public class Viewport
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }
        public class Geometry
        {
            public Location location { get; set; }
            public string location_type { get; set; }
            public Viewport viewport { get; set; }
        }
        public class Result
        {
            public List<AddressComponent> address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public bool partial_match { get; set; }
            public List<string> types { get; set; }
        }
        public class RootObject
        {
            public List<Result> results { get; set; }
            public string status { get; set; }
        }
    }






}

