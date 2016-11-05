using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using YSL.Common.Log;
namespace YSL.Host.InitConfig
{
    /// <summary>
    /// 启动wcf的服务
    /// </summary>
    public class WcfStartUp
    {
        //是否启动完成
        private static bool isGoOn = false;
        /// <summary>
        /// 启动wcf服务
        /// </summary>
        public static void StartWcf()
        {
            Type[] types = {
                             // typeof(DateTimeService)
                             //....
                            };
            RegisterWcf(types);
        }

        /// <summary>
        /// 注册wcf的服务类型
        /// </summary>
        /// <param name="sercicesTypes">服务类型数组</param>
        private static void RegisterWcf(Type[] sercicesTypes = null)
        {
            if (sercicesTypes != null && sercicesTypes.Length > 0)
            {
                Func<Type[]> funcs = new Func<Type[]>(() => sercicesTypes);
                IAsyncResult result = funcs.BeginInvoke(new AsyncCallback(Callback), funcs);
                while (true)
                {
                    if (isGoOn)
                    {
                        //处理其他的事情
                        //.....
                    }
                    Thread.Sleep(1);
                }
            }
        }
        /// <summary>
        /// 异步回调
        /// </summary>
        /// <param name="ar"></param>
        private static void Callback(IAsyncResult ar)
        {
            try
            {
                Func<Type[]> funcs = (Func<Type[]>)ar.AsyncState;
                Type[] types = null;
                if (null != funcs)
                {
                    types = funcs.EndInvoke(ar);
                }
                foreach (var t in types)
                {
                    ServiceHost host = new ServiceHost(t);
                    host.Opened += (p, q) =>
                    {
                        Console.WriteLine(t.Name + "启动成功");
                    };
                    host.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("启动Wcf服务异常:" + ex.Message);
                Logger.WriteLog(LogType.FATAL, "启动Wcf服务失败", ex);
            }
            finally
            {
                isGoOn = true;
            }
        }
    }
}
