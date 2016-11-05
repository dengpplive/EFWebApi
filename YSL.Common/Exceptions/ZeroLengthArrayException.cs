namespace YSL.Common.Exceptions
{
    /// <summary>
    /// 对长度为零的数组进行操作时引发的异常
    /// </summary>
    public class ZeroLengthArrayException : CustomException {
        public ZeroLengthArrayException() : base("数组长度为零") { }
        public ZeroLengthArrayException(string name) : base(string.Format("数组 \"{0}\" 长度为零", name)) { }
    }
}
