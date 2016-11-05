using NetDimension.OpenAuth.Sina;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Extender;
namespace YSL.Framework.ThirdPartyLogin
{
    /// <summary>
    /// 新浪微博第三方登陆
    /// </summary>
    public class SinaClientLogin
    {
        private string _appkey = string.Empty;
        private string _appsecret = string.Empty;
        private string _callback_url = string.Empty;
        private string _accessToken = string.Empty;
        private string _uid = string.Empty;
        public bool IsAuthorized = false;
        public SinaWeiboClient _openAuth = null;
        public SinaClientLogin(string appkey, string appsecret, string callback_url, string access_token = "", string uid = "")
        {
            this._appkey = appkey;
            this._appsecret = appsecret;
            this._callback_url = callback_url;
            this._accessToken = access_token;
            this._uid = uid;
            this._openAuth = new SinaWeiboClient(this._appkey, this._appsecret, callback_url, access_token, uid);
        }
        /// <summary>
        /// 获取授权请求的url信息
        /// </summary>
        /// <returns></returns>
        public SinaUrl GetAuthorizationUrl()
        {
            return this._openAuth.GetAuthorizationUrl().FromJSON<SinaUrl>();
        }
        /// <summary>
        /// 通过回调返回的code进行授权
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        public bool Authorized(string code)
        {
            this._openAuth.GetAccessTokenByCode(code);
            if (this._openAuth.IsAuthorized)
            {
                this.IsAuthorized = true;
                this._accessToken = this._openAuth.AccessToken;
                this._uid = this._openAuth.UID;
                this._openAuth = new SinaWeiboClient(this._appkey, this._appsecret, this._callback_url, this._accessToken, this._uid);
            }
            return this._openAuth.IsAuthorized;
        }
        /// <summary>
        /// 通过认证后 调用api
        /// </summary>
        /// <param name="url"></param>
        /// <param name="dicParam"></param>
        /// <returns></returns>
        public string HttpGet(string url, Dictionary<string, object> dicParam)
        {
            string result = string.Empty;
            var response = this._openAuth.HttpGet(url, dicParam);            
            if (response.IsSuccessStatusCode)
            {
                result = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(result);
            }
            return result;
        }
        /// <summary>
        /// 获取登陆用户的资源信息
        /// </summary>
        /// <returns></returns>
        public JObject GetUserInfo()
        {
            JObject jObject = null;
            if (!string.IsNullOrEmpty(this._uid))
            {
                string url = "https://api.weibo.com/2/users/show.json";
                jObject = JObject.Parse(HttpGet(url, new Dictionary<string, object>
                {
                   { "uid" , this._uid}
                }));
            }
            return jObject;
        }

    }
    public class SinaUrl
    {
        public bool authorized { get; set; }
        public string url { get; set; }
    }
}
