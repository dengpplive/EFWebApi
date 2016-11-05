using QConnectSDK;
using QConnectSDK.Context;
using QConnectSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.ThirdPartyLogin
{
    /// <summary>
    /// QQ第三方登陆
    /// </summary>
    public class QQLogin
    {
        /// <summary>
        /// 附加信息 回调使用
        /// </summary>
        string _state { get; set; }
        string _scope = "get_user_info,add_share,list_album,upload_pic,check_page_fans,add_t,add_pic_t,del_t,get_repost_list,get_info,get_other_info,get_fanslist,get_idolist,add_idol,del_idol,add_one_blog,add_topic,get_tenpay_addr";
        /// <summary>
        /// 获取授权登陆地址
        /// </summary>
        /// <returns></returns>
        public string GetAuthenticationUrl(string guid)
        {
            string url = string.Empty;
            if (!string.IsNullOrEmpty(guid))
            {
                this._state = guid;
                var content = new QzoneContext();
                url = content.GetAuthorizationUrl(this._state, this._scope);
            }
            return url;
        }
        /// <summary>
        /// 回调处理
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="ac"></param>
        /// <returns></returns>
        public bool QQCallback(string code, string state, Action<User> ac)
        {
            bool IsPass = false;
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state)) return IsPass;
            if (state == this._state)
            {
                var qzone = new QOpenClient(code, state);
                var currentUser = qzone.GetCurrentUser();
                if (ac != null && currentUser != null) ac(currentUser);
                return true;
            }
            return IsPass;
        }
    }
}
