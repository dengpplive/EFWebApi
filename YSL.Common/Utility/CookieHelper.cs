using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.Security;


namespace YSL.Common.Utility
{
    /// <summary>
    ///Cookie操作
    /// </summary>
    public class CookieHelper
    {
        #region Cookie操作
        /// <summary>
        /// 清除Cookie名称
        /// </summary>
        /// <param name="cookieName"></param>
        public static void ClearCookie(string cookieName)
        {
            ClearCookie(cookieName, string.Empty);
        }
        /// <summary>
        /// 清除Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieDomain"></param>
        public static void ClearCookie(string cookieName, string cookieDomain)
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            if (cookie != null)
            {
                cookie.Values.Clear();
                cookie.Expires = DateTime.Now.AddDays(-1);
                if (string.IsNullOrEmpty(cookieDomain) && HttpContext.Current.Request.Url.Host.IndexOf(cookieDomain) > -1 && IsValidDomain(HttpContext.Current.Request.Url.Host))
                    cookie.Domain = cookieDomain;
                HttpContext.Current.Response.AppendCookie(cookie);
                HttpContext.Current.Response.Cookies.Remove(cookieName);
                FormsAuthentication.SignOut();
            }
        }

        /// <summary>
        /// 写网站cookie值
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string cookieName, string strValue)
        {
           // WriteCookie(string.Empty, strValue, "/", 30 * 24 * 60 * 60, cookieName,string.Empty);
            WriteCookie(string.Empty, strValue, "/", 0, cookieName, string.Empty);
        }
        /// <summary>
        /// 写网站cookie值
        /// </summary>
        /// <param name="strName">项</param>
        /// <param name="strValue">值</param>
        /// <param name="path">cookie路径</param>
        /// <param name="second">过期时间(单位秒)</param>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="cookieDomain">Cookie域</param>
        public static void WriteCookie(string strName, string strValue, string path, int second, string cookieName, string cookieDomain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
                cookie.Path = "/";
                if (string.IsNullOrEmpty(strName)) cookie.Value = strValue;
                else cookie.Values[strName] = HttpUtility.UrlEncode(strValue);
                if (second > 0)
                {
                    cookie.Expires = DateTime.Now.AddSeconds(second);
                }
            }
            else
            {
                cookie.Path = "/";
                if (string.IsNullOrEmpty(strName)) cookie.Value = strValue;
                else cookie.Values[strName] = HttpUtility.UrlEncode(strValue);
                cookie.Expires = DateTime.Now.AddMinutes(second);
            }
            if (cookieDomain != string.Empty && HttpContext.Current.Request.Url.Host.IndexOf(cookieDomain) > -1 && IsValidDomain(HttpContext.Current.Request.Url.Host))
                cookie.Domain = cookieDomain;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 获得网站cookie值
        /// </summary>
        /// <param name="strName">项</param>
        /// <returns>值</returns>
        public static string GetCookie(string cookieName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[cookieName].Value);
            }
            return "";
        }
        /// <summary>
        /// 获得网站cookie值
        /// </summary>
        /// <param name="strName">项</param>
        /// <returns>值</returns>
        public static string GetCookie(string strName, string cookieName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[cookieName] != null && HttpContext.Current.Request.Cookies[cookieName][strName] != null)
            {
                return HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies[cookieName][strName].ToString());
            }
            return "";
        }
        /// <summary>
        /// 是否为有效域
        /// </summary>
        /// <param name="host">域名</param>
        /// <returns></returns>
        public static bool IsValidDomain(string host)
        {
            Regex r = new Regex(@"^\d+$");
            if (host.IndexOf(".") == -1)
            {
                return false;
            }
            return r.IsMatch(host.Replace(".", string.Empty)) ? false : true;
        }
        #endregion

        /// <summary>
        /// 单值cookie设定
        /// </summary>
        /// <param name="name">cookie对象的名称</param>
        /// <param name="value">cookie对象的值</param>
        /// <param name="minutes">失效时间：分钟</param>
        public static void SetCookie(string name, string value, int minutes)
        {
            HttpCookie cookie = new HttpCookie(name, value);

            cookie.Value = value;
            DateTime dt = DateTime.Now;
            TimeSpan ts = new TimeSpan(0, minutes, 0);
            cookie.Expires = dt.Add(ts);
            HttpContext.Current.Response.Cookies.Add(cookie);

        }
        /// <summary>
        /// 多值cookie设定，相当于一个cookie里面又有好多键值对
        /// </summary>
        /// <param name="name">cookie对象的名称</param>
        /// <param name="keyValue">cookie对象保存的键值对</param>
        /// <param name="minutes">失效时间：分钟</param>
        public static void SetCookie(string name, Dictionary<string, string> keyValue, int minutes)
        {
            HttpCookie cookie = new HttpCookie(name);

            foreach (var item in keyValue)
            {
                cookie.Values.Add(item.Key, item.Value);
            }
            DateTime dt = DateTime.Now;
            TimeSpan ts = new TimeSpan(0, minutes, 0);
            cookie.Expires = dt.Add(ts);
            HttpContext.Current.Response.Cookies.Add(cookie);

        }
        /// <summary>
        /// 单值cookie修改
        /// </summary>
        /// <param name="name">cookie对象的名称</param>
        /// <param name="value">cookie对象的值</param>
        /// <param name="minutes">失效时间：分钟</param>
        /// <returns>是否修改成功状态</returns>
        public static bool ModifyCookie(string name, string value, int minutes)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                cookie.Value = value;
                cookie.Expires = DateTime.Now.Add(new TimeSpan(0, minutes, 0));
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            return false;

        }
        /// <summary>
        /// 多值cookie修改
        /// </summary>
        /// <param name="name">cookie对象的名称</param>
        /// <param name="keyValue">cookie对象保存的键值对</param>
        /// <param name="minutes">失效时间：分钟</param>
        /// <returns>是否修改成功状态</returns>
        public static bool ModifyCookie(string name, Dictionary<string, string> keyValue, int minutes)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                foreach (var item in keyValue)
                {
                    cookie.Values[item.Key] = item.Value;
                }
                cookie.Expires = DateTime.Now.Add(new TimeSpan(0, minutes, 0));
                HttpContext.Current.Response.Cookies.Add(cookie);
                return true;
            }
            return false;

        }
        /// <summary>
        /// 删除cookie对象
        /// </summary>
        /// <param name="name">cookie对象的名称</param>
        /// <returns>是否删除成功的状态</returns>
        public static void DeleteCookie(string name)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.Add(new TimeSpan(0, -1, 0));
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

        }
        /// <summary>
        /// 取得单值cookie的值
        /// </summary>
        /// <param name="name">cookie对象的名称</param>
        /// <returns>对应的cookie值</returns>
        public static string GetCookieValue(string name)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                return cookie.Value;
            }
            return null;
        }
        /// <summary>
        /// 取得一个以name为名的多值cookie中的以key为键的cookie值
        /// </summary>
        /// <param name="name">cookie对象的名称</param>
        /// <param name="key">cookie对象下的键名称</param>
        /// <returns></returns>
        public static string GetCookieValue(string name, string key)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
            {
                return HttpContext.Current.Request.Cookies[name][key];
            }
            return null;
        }
    }
}
