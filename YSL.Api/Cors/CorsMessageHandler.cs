using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Controllers;

public class CorsMessageHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //得到描述目标Action的HttpActionDescriptor
        HttpMethod originalMethod = request.Method;
        bool isPreflightRequest = request.IsPreflightRequest();
        if (isPreflightRequest)
        {
            string method = request.Headers.GetValues("Access-Control-Request-Method").First();
            request.Method = new HttpMethod(method);
        }
        HttpConfiguration configuration = request.GetConfiguration();
        HttpControllerDescriptor controllerDescriptor = configuration.Services.GetHttpControllerSelector().SelectController(request);
        HttpControllerContext controllerContext = new HttpControllerContext(request.GetConfiguration(), request.GetRouteData(), request)
        {
            ControllerDescriptor = controllerDescriptor
        };
        HttpActionDescriptor actionDescriptor = configuration.Services.GetActionSelector().SelectAction(controllerContext);

        //根据HttpActionDescriptor得到应用的CorsAttribute特性
        CorsAttribute corsAttribute = actionDescriptor.GetCustomAttributes<CorsAttribute>().FirstOrDefault() ??
            controllerDescriptor.GetCustomAttributes<CorsAttribute>().FirstOrDefault();
        if (null != corsAttribute)
        {
            //利用CorsAttribute实施授权并生成响应报头
            IDictionary<string, string> headers;
            request.Method = originalMethod;
            bool authorized = corsAttribute.TryEvaluate(request, out headers);
            HttpResponseMessage response;
            if (isPreflightRequest)
            {
                if (authorized)
                {
                    response = new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, corsAttribute.ErrorMessage);
                }
            }
            else
            {
                response = base.SendAsync(request, cancellationToken).Result;
            }

            //添加响应报头
            foreach (var item in headers)
            {
                response.Headers.Add(item.Key, item.Value);
            }
            return Task.FromResult<HttpResponseMessage>(response);
        }
        else
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}


//GlobalConfiguration.Configuration.MessageHandlers.Add(new MyCorsMessageHandler());
public class MyCorsMessageHandler : DelegatingHandler
{
    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //根据当前请求创建CorsRequestContext
        CorsRequestContext context = request.CreateCorsRequestContext();

        //针对非预检请求：将请求传递给消息处理管道后续部分继续处理，并得到响应
        HttpResponseMessage response = null;
        if (!context.IsPreflight)
        {
            response = await base.SendAsync(request, cancellationToken);
        }

        //利用注册的CorsPolicyProviderFactory得到对应的CorsPolicyProvider
        //借助于CorsPolicyProvider得到表示CORS资源授权策略的CorsPolicy
        HttpConfiguration configuration = request.GetConfiguration();
        CorsPolicy policy = await configuration.GetCorsPolicyProviderFactory().GetCorsPolicyProvider(request).GetCorsPolicyAsync(request, cancellationToken);

        //获取注册的CorsEngine
        //利用CorsEngine对请求实施CORS资源授权检验，并得到表示检验结果的CorsResult对象
        ICorsEngine engine = configuration.GetCorsEngine();
        CorsResult result = engine.EvaluatePolicy(context, policy);

        //针对预检请求
        //如果请求通过授权检验，返回一个状态为“200， OK”的响应并添加CORS报头
        //如果授权检验失败，返回一个状态为“400， Bad Request”的响应并指定授权失败原因
        if (context.IsPreflight)
        {
            if (result.IsValid)
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.AddCorsHeaders(result);
            }
            else
            {
                response = request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(" |", result.ErrorMessages.ToArray()));
            }
        }
        //针对非预检请求
        //CORS报头只有在通过授权检验情况下才会被添加到响应报头集合中
        else if (result.IsValid)
        {
            response.AddCorsHeaders(result);
        }
        return response;
    }
}

