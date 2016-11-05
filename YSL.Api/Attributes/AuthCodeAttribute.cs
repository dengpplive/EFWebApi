using System;
using Autofac;
using System.Net;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using YSL.Common.MessagePackage;
using System.Collections.Generic;
using System.Web.Http.Controllers;
namespace YSL.Api.Filters
{
    /// <summary>
    /// 检测请求是否可信任
    /// 不需登录,请求头部需要包含可识别的appkey   
    /// </summary>
    public class CheckAppAttribute : ActionFilterAttribute
    {
        private const string EncryptValue = "5bvv"; // TODO 可配置 AxOne
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
    }

    /// <summary>
    /// API检测登录过滤器(需要登录之后才能调用API接口)
    /// Author : axone
    /// </summary>
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //var headers = filterContext.Request.Headers;
            //var qs = HttpUtility.ParseQueryString(filterContext.Request.RequestUri.Query);
            //var uid = Convert.ToInt32(qs["uid"] ?? "0");
            //var type = Convert.ToInt32(qs["type"] ?? "0");
            //if (headers.Contains("token") && uid > 0 && type > 0)
            //{
            //    var userType = (AuthUserType)2;
            //    if (type == 1)
            //        userType = AuthUserType.Admin;
            //    else if (type == 2)
            //        userType = AuthUserType.General;
            //    else
            //        filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, new { Success = false, ExceptionMessage = LanguageUtil.Translate("api_Filters_AuthCodeAttribute_CheckLoginAttribute_OnActionExecuting_AuthUserType") });
            //    var tokenEncryptStr = headers.GetValues("token").First();
            //    var authKeys = ((IContainer)HttpRuntime.Cache["containerKey"]).Resolve<IAuthKeysBusiness>().GetAuthKeys(uid, userType);
            //    if (string.IsNullOrEmpty(tokenEncryptStr) || authKeys == null || authKeys.PrivateKey == null)
            //    {
            //        filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, new { Success = false, ExceptionMessage = LanguageUtil.Translate("api_Filters_AuthCodeAttribute_CheckLoginAttribute_OnActionExecuting_AuthKeys") });
            //        return;
            //    }
            //    var tokenDecryptStr = RSAHelper.DecryptString(tokenEncryptStr, authKeys.PrivateKey);
            //    if (string.IsNullOrWhiteSpace(tokenDecryptStr) || !string.Equals(tokenDecryptStr, uid.ToString()))
            //    {
            //        filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, new { Success = false, ExceptionMessage = LanguageUtil.Translate("api_Filters_AuthCodeAttribute_CheckLoginAttribute_OnActionExecuting_token") });
            //        return;
            //    }
            //}
            //else
            //{
            //    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, new { Success = false, ExceptionMessage = LanguageUtil.Translate("api_Filters_AuthCodeAttribute_CheckLoginAttribute_OnActionExecuting_token_null") });
            //    return;
            //}
            base.OnActionExecuting(filterContext);
        }
    }

    /// <summary>
    /// APP_API检测登录过滤器(需要登录之后才能调用API接口)
    /// </summary>
    public class CheckAppLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //var headers = filterContext.Request.Headers;
            //var result = new ResponsePackage<object>();
            //var extionData = new ResponseExtensionData
            //{
            //    ModelValidateErrors = new List<ModelValidateError>(),
            //    CallResult = CallResult.Unauthorized,
            //    RetMsg = LanguageUtil.Translate("api_Filters_AuthCodeAttribute_CheckAppLoginAttribute_OnActionExecuting_RetMsg")
            //};
            //if (!headers.Contains("uid"))
            //{
            //    result.Data = null;
            //    result.ExtensionData = extionData;
            //    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, result);
            //    return;
            //}
            //var uid = Convert.ToInt32(headers.GetValues("uid").First());
            //if (headers.Contains("token") && uid > 0)
            //{
            //    const AuthUserType userType = (AuthUserType)2;
            //    var tokenEncryptStr = headers.GetValues("token").First();
            //    var authKeys = ((IContainer)HttpRuntime.Cache["containerKey"]).Resolve<IAuthKeysBusiness>().GetAuthKeys(uid, userType);
            //    if (string.IsNullOrEmpty(tokenEncryptStr) || authKeys == null || authKeys.PrivateKey == null)
            //    {
            //        result.Data = null;
            //        result.ExtensionData = extionData;
            //        filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, result);
            //        return;
            //    }
            //    var tokenDecryptStr = RSAHelper.DecryptString(tokenEncryptStr, authKeys.PrivateKey);
            //    if (string.IsNullOrWhiteSpace(tokenDecryptStr) || !string.Equals(tokenDecryptStr, uid.ToString()))
            //    {
            //        result.Data = null;
            //        result.ExtensionData = extionData;
            //        filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, result);
            //        return;
            //    }
            //}
            //else
            //{
            //    result.Data = null;
            //    result.ExtensionData = extionData;
            //    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, result);
            //    return;
            //}
            base.OnActionExecuting(filterContext);
        }
    }

    /// <summary>
    /// 参数绑定数据验证
    /// </summary>
    public class ModelValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //var modelState = actionContext.ModelState;
            //if (modelState.IsValid &&
            //    (actionContext.ActionArguments.Count != 1 || actionContext.ActionArguments.ElementAt(0).Value != null))
            //{
            //    return;
            //}
            //IList<ModelValidateError> errors;
            //if (modelState.IsValid)
            //{
            //    errors = new List<ModelValidateError>{
            //        new ModelValidateError
            //        {
            //            Key=LanguageUtil.Translate("api_Filters_AuthCodeAttribute_ModelValidateAttribute_OnActionExecuting_Key"),
            //            Value=LanguageUtil.Translate("api_Filters_AuthCodeAttribute_ModelValidateAttribute_OnActionExecuting_Value")
            //        }
            //    };
            //}
            //else
            //{
            //    errors = modelState.Where(w => modelState[w.Key].Errors.Any()).Select(w => new ModelValidateError { Key = w.Key, Value = string.Join(";", w.Value.Errors.Select(t => t.ErrorMessage)) }).ToList();
            //}
            //string msg;
            //if (errors.Any() && !string.IsNullOrWhiteSpace(errors.First().Value))
            //{
            //    msg = errors.First().Value;
            //}
            //else
            //{
            //    msg = LanguageUtil.Translate("api_Filters_AuthCodeAttribute_ModelValidateAttribute_OnActionExecuting_msg");
            //}
            var data = new ResponseExtensionData
            {
                CallResult = CallResult.ModelError,
                RetMsg = "",
                ModelValidateErrors = ""
            };
            actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponseByReturnType(actionContext, data);
        }
    }
}
