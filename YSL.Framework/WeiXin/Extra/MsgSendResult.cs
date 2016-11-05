namespace WeiXin.Public.Common.Extra
{
    public class MsgSendResult
    {
        public int ret { get; set; }

        public string msg { get; set; }

        public bool IsSuccess { get { return ret == 0; } }
    }
}