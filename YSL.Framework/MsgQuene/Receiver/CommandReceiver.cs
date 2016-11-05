using System;
using System.Messaging;
using System.Text.RegularExpressions;

namespace YSL.Framework.MsgQuene.Receiver
{
    public abstract class CommandReceiver : BaseReceiver
    {
        public Action<int, DateTime, Guid?> AdjustPriceCommandReceived;
        protected void OnCommandReceived(string label, Message msg)
        {
            if (AdjustPriceCommandReceived != null)
            {
                switch (label)
                {
                    case "":
                        {
                            //处理消息
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}