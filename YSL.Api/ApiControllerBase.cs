using JSL.DataEntity.ApiModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using YSL.Common.Assert;
using YSL.Common.Log;
using YSL.Common.Extender;
using YSL.Common.Resources;
namespace YSL.Api
{
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        /// 增删改调用的函数
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="infoLog"></param>
        /// <returns></returns>
        protected Result CommonResult(Func<object> handler, Action<Result> output = null)
        {
            var result = new Result();
            try
            {
                AssertUtil.IsNotNull(handler, Constant.CallFunNotNull);
                var o = handler();
                result.Data = o;
                if (output != null) output(result);
            }
            catch (AssertException ex) //断言异常
            {
                result.Status = false;
                result.Message = ex.MostInnerException().Message;
                LogBuilder.Log4Net.Info(result.Message);
            }
            return result;
        }
    }
}
