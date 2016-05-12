using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CP.AdminWeb.Models
{
    public class LogOnViewModel
    {
        [Required]
        [Display(Name = "账号")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "密码"), DataType(DataType.Password)]
        public string UserPwd { get; set; }
    }
}
