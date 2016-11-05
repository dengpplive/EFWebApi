using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace YSL.Framework.AddressLocator
{
    public class IPAddressLocator {
        /// <summary>
        /// 获取本机局域网IP地址
        /// </summary>
        /// <returns></returns>
        public static IList<IPAddress> GetLanIP() {
            var addresses = Dns.GetHostAddresses(Dns.GetHostName());
            return addresses.Where(item => item.AddressFamily == AddressFamily.InterNetwork).ToList();
        }
        /// <summary>
        /// 获取广域网IP地址
        /// </summary>
        public static IPAddress GetWanIP() {
            return getWanIP();
        }
        /// <summary>
        /// 获取本机广域网IP地址
        /// 访问网络需要设置代理信息时
        /// </summary>
        public static IPAddress GetWanIP(string host, int port, string userName, string password, string domain) {
            var proxy = new WebProxy(host, port) {
                Credentials = new NetworkCredential(userName, password, domain)
            };
            return getWanIP(proxy);
        }

        private static IPAddress getWanIP(IWebProxy proxy = null) {
            var client = new WebClient {
                Encoding = System.Text.Encoding.Default
            };
            if(proxy != null) {
                client.Proxy = proxy;
            }
            return getWanIP(client);
        }
        private static IPAddress getWanIP(WebClient client) {
            var reply = client.DownloadString("http://www.ip138.com/ips138.asp");
                //"http://20140507.ip138.com/ic.asp"
            //http://www.ip138.com/ip2city.asp; //http://city.ip138.com/city0.asp //http://iframe.ip138.com/city.asp  "http://" + DateTime.Today.ToString("yyyyMMdd") + ".ip138.com/ic.asp"
            var match = Regex.Match(reply, @"\[(?<ip>\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})\]");
            if(match.Success) {
                return IPAddress.Parse(match.Groups["ip"].Value);
            }
            return IPAddress.None;
        }
        /// <summary>
        /// 获取当前服务器的IP地址
        /// 仅适用于网站
        /// </summary>
        public static IPAddress GetServerIP() {
            var result = IPAddress.None;
            try {
                var addresses = Dns.GetHostAddresses(System.Web.HttpContext.Current.Request.Url.DnsSafeHost);
                foreach(var item in addresses) {
                    if(item.AddressFamily == AddressFamily.InterNetwork) {
                        result = item;
                        break;
                    }
                }
            } catch { }
            return result;
        }
        /// <summary>
        /// 获取访问者IP地址
        /// </summary>
        public static IPAddress GetRequestIP(System.Web.HttpRequest httpRequest) {
            var requestIP = HasUseProxy(httpRequest) ? httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"] : httpRequest.ServerVariables["REMOTE_ADDR"];
            IPAddress ipAddress;
            if(IPAddress.TryParse(requestIP, out ipAddress)) {
                return ipAddress;
            }
            return IPAddress.None;
        }
        /// <summary>
        /// 客户端是否使用了代理
        /// </summary>
        public static bool HasUseProxy(System.Web.HttpRequest request) {
            return !string.IsNullOrWhiteSpace(request.ServerVariables["HTTP_VIA"]);
        }
    }
}