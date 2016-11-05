namespace YSL.Common.Exceptions
{
    /// <summary>
    /// 内容项重复异常
    /// </summary>
    public class RepeatedItemException : CustomException {
        public RepeatedItemException()
            : this("已存在内容项") {
        }
        public RepeatedItemException(string message)
            : base(message) {
        }
        public RepeatedItemException(string item, string message)
            : this(message + " " + item) {
        }
    }
}