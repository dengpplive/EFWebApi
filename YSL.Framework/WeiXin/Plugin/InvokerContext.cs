using WeiXin.Public.Message;
namespace WeiXin.Public.Plugin
{
    /// <summary>
    /// Invoker的上下文环境
    /// </summary>
    public class InvokerContext
    {
        public ReceiveMessage Message { get; set; }
    }
}