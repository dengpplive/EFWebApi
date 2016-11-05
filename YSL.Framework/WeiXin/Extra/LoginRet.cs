using System.Web.UI.WebControls;

namespace WeiXin.Public.Common.Extra
{
    public class LoginRet
    {
        public int Ret { get; set; }

        /// <summary>
        /// 消息。成功后从此处取结果
        /// </summary>
        public string ErrMsg { get; set; }

        public int ShowVerifyCode { get; set; }

        public int ErrCode { get; set; }

        public bool IsSuccess { get { return ErrCode == 0; } }
    }
}