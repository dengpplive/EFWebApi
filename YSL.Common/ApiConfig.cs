using System;
using System.Collections;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace YSL.Common
{
    public static class ApiConfig
    {
        /// <summary>
        /// webapi启动服务地址
        /// </summary>
        public static string WebApiHost = ConfigurationManager.AppSettings["WebApiHost"];
        //请求的基地址
        public static string BaseAddress = ConfigurationManager.AppSettings["BaseAddress"];
        /// <summary>
        /// 服务监听地址
        /// </summary>
        /// <returns></returns>
        public static string ServicePort
        {
            get
            {
                if (!ConfigurationManager.AppSettings.AllKeys.Contains("ServicePort"))
                {
                    return string.Empty;
                }
                string ip = GetLocalIP;
                string strport = ConfigurationManager.AppSettings["ServicePort"].Trim();
                int port;
                bool res = int.TryParse(strport, out port);
                if (!res)
                {
                    throw new ArgumentException("服务层监听端口配置错误");
                }
                return string.Format("http://{0}:{1}/", ip, port);
            }
        }
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP
        {
            get
            {
                var hostName = Dns.GetHostName();
                IPHostEntry localhost = Dns.GetHostEntry(hostName);
                IPAddress localAddress = localhost.AddressList.FirstOrDefault(add => add.AddressFamily == AddressFamily.InterNetwork);
                return localAddress != null ? localAddress.ToString() : string.Empty;
            }

        }
        public static ArrayList ApiAssemblies
        {
            get
            {
                var assemblies = new ArrayList();
                string assemblie = ConfigurationManager.AppSettings["ApiAssemblies"];
                if (string.IsNullOrEmpty(assemblie))
                {
                    assemblies.Add("YSL.Api.dll");
                    return assemblies;
                }
                assemblies.AddRange(assemblie.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                return assemblies;
            }
        }

        /// <summary>
        /// Url路径合并
        /// </summary>
        /// <param name="paths">路径列表</param>
        /// <returns>合并后的返回值</returns>
        public static string Combine(params string[] paths)
        {
            string returnStr = string.Empty;

            if (paths != null && paths.Length > 0)
            {
                for (int i = 1; i < paths.Length; i++)
                {
                    if (returnStr.EndsWith("/") || paths[i].StartsWith("/"))
                        returnStr += paths[i];
                    else
                        returnStr += "/" + paths[i];
                }
            }
            return returnStr;
        }
    }
}
