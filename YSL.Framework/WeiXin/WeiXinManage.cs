using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeiXin.Public.Common;
using WeiXin.Public.Message;

namespace YSL.Framework.WeiXin
{
    /// <summary>
    /// 微信管理类
    /// </summary>
    public class WeiXinManage
    {
        /// <summary>
        /// 放在接收数据的页面即可 实现WxMsgHandler类的消息
        /// </summary>
        public static void StartAcceptRequest()
        {
            if (!ReceiveMessage.IsRgisterMessageHandler) ReceiveMessage.ResisterHandler(new WxMsgHandler());
            if (ReceiveMessage.IsRgisterMessageHandler)
            {
                var sign = EntrySign.ParseFromContext();
                //验签名
                if (sign.Check())
                {
                    //如果是GET请求就返回echostr数据
                    if (EntrySign.IsEntryCheck())
                    {
                        sign.Response();
                    }
                    else
                    {
                        //其他请求交给注册的实现接口IMessageHandler的类完成
                        var msg = ReceiveMessage.ParseFromContext();
                        var rep = msg.Process();
                        rep.Response();
                    }
                }
            }
        }
    }
}
