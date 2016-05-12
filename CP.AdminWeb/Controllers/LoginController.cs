using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CP.Model;
using CP.Service;

namespace CP.AdminWeb.Controllers
{
    public class LoginController : Controller
    {
        /// <summary>
        /// 用户登录页码
        /// </summary>
        public ActionResult Login()
        {
            return View();
        }
        /// <summary>
        /// 登陆AJAX调用
        /// </summary>
        [HttpPost]
        public void LoginService(String userName, String userPassword)
        {
            String message;
            UserLogin(userName, userPassword, out message);
            Response.Write(message);
        }


        /// <summary>
        /// 注销AJAX调用
        /// </summary>
        [HttpPost]
        public void LoginOut()
        {
            UserService.Create().LoginOut();
            Response.Write("LogoutSuccess");
        }


        #region 用户登录
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userPassword">密码</param>
        /// <param name="message"></param>
        /// <returns></returns>
        private Boolean UserLogin(String userName, String userPassword, out String message)
        {
            Boolean success = false;
            message = String.Empty;
            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(userPassword))
            {
                UserInfo user = new UserInfo()
                {
                    UserName = userName,
                    PassWord = userPassword
                };
                if (UserService.Create().ValidUser(user) > 0)
                {
                    message = "LoginSuccess";
                    success = true;
                }
                else
                {
                    message = "LoginError";
                }
            }
            else
            {
                message = "DataFormatError";
            }
            return success;
        }
        #endregion
    }
}
