namespace YSL.Common.Exceptions
{
    public class NotFoundException : CustomException {
        public NotFoundException(string model)
            : this(model, "对象不存在") {
        }
        public NotFoundException(string model, string message)
            : base(message + " " + model) {
        }
    }
}
