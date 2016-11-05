using YSL.Host.Route;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YSL.Common.Log;
using YSL.Framework.ThirdPartyLogin;
namespace YSL.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test();
            //Bootstrapper.Start();
            Bootstrapper.Start(Test);
            Console.ReadLine();
        }



        public static void Test()
        {
            string strGuid = Guid.NewGuid().ToString().Replace("-", "");
            WeiXinLogin login = new WeiXinLogin();
            string url = login.GetLoginUrl(strGuid);

            Logger.WriteLog(LogType.DEBUG, "url:" + url);
        }
    }
}
