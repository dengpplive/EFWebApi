namespace YSL.Common.Exceptions
{
    /// <summary>
    /// 无效值异常
    /// </summary>
    [System.Serializable]
    public class InvalidValueException : CustomException {
        public InvalidValueException() : this("设置的值无效") { }
        public InvalidValueException(string message) : base(message) { }
    }
}