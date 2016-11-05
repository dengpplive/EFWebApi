using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.MessagePush.Interface
{
    public interface ICallBackServices
    {
        /// <summary>
        /// 服务端客户端发送信息(异步)
        /// </summary>
        /// <param name="Message"></param>
        [OperationContract(IsOneWay = true)]
        void SendMessage(string Message);
    }
}
