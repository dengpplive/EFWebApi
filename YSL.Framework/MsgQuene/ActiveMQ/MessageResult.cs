using Apache.NMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.MsgQuene.ActiveMQ
{
    public class MessageResult
    {
        public object Custom { get; set; }
        public IMessage IMessage { get; set; }
    }
}
