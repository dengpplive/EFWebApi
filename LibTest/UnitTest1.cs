using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YSL.Common.Utility;

namespace LibTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string lat = "25.0392";//纬度
            string lng = "121.525";//经度
            //http://api.map.baidu.com/geocoder/v2/?ak=C93b5178d7a8ebdb830b9b557abce78b&callback=renderReverse&location=25,100&output=json&pois=1

            // string result = GeocodingHelper.latLngToChineseAddress(lat, lng);
            //var result = ClientHelper.GetBaiduAddress("121.15.174.69");

            //var result1 = ClientHelper.GetBaiDuLL("湖北");
            //ClientHelper.QQGetAddress("121.15.174.69");
            //ClientHelper.GetBaiduAddress(lat, lng);

            //string sss = {"status":0,"result":{"location":{"lng":60.299999966455,"lat":25.40000001759},"formatted_address":"","business":"","addressComponent":{"adcode":"0","city":"","country":"Iran","direction":"","distance":"","district":"","province":"","street":"","street_number":"","country_code":1190000},"poiRegions":[],"sematic_description":"","cityCode":1190222}};

            //var str = ClientHelper.GetIpLookup("121.15.174.69");
            Console.ReadLine();
        }
    }
}
