using NetFLowClass;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using YSL.Common.Extender;
namespace YSL.Common.Utility
{
    /// <summary>
    /// ip与地理位置
    /// </summary>
    public static class IPHelper
    {

        #region 获取浏览器版本号

        /// <summary>  
        /// 获取浏览器版本号  
        /// </summary>  
        /// <returns></returns>  
        public static string GetBrowser()
        {
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
            return bc.Browser + bc.Version;
        }

        #endregion

        #region 获取操作系统版本号

        /// <summary>  
        /// 获取操作系统版本号  
        /// </summary>  
        /// <returns></returns>  
        public static string GetOSVersion()
        {
            //UserAgent   
            var userAgent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];

            var osVersion = "";

            if (userAgent.Contains("NT 6.1"))
            {
                osVersion = "Windows 7";
            }
            else if (userAgent.Contains("NT 6.0"))
            {
                osVersion = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                osVersion = "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                osVersion = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                osVersion = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                osVersion = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                osVersion = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                osVersion = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                osVersion = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                osVersion = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                osVersion = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                osVersion = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                osVersion = "SunOS";
            }
            return osVersion;
        }
        #endregion

        #region 获取客户端IP地址
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP()
        {
            string loginip = "";
            //Request.ServerVariables[""]--获取服务变量集合   
            if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null) //判断发出请求的远程主机的ip地址是否为空  
            {
                //获取发出请求的远程主机的Ip地址  
                loginip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            //判断登记用户是否使用设置代理  
            else if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    //获取代理的服务器Ip地址  
                    loginip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    //获取客户端IP  
                    loginip = HttpContext.Current.Request.UserHostAddress;
                }
            }
            else
            {
                //获取客户端IP  
                loginip = HttpContext.Current.Request.UserHostAddress;
            }
            if (null == loginip || loginip == String.Empty || !Converter.IsIP(loginip))
            {
                loginip = GetIPAddress;
                if (string.IsNullOrEmpty(loginip))
                    return "0.0.0.0";
            }
            return loginip;
        }

        #endregion

        #region 取客户端真实IP

        ///  <summary>    
        ///  取得客户端真实IP。如果有代理则取第一个非内网地址    
        ///  </summary>    
        public static string GetIPAddress
        {
            get
            {
                var result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(result))
                {
                    //可能有代理    
                    if (result.IndexOf(".") == -1)        //没有“.”肯定是非IPv4格式    
                        result = null;
                    else
                    {
                        if (result.IndexOf(",") != -1)
                        {
                            //有“,”，估计多个代理。取第一个不是内网的IP。    
                            result = result.Replace("  ", "").Replace("'", "");
                            string[] temparyip = result.Split(",;".ToCharArray());
                            for (int i = 0; i < temparyip.Length; i++)
                            {
                                if (IsIPAddress(temparyip[i])
                                        && temparyip[i].Substring(0, 3) != "10."
                                        && temparyip[i].Substring(0, 7) != "192.168"
                                        && temparyip[i].Substring(0, 7) != "172.16.")
                                {
                                    return temparyip[i];        //找到不是内网的地址    
                                }
                            }
                        }
                        else if (IsIPAddress(result))  //代理即是IP格式    
                            return result;
                        else
                            result = null;        //代理中的内容  非IP，取IP    
                    }

                }

                string IpAddress = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];

                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];

                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.UserHostAddress;

                return result;
            }
        }



        #endregion

        #region  判断是否是IP格式

        ///  <summary>  
        ///  判断是否是IP地址格式  0.0.0.0  
        ///  </summary>  
        ///  <param  name="str1">待判断的IP地址</param>  
        ///  <returns>true  or  false</returns>  
        public static bool IsIPAddress(string str1)
        {
            if (string.IsNullOrEmpty(str1) || str1.Length < 7 || str1.Length > 15) return false;
            const string regFormat = @"^d{1,3}[.]d{1,3}[.]d{1,3}[.]d{1,3}$";
            var regex = new Regex(regFormat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }

        #endregion

        #region 获取公网IP及省份信息
        /// <summary>
        /// 获取公网IP及省份信息  
        /// </summary>
        /// <param name="isNetIp">是否公网ip地址</param>
        /// <returns></returns>
        public static string GetNetIpAndCity(bool isNetIp = false)
        {
            var tempIp = "";
            var city = "";
            var addr = "";
            try
            {
                //http://1111.ip138.com/ic.asp               
                string url = "http://www.ip138.com/ips138.asp";
                var wr = System.Net.WebRequest.Create(url);
                var s = wr.GetResponse().GetResponseStream();
                var sr = new System.IO.StreamReader(s, Encoding.GetEncoding("gb2312"));
                var allText = sr.ReadToEnd();
                string pattern = @"IP地址是：\[(?<ip>[\.|\d]+)?\]\s*来自：(?<city>\w+)?\s*(?<addr>\w+)?\s*";
                Match mch = Regex.Match(allText, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (mch.Success)
                {
                    tempIp = mch.Groups["ip"].Value;
                    city = mch.Groups["city"].Value;
                    addr = mch.Groups["addr"].Value;
                }
                sr.Close();
                s.Close();
                wr.Abort();
                if (!isNetIp) tempIp = GetIP();
            }
            catch
            {
                if (System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.Length > 1)
                    tempIp = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[1].ToString();
                if (string.IsNullOrEmpty(tempIp)) return GetIP() + " " + city;
            }
            finally
            {

            }
            return tempIp + " " + city + " " + addr;
        }
        #endregion


        #region ip获取地理位置
        public static IpLookup GetIpLookup(string ip)
        {
            string url = string.Format("http://int.dpool.sina.com.cn/iplookup/iplookup.php?format=json&ip={0}", ip);
            var result = WebApiHelper.InvokeApi<IpLookup>(url);
            return result;
        }

        /// <summary>
        /// 根据ip获取经纬度
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static Geography GetLL(string ip)
        {
            string url = string.Format("http://freegeoip.net/json/{0}", ip);
            var result = WebApiHelper.InvokeApi<Geography>(url);
            return result;
        }

        /// <summary>
        /// baidu的api获取地址位置
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static BaiduGeography GetBaiduAddress(string ip)
        {
            string url = string.Format("http://api.map.baidu.com/location/ip?ak=kiXVSjCYTDNtoxFjzfVDDqg2&ip={0}&coor=bd09ll", ip);
            var result = WebApiHelper.InvokeApi<BaiduGeography>(url);
            return result;
        }
        /// <summary>
        /// 根据经纬度获取地理位置 
        /// </summary>
        /// <param name="latitude">伟度</param>
        /// <param name="longitude">经度</param>
        /// <returns></returns>
        public static BaiduLLGeography GetBaiduAddress(string latitude, string longitude)
        {
            string url = string.Format("http://api.map.baidu.com/geocoder/v2/?ak=C93b5178d7a8ebdb830b9b557abce78b&callback=renderReverse&location={0},{1}&output=json&pois=0", latitude, longitude);            
            var result = WebApiHelper.InvokeApi<BaiduLLGeography>(url);
            return result;
        }

        /// <summary>
        /// 输入地址 查询所在的经纬度
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns></returns>
        public static BaiduGeoCoding GetBaiDuLL(string address)
        {
            string url = string.Format("http://api.map.baidu.com/geocoder/v2/?address={0}&output=json&ak=kiXVSjCYTDNtoxFjzfVDDqg2&callback=showLocation", address);
            var result = WebApiHelper.InvokeApi<BaiduGeoCoding>(url);
            return null;
        }
        /// <summary>
        /// QQ 根据ip获取地理位置
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static IPLocation QQGetAddress(string ip)
        {
            QQWryLocator qqwry = new QQWryLocator(Converter.GetMapPath("qqwry.dat"));
            return qqwry.Query(ip);
        }
        #endregion

    }
    public class IpLookup
    {
        public string ret { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string country { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string isp { get; set; }
        public string type { get; set; }
        public string desc { get; set; }
    }
    /// <summary>
    /// ip获取地址位置信息
    /// </summary>
    public class Geography
    {
        /// <summary>
        /// IP
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 国家代码
        /// </summary>
        public string country_code { get; set; }
        /// <summary>
        /// 国家名称
        /// </summary>
        public string country_name { get; set; }
        /// <summary>
        /// 区域代码
        /// </summary>
        public string region_code { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string region_name { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 压缩编码
        /// </summary>
        public string zip_code { get; set; }
        /// <summary>
        /// 时区
        /// </summary>
        public string time_zone { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string latitude { get; set; }
        /// <summary>
        /// 伟度
        /// </summary>
        public string longitude { get; set; }
        public string metro_code { get; set; }
    }

    /// <summary>
    /// ip获取地址位置信息
    /// </summary>
    public class BaiduGeography
    {
        public class Point
        {
            /// <summary>
            /// 经度
            /// </summary>
            public string x { get; set; }
            /// <summary>
            /// 伟度
            /// </summary>
            public string y { get; set; }
        }
        public class Address_detail
        {
            /// <summary>
            /// 城市
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 百度城市代码
            /// </summary>
            public string city_code { get; set; }
            /// <summary>
            /// 区县
            /// </summary>
            public string district { get; set; }
            /// <summary>
            /// 省份
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 街道
            /// </summary>
            public string street { get; set; }
            /// <summary>
            /// 门址 或者牌号
            /// </summary>
            public string street_number { get; set; }
        }
        public class Content
        {
            /// <summary>
            /// 简要地址
            /// </summary>
            public string address { get; set; }
            /// <summary>
            /// 详细地址信息
            /// </summary>
            public Address_detail address_detail { get; set; }
            /// <summary>
            /// 百度经纬度坐标值
            /// </summary>
            public Point point { get; set; }

        }
        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// 详细内容
        /// </summary>
        public Content content { get; set; }
        /// <summary>
        /// 回状态码
        /// </summary>
        public string status { get; set; }

    }
    /// <summary>
    /// 经纬度
    /// </summary>
    public class Location
    {
        /// <summary>
        /// 经度
        /// </summary>
        public string lng { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string lat { get; set; }
    }
    /// <summary>
    /// 地址查询信息
    /// </summary>
    public class BaiduGeoCoding
    {
        public class Result
        {
            /// <summary>
            /// 经纬度
            /// </summary>
            public Location location { get; set; }
            /// <summary>
            /// 位置的附加信息，是否精确查找。1为精确查找，即准确打点；0为不精确，即模糊打点。
            /// </summary>
            public string precise { get; set; }
            /// <summary>
            /// 可信度，描述打点准确度
            /// </summary>
            public string confidence { get; set; }
            /// <summary>
            /// 地址类型
            /// </summary>
            public string level { get; set; }
        }
        /// <summary>
        /// 返回结果状态值， 成功返回0，其他值请查看下方返回码状态表。
        /// 0	正常
        //1	服务器内部错误
        //2	请求参数非法
        //3	权限校验失败
        //4	配额校验失败
        //5	ak不存在或者非法
        //101	服务禁用
        //102	不通过白名单或者安全码不对
        //2xx	无权限
        //3xx	配额错误
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 返回结果信息
        /// </summary>
        public Result result { get; set; }
    }

    /// <summary>
    /// 经纬度定位数据
    /// </summary>
    public class BaiduLLGeography
    {
        public class AddressComponent
        {
            /// <summary>
            /// 行政区划代码
            /// </summary>
            public string adcode { get; set; }
            /// <summary>
            /// 城市名
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 国家
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 和当前坐标点的方向
            /// </summary>
            public string direction { get; set; }
            /// <summary>
            /// 和当前坐标点的距离，当有门牌号的时候返回数据
            /// </summary>
            public string distance { get; set; }
            /// <summary>
            /// 区县名
            /// </summary>
            public string district { get; set; }
            /// <summary>
            /// 省名
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 街道名
            /// </summary>
            public string street { get; set; }
            /// <summary>
            /// 街道门牌号
            /// </summary>
            public string street_number { get; set; }
            /// <summary>
            /// 国家代码
            /// </summary>
            public string country_code { get; set; }
        }
        /// <summary>
        /// 返回结果状态值， 成功返回0，其他值请查看下方返回码状态表。
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 结构化地址信息
        /// </summary>
        public string formatted_address { get; set; }
        /// <summary>
        /// 当前位置结合POI的语义化结果描述
        /// </summary>
        public string sematic_description { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        public string cityCode { get; set; }
        /// <summary>
        /// 商业圈
        /// </summary>
        public string business { get; set; }
        /// <summary>
        /// 经纬度
        /// </summary>
        public Location location { get; set; }
        /// <summary>
        /// 地址组
        /// </summary>
        public AddressComponent addressComponent { get; set; }
        /// <summary>
        /// 周边区域
        /// </summary>
        public JArray poiRegions { get; set; }
    }
}
