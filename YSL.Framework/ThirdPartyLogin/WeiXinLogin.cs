using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Extender;
using YSL.Common.Utility;
using YSL.Framework.Config;
using YSL.Framework.Config.Models;
namespace YSL.Framework.ThirdPartyLogin
{
    /// <summary>
    /// 微信第三方登陆
    /// </summary>
    public class WeiXinLogin
    {
        private string _appID = string.Empty;
        private string _appSecret = string.Empty;
        private string _callBackUrl = string.Empty;
        private Token token = null;
        private Union userinfo = null;
        public WeiXinLogin(string appID, string appSecret, string callBackUrl)
        {
            this._appID = appID;
            this._appSecret = appSecret;
            this._callBackUrl = callBackUrl;
        }
        public WeiXinLogin()
        {
            ThirdPartyPlatformSection cfg = ConfigManage.GetThePartyPlatform();
            if (cfg != null)
            {
                this._appID = cfg.WXLogin.AppId;
                this._appSecret = cfg.WXLogin.AppSecret;
                this._callBackUrl = cfg.WXLogin.CallBackUrl;
            }
        }
        /// <summary>
        /// 获取登陆url
        /// </summary>
        /// <param name="stateGuid"></param>
        /// <returns></returns>
        public string GetLoginUrl(string stateGuid)
        {
            return string.Format("https://open.weixin.qq.com/connect/qrconnect?appid={0}&redirect_uri={1}&response_type=code&scope=snsapi_login&state={2}#wechat_redirect",
                   this._appID,
                   this._callBackUrl.UrlEncode(),
                   stateGuid
                  );
        }
        /// <summary>
        /// 回调授权
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        public bool Authorized(string code)
        {
            bool IsAuthorized = false;
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
                 this._appID,
                 this._appSecret,
                 code
                );
            this.token = WebApiHelper.GetAsync<Token>(url).Result;
            if (token.expires_in == "7200"
                && !string.IsNullOrEmpty(token.access_token))
            {
                IsAuthorized = true;
            }
            return IsAuthorized;
        }
        /// <summary>
        /// 超时后刷新 AccessToken
        /// </summary>
        public void Refresh_AccessToken()
        {
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}",
                   this._appID,
                   this.token.access_token
                );
            var newTolen = WebApiHelper.GetAsync<Token>(url).Result;
            if (this.token != null)
                this.token.access_token = newTolen.access_token;
        }

        public void GetAccessToken()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}");
            this.token = WebApiHelper.GetAsync<Token>(url).Result;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public Union GetUserInfo()
        {
            if (token != null)
            {
                string url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}",
                    token.access_token,
                     this._appID
                    );
                this.userinfo = WebApiHelper.GetAsync<Union>(url).Result;
            }
            return this.userinfo;
        }

    }

    /// <summary>
    /// 授权成功后 有关access_token的信息
    /// </summary>
    public class Token
    {
        /// <summary>
        ///  获取资源需要的access_token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 过期时间 默认72小时
        /// </summary>
        public string expires_in { get; set; }
        /// <summary>
        /// 刷新token
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 登录公众号的用户标识 openid
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 范围
        /// </summary>
        public string scope { get; set; }
        /// <summary>
        /// 用户unionid
        /// </summary>
        public string unionid { get; set; }
    }

    /// <summary>
    /// 微信登录成功后该用户的信息
    /// </summary>
    public class Union
    {
        /// <summary>
        /// 用户的微信 openid
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 微信昵称
        /// </summary>

        public string nickname { get; set; }
        /// <summary>
        /// 性别
        /// </summary>

        public string sex { get; set; }
        /// <summary>
        /// 使用的语言
        /// </summary>

        public string language { get; set; }
        /// <summary>
        /// 普通用户个人资料填写的省份
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 普通用户个人资料填写的城市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 国家，如中国为CN
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空
        /// </summary>

        public string headimgurl { get; set; }
        /// <summary>
        /// 用户特权信息
        /// </summary>
        public JArray privilege { get; set; }
        /// <summary>
        /// 用户统一标识
        /// </summary>
        public string unionid { get; set; }

    }
}
