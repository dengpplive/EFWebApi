using System.Web.Mvc;

namespace YSL.Api.ModelClientRule
{
    /// <summary>
    /// 服务端Model非空验证规则    
    /// </summary>
    public class ModelClientValidationRequriedToRule : ModelClientValidationRule
    {
        public ModelClientValidationRequriedToRule(string errorMessage)
        {
            ValidationType = "required";
            ErrorMessage = errorMessage;
        }
    }
}
