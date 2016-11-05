using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YSL.Common.Extender.ExecptionExtender;
namespace YSL.Common.Assert
{
    /// <summary>
    /// 断言异常
    /// </summary>
    [Serializable]
    public class AssertException : AppException
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public AssertException()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">消息</param>
        public AssertException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常</param>
        public AssertException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    /// <summary>
    /// 业务规则异常
    /// </summary>
    [Serializable]
    public class BizRuleException : AppException
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public BizRuleException()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">消息</param>
        public BizRuleException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常</param>
        public BizRuleException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
