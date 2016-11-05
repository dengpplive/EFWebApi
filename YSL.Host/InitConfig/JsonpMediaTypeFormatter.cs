using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace YSL.Host.InitConfig
{
   public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {

        private string _callbackQueryParameter;

        public JsonpMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(DefaultMediaType);
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
            MediaTypeMappings.Add(new UriPathExtensionMapping("jsonp", DefaultMediaType));
        }

        public string CallbackQueryParameter
        {
            get { return _callbackQueryParameter ?? "callback"; }
            set { _callbackQueryParameter = value; }
        }

        /// <summary>
        /// 将对象序列化后的JSON字符串填充到JavaScript回调函数中
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        /// <param name="content"></param>
        /// <param name="transportContext"></param>
        /// <returns></returns>
        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            string callback;

            if (IsJsonpRequest(out callback))
            {
                return Task.Factory.StartNew(() =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(callback + "(");
                    writer.Flush();

                    base.WriteToStreamAsync(type, value, stream, content, transportContext).Wait();

                    writer.Write(")");
                    writer.Flush();
                });
            }
            else
            {
                return base.WriteToStreamAsync(type, value, stream, content, transportContext);
            }
        }

        /// <summary>
        /// 判断是否为跨域请求
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        private bool IsJsonpRequest(out string callback)
        {
            callback = null;
            if (HttpContext.Current==null)
            {
                return false;
            }
            if (HttpContext.Current.Request.HttpMethod != "GET")
                return false;

            callback = HttpContext.Current.Request.QueryString[CallbackQueryParameter];

            return !string.IsNullOrEmpty(callback);
        }
    }
}
