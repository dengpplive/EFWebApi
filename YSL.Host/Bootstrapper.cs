using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common;
using Microsoft.Owin.Hosting;
using YSL.Host.Route;
using YSL.Host.InitConfig;
namespace YSL.Host
{
    public class Bootstrapper
    {
        /// <summary>
        /// 启动
        /// </summary>
        public static void Start(Action testAction = null)
        {
            //启动WCF服务
            WcfStartUp.StartWcf();
            //使用owin或者windows 服务启动webapi
            using (WebApp.Start<Startup>(url: ApiConfig.WebApiHost))
            {
                Console.WriteLine(ApiConfig.WebApiHost);
                Console.WriteLine("API服务已经启动......");
                if (testAction != null) testAction();
                Console.ReadLine();
            }
        }
    }
}
