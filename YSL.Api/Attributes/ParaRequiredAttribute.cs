using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using YSL.Api.ModelClientRule;
using YSL.Common.Resources;
namespace YSL.Api.Attributes
{
    /// <summary>
    /// 服务端Model非空验证
    /// Author:AxOne
    /// </summary>
    public class ParaRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        public ParaRequiredAttribute()
        {
            ErrorMessage = Constant.RequiredTip;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRequriedToRule(FormatErrorMessage(metadata.GetDisplayName()));
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

    }
}
