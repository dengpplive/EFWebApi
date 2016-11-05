using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YSL.Common.MessagePackage
{
    /// <summary>
    /// 响应状态
    /// </summary>
    public class ResponseExtensionData
    {
        /// <summary>
        /// 请求响应状态
        /// </summary>
        [Display(Name = "请求响应状态")]
        public CallResult CallResult { get; set; }

        /// <summary>
        /// 响应状态描述
        /// </summary>
        [Display(Name = "响应状态描述")]
        public string RetMsg { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        [Display(Name = "附加数据")]
        public IList<ModelValidateError> ModelValidateErrors { get; set; }

    }

    /// <summary>
    /// 请求状态
    /// </summary>
    public enum CallResult
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        [Display(Name = "请求成功")]
        Success = 1,

        /// <summary>
        /// 参数格式验证失败
        /// </summary>
        [Display(Name = "参数格式验证失败")]
        ModelError = 2,

        /// <summary>
        /// 数据值错误
        /// </summary>
        [Display(Name = "数据值错误")]
        ParameterError = 3,

        /// <summary>
        /// 身份验证未通过
        /// </summary>
        [Display(Name = "身份验证未通过")]
        Unauthorized = 4,

        /// <summary>
        /// 账户信息错误
        /// </summary>
        [Display(Name = "账户信息错误")]
        AccountError = 5,

        /// <summary>
        /// 请求失败
        /// </summary>
        [Display(Name = "请求失败")]
        Failed = 6,

        /// <summary>
        /// 业务逻辑错误
        /// </summary>
        [Display(Name = "业务逻辑错误")]
        BusinessError = 7
    }
}