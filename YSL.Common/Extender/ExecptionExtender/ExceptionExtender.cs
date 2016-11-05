using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Assert;

namespace YSL.Common.Extender
{
    /// <summary>
    /// 异常拓展类
    /// </summary>
    public static class ExceptionExtender
    {
        /// <summary>
        /// 获得最内层异常
        /// </summary>
        /// <param name="exception">异常</param>
        /// <returns>最内层异常</returns>
        public static Exception MostInnerException(this Exception exception)
        {
            Exception tmp = exception;
            while (tmp.InnerException != null && !(tmp is AssertException))
            {
                tmp = tmp.InnerException;
            }
            return tmp;
        }

        /// <summary>
        /// 格式错误
        /// </summary>
        /// <param name="exception">异常</param>
        /// <returns>最底层的异常</returns>
        public static string Format(this Exception exception)
        {
            var inner = exception.MostInnerException();
            return inner.Message;
        }
    }
}
