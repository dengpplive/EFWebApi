using JSL.DataEntity.ApiModel;
using JSL.DataEntity.ApiModel.Request;
using JSL.EFDataContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using YSL.Business.Interface;
using YSL.Common.Extender;
using YSL.Common.MessagePackage;
using YSL.Common.Log;
using YSL.Framework.WeiXin;
//api调用业务层接口方法
namespace YSL.Api
{
    /// <summary>
    /// 例子
    /// </summary>   
    public class WeiXinApiController : ApiControllerBase
    {

        /// <summary>
        /// 构造函数
        /// </summary>      
        public WeiXinApiController()
        {

        }
        [AcceptVerbs("GET", "POST")]
        public void TestAPI()
        {
            WeiXinManage.StartAcceptRequest();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Result Login()
        {
            Logger.WriteLog(LogType.DEBUG, "12121222Login");

            return CommonResult(() =>
           {
               //调用业务层 接口

               return new Result()
               {

               };
           }
            , r => Console.WriteLine(r.ToJSON()));
        }


    }
}
