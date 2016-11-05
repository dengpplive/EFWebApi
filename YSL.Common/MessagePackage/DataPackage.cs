using System.ComponentModel.DataAnnotations;

namespace YSL.Common.MessagePackage
{
    /// <summary>
    /// 响应结果包装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataPackage<T>
    {
        [Display(Name = "响应数据")]
        public T Data { get; set; }
        [Display(Name = "安全校验，响应状态等信息")]
        public ResponseExtensionData ExtensionData { get; set; }
    }
    /// <summary>
    /// 请求数据包
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequestPackage<T>
    {
        [Display(Name = "请求数据")]
        public T Data { get; set; }
    }
}
