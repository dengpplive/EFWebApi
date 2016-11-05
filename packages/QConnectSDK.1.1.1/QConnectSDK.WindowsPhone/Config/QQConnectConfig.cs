using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows;

namespace QConnectSDK.Config
{
 
    public class QQConnectConfig
    {
        private static IAppSettings _Settings;
        private string _Namespace;
        internal string Namespace { get { return _Namespace ?? (_Namespace = Application.Current.ToString().Replace(".App", string.Empty)); } }
        public IAppSettings Settings { get { return _Settings ?? (_Settings = new AppSettings(Namespace)); } }

        public string GetAuthorizeURL()
        {
            return Settings["AuthorizeURL"];
        }
        /// <summary>
        /// 申请QQ登录成功后，分配给应用的appid
        /// </summary>
        /// <returns>string AppKey</returns>
        public string GetAppKey()
        {
            return Settings["AppKey"];
        }

        /// <summary>
        /// 申请QQ登录成功后，分配给网站的appkey
        /// </summary>
        /// <returns>string AppSecret</returns>
        public string GetAppSecret()
        {
            return Settings["AppSecret"];
        }

        /// <summary>
        /// 得到回调地址
        /// </summary>
        /// <returns></returns>
        public string GetCallBackURI()
        {
            return Settings["CallBackURI"];
        }
    }
}
