using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using YSL.Framework.MessagePush.Interface;

namespace YSL.Framework.MessagePush.Impl
{
    /// <summary>
    ///  实例使用Single，共享一个
    ///  并发使用Mutiple, 支持多线程访问(一定要加锁)
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MessageServices : IServices
    {
        //ConfigurationManager.ConnectionStrings["SendMessageType"].ToString();
        public static readonly string SendMessageType = "SESSIONID";
        //单一实例
        private static readonly object InstObj = new object();
        //记录机器名称
        public static Dictionary<string, ICallBackServices> DicHost = null;
        //记录Sessionid
        public static Dictionary<string, ICallBackServices> DicHostSess = null;


        public MessageServices()
        {
            DicHost = new Dictionary<string, ICallBackServices>();
            DicHostSess = new Dictionary<string, ICallBackServices>();
        }
        #region IServices 成员
        public void Register()
        {
            ICallBackServices client = OperationContext.Current.GetCallbackChannel<ICallBackServices>();
            //获取当前机器Sessionid--------------------------如果多个客户端在同一台机器，就使用此信息。
            string sessionid = OperationContext.Current.SessionId;
            //获取当前机器名称-----多个客户端不在同一台机器上，就使用此信息。
            string ClientHostName = OperationContext.Current.Channel.RemoteAddress.Uri.Host;
            //注册客户端关闭触发事件
            OperationContext.Current.Channel.Closing += new EventHandler(Channel_Closing);
            if (SendMessageType.ToUpper() == "SESSIONID")
            {
                DicHostSess.Add(sessionid, client);//添加
            }
            else
            {
                DicHost.Add(ClientHostName, client); //添加
            }
        }
        void Channel_Closing(object sender, EventArgs e)
        {
            lock (InstObj)//加锁，处理并发
            {
                //if (RegList != null && RegList.Count > 0)
                //    RegList.Remove((ICallBackServices)sender); 
                if (SendMessageType.ToUpper() == "SESSIONID")
                {
                    if (DicHostSess != null && DicHostSess.Count > 0)
                    {
                        foreach (var d in DicHostSess)
                        {
                            if (d.Value == (ICallBackServices)sender)//删除此关闭的客户端信息
                            {
                                DicHostSess.Remove(d.Key);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (DicHost != null && DicHost.Count > 0) //同上
                    {
                        foreach (var d in DicHost)
                        {
                            if (d.Value == (ICallBackServices)sender)
                            {
                                DicHost.Remove(d.Key);
                                break;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}

