using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSL.DataEntity.ApiModel
{
    /// <summary>
    /// api响应结果
    /// </summary>
    public class Result
    {
        [Display(Name = "状态")]
        public bool Status { get; set; }

        [Display(Name = "状态码")]
        public int Code { get; set; }

        [Display(Name = "状态描述")]
        public string Message { get; set; }

        [Display(Name = "结果数据")]

        public Object Data { get; set; }
    }
}
