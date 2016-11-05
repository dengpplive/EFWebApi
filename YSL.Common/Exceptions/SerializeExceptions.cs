using System;
using System.Runtime.Serialization;

namespace YSL.Common.Exceptions
{
    public abstract class SerializeException {
    }

    /// <summary>
    /// 反序列化时遇到无效格式的字符串时引发的异常。
    /// </summary>
    [Serializable]
    public class InvalidStringFormatException : System.Exception {
        /// <summary>
        /// 初始化 InvalidStringFormatException 类型的新实例。
        /// </summary>
        public InvalidStringFormatException():base("字符串格式无效。") {}

        /// <summary>
        /// 用指定的描述信息，初始化 InvalidStringFormatException 类型的新实例。
        /// </summary>
        /// <param name="message">描述信息。</param>
        public InvalidStringFormatException(string message) : base(message) {}

        /// <summary>
        /// 用指定的描述信息和内部异常，初始化 InvalidStringFormatException 类型的新实例。
        /// </summary>
        /// <param name="message">描述信息。</param>
        /// <param name="inner">内部异常。</param>
        public InvalidStringFormatException(string message, System.Exception inner) : base(message, inner) { }

        protected InvalidStringFormatException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
    }
}
