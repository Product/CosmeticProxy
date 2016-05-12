using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CP.AdminWeb.Models;
using Gyyx.Core.Security;
using WebGrease.Css.Ast.Selectors;

namespace CP.AdminWeb.Controllers
{
    public class AdminController : Controller
    {
        public ViewResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                if (Authenticate(model))
                {
                    return Redirect(returnUrl ?? Url.Action("Index", "User"));
                }
                else
                {
                    ModelState.AddModelError("", "用户名密码错误");
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        private bool Authenticate(LogOnViewModel model)
        {
            if (model.UserName == "duping" && HashAlgorithm.MD5(model.UserPwd) == "b34ad1653e05449d02d9f62f2182ad8f")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
