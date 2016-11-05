using YSL.Host.InitConfig;
using Autofac;
using Autofac.Configuration;
using Autofac.Integration.WebApi;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Reflection;
using System.Web;
using System.Web.Http;
using YSL.Common;
using YSL.Common.Log;
using YSL.Common.Resources;

namespace YSL.Host.Route
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            //加载指定的api程序集
            ApiAssembie.LoadApiAssembie(ApiConfig.ApiAssemblies);
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
           
            
            //ioc控制反转
            ApiIocConfig.Register(config);
            //注册异常过滤器
            config.Filters.Add(new WebApiExceptionFilter());
            //ApiExplorerConfig
            ApiExplorerConfig.Register(config);
            //启动日志
            LogBuilder.InitLog4Net("APILog");

            config.Formatters.Clear();
            config.Formatters.Insert(0, new JsonpMediaTypeFormatter());
            var serializerSettings = config.Formatters.JsonFormatter.SerializerSettings;
            var contractResolver = (DefaultContractResolver)serializerSettings.ContractResolver;
            contractResolver.IgnoreSerializableAttribute = true;
            //异步请求消息
            config.MessageHandlers.Add(new MessageDispatcher());

            //启用跨域
            //GlobalConfiguration.Configuration.EnableCors();
            //config.EnableCors();

            //api的路由设置
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });
            //添加到中间件middleware
            appBuilder.UseWebApi(config);

        }
    }
}
