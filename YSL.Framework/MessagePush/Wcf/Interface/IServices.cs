using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.MessagePush.Interface
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICallBackServices))]
    public interface IServices
    {
        /// <summary>
        /// 注册客户端信息
        /// </summary>
        [OperationContract(IsOneWay = false)]
        void Register();
    }
}
