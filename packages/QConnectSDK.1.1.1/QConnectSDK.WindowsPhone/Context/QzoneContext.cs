using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QConnectSDK.Config;
using System.Net;
using System.IO;
using RestSharp;
using QConnectSDK.Exceptions;
using RestSharp.Deserializers;
using QConnectSDK.Models;
using QConnectSDK.Api;

namespace QConnectSDK.Context
{
    /// <summary>
    /// QQ登陆的上下文数据
    /// </summary>
    public class QzoneContext
    {
        private RestApi restApi;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="authVericode"></param>
        public QzoneContext(string authVericode)
        {
            this.oAuthVericode = authVericode;
            this.config = new QQConnectConfig();
            this.restApi = new RestApi(this);         
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public QzoneContext()
            :this(string.Empty)
        {
            
        }

        public QzoneContext(OAuthToken oAuthToken)
        {
            this.oAuthToken = oAuthToken;
            this.config = new QQConnectConfig();
        }

        private QQConnectConfig config = null;

        /// <summary>
        /// 配置数据
        /// </summary>
        public QQConnectConfig Config
        {
            get { return config; }
            set { config = value; }
        }

        /// <summary>
        /// 获取Authorization Code的URL地址
        /// </summary>
        /// <param name="scope">请求用户授权时向用户显示的可进行授权的列表。可填写的值是【QQ登录】API文档中列出的接口，
        /// 以及一些动作型的授权（目前仅有：do_like），如果要填写多个接口名称，请用逗号隔开。
        /// 例如：scope=get_user_info,add_share,list_album,upload_pic,check_page_fans,add_t,add_pic_t,del_t,get_repost_list,get_info,get_other_info 
        /// get_fanslist,get_idolist,add_idol,del_idol
        /// 不传则默认请求对接口get_user_info进行授权。
        /// 建议控制授权项的数量，只传入必要的接口名称，因为授权项越多，用户越可能拒绝进行任何授权。</param>
        /// <returns></returns>
        public string GetAuthorizationUrl(string scope="")
        {
            string url = string.Empty;
            if (string.IsNullOrEmpty(scope))
            {
                url = string.Format("{0}?response_type=token&client_id={1}&redirect_uri={2}&display=mobile", config.GetAuthorizeURL(), config.GetAppKey(), config.GetCallBackURI().ToString());
            }
            else
            {
                url = string.Format("{0}?response_type=token&client_id={1}&redirect_uri={2}&scope={3}&display=mobile", config.GetAuthorizeURL(), config.GetAppKey(), config.GetCallBackURI().ToString(),scope);
            }
            return url;
        }


        private string oAuthVericode;
        /// <summary>
        /// The secret to the Authorized OAuth Access Token granted to you from 
        /// </summary>
        public string OAuthVericode
        {
            get
            {
                return oAuthVericode;
            }
            set
            {
                oAuthVericode = value;
            }
        }

        private OAuthToken oAuthToken;

        /// <summary>
        /// 通过Authorization Code获取Access Token
        /// </summary>
        public OAuthToken AccessToken
        {
            get
            {
                return oAuthToken;
            }
            set
            {
                oAuthToken = value;
            }
        }      

      
    }
}
