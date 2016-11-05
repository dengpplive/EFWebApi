using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSL.DataEntity.ApiModel.Request
{    
    public class UserView
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "用户名")]
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [Display(Name = "用户密码")]
        public string Password { get; set; }
    }
}
