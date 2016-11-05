using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using Microsoft.Owin;
using YSL.Common.Extender;
namespace YSL.Host.InitConfig
{
    public class WebApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnException(actionExecutedContext);
            var request = actionExecutedContext.Request;
            var owinContext = request.Properties["MS_OwinContext"] as OwinContext;
            var ipAddress = "";
            if (owinContext!=null)
            {
                ipAddress=owinContext.Request.RemoteIpAddress;
            }
            var refererUrl = request.Headers.Referrer == null ? "" : request.Headers.Referrer.ToString();
            var userAgent = request.Headers.UserAgent == null ? "" : request.Headers.UserAgent.ToString();
            var contentType = request.Content.Headers.ContentType == null ? "" : request.Content.Headers.ContentType.ToString();
            var data = request.Content.ReadAsStringAsync().Result;
            var method = request.Method.ToString();
            var requestUrl = request.RequestUri.AbsoluteUri;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("IpAddress:{0}\t\n", ipAddress);
            sb.AppendFormat("UserAgent:{0}\t\n", userAgent);
            sb.AppendFormat("Referer:{0}\t\n", refererUrl);
            sb.AppendFormat("Url:{0}\t\n", requestUrl);
            sb.AppendFormat("ContentType:{0}\t\n", contentType);
            sb.AppendFormat("Method:{0}\t\n", method);
            sb.AppendFormat("Data:{0}\t\n", data);
            sb.AppendFormat("Exception:{0}\t\n", actionExecutedContext.Exception.MostInnerException().Message);
            //错误日志
#if !DEBUG
                         LogBuilder.Log4Net.Error(sb.ToString());
#else
            Console.WriteLine(sb.ToString());
#endif
        }
    }
}
