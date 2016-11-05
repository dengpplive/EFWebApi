using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Exceptions
{
    /// <summary>
    /// 自定义异常
    /// </summary>
    [Serializable]
    public class CustomException : System.Exception
    {
        public CustomException() { }
        public CustomException(int code) { ErrorCode = code; }
        public CustomException(string message) : base(message) { }
        public CustomException(int code, string message) : base(message) { ErrorCode = code; }
        public CustomException(string message, System.Exception inner) : base(message, inner) { }
        public CustomException(int code, string message, System.Exception inner) : base(message, inner) { ErrorCode = code; }
        public CustomException(int code, System.Exception inner) : this(code, inner.Message, inner) { }
        public virtual int ErrorCode { get; set; }
    }
}
