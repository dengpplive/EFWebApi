using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Framework.MessagePush.Interface;

namespace YSL.Framework.MessagePush.Impl
{
    public class MessageCallBack : ICallBackServices
    {
        #region ICallBackServices 成员

        public void SendMessage(string Message)
        {

            //Console.WriteLine("[ClientTime{0:HHmmss}]Service Broadcast:{1}", DateTime.Now, Message);

        }
        #endregion
    }
}
