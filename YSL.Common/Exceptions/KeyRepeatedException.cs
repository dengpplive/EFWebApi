namespace YSL.Common.Exceptions
{
    /// <summary>
    /// 键值重复异常
    /// </summary>
    public class KeyRepeatedException : CustomException {
        public KeyRepeatedException()
            : this("已存在具有相同键的元素") {
        }
        public KeyRepeatedException(string message)
            : base(message) {
        }
    }
}