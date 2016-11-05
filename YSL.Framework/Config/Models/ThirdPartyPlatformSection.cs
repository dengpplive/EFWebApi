using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.Config.Models
{
    /// <summary>
    /// 第三方平台的配置信息
    /// </summary>
    public class ThirdPartyPlatformSection : ConfigurationSection
    {
        [ConfigurationProperty("wxlogin")]
        public WxLogin WXLogin
        {
            get { return (WxLogin)this["wxlogin"]; }
            set { this["wxlogin"] = value; }
        }
        public class WxLogin : ConfigurationElement
        {
            [ConfigurationProperty("appid", IsRequired = true)]
            public string AppId
            {
                get { return (string)this["appid"]; }
                set { this["appid"] = value; }
            }
            [ConfigurationProperty("appsecret", IsRequired = true)]
            public string AppSecret
            {
                get { return (string)this["appsecret"]; }
                set { this["appsecret"] = value; }
            }
            [ConfigurationProperty("callbackurl")]
            public string CallBackUrl
            {
                get { return (string)this["callbackurl"]; }
                set { this["callbackurl"] = value; }
            }
        }
    }
}
