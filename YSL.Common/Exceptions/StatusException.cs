namespace YSL.Common.Exceptions
{
    public class StatusException : CustomException {
        public StatusException(string model)
            : this(model, "状态错误") {
        }
        public StatusException(string model, string message)
            : base(message + " " + model) {
        }
    }
}
