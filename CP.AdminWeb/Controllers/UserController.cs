using System.Text.RegularExpressions;
using System.Web.Security;
using System.Web.UI.WebControls;
using CP.AdminWeb.Models;
using CP.BLL;
using CP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CP.Service;
using Gyyx.Core.Security;

namespace CP.AdminWeb.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            UserBLL userbll = new UserBLL();
            List<UserInfo> userList = new List<UserInfo>();

            IList < UserInfo > list = userbll.GetUserList();
            foreach (UserInfo user in list)
            {
                UserService.Create().GetUserName(user);
            }

            UserListViewModel userListViewModel = new UserListViewModel()
            {
                UserList = list
            };
            return View(userListViewModel);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            ViewBag.Message = Request.RequestType;
            return View();
        }


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="password2"></param>
        /// <param name="email"></param>
        /// <param name="trueName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(String userName, String password, String password2, String email, String trueName)
        {
            UserInfo userInfo;
            String message;
            Boolean addSuccess = false;
            if (VerifyUserData(userName, password, password2, email, trueName, out userInfo))
            {
                AddUser(userInfo, out message);
            }
            else
            {
                message = "输入的内容中有无效数据!";
            }

            ViewBag.Message = message;

            return View();
        }





        /// <summary>
        /// 验证从form表单接收的用户新增用户的数据是否完善和正确
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="password2">确认密码</param>
        /// <param name="email">邮件地址</param>
        /// <param name="trueName">真实姓名</param>
        /// <param name="userInfo">抛出的用户信息对象</param>
        /// <returns>数据是否验证正确</returns>
        private Boolean VerifyUserData(String userName, String password, String password2, String email,
            String trueName, out UserInfo userInfo)
        {
            Regex emailRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            Boolean verifyDataSuccess = false;
            userInfo = null;
            if (!(String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password) ||
                String.IsNullOrEmpty(password2) || password.Trim() != password2.Trim() ||
                String.IsNullOrEmpty(email) || !emailRegex.IsMatch(email)))
            {
                verifyDataSuccess = true;
            }
            if (verifyDataSuccess)
            {
                userInfo = new UserInfo()
                {
                    UserName = userName,
                    PassWord = password,
                    Email = email,
                };
                if (!String.IsNullOrEmpty(trueName))
                {
                    userInfo.TrueName = trueName;
                }
            }
            return verifyDataSuccess;
        }


        #region 新增用户
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="userInfo">用户信息对象</param>
        /// <param name="errorMessage">当添加错误时抛出的错误信息</param>
        /// <returns>是否添加正确</returns>
        private Boolean AddUser(UserInfo userInfo, out String addUserMessage)
        {
            Boolean addSuccess = false;
            addUserMessage = String.Empty;
            MembershipCreateStatus addStatus = UserService.Create().Create(userInfo, true);
            switch (addStatus)
            {
                case MembershipCreateStatus.Success:
                    addUserMessage = "创建用户成功";
                    addSuccess = true;
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    addUserMessage = userInfo.UserName + "已存在，请选用他用户名创建用户!";
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    addUserMessage = "此邮箱地址已被使用过，请使用其他邮箱地址!";
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    addUserMessage = "密码的格式不正确，请重新设定密码!";
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    addUserMessage = "邮箱地址个是不正确，请重新输入邮箱地址!";
                    break;
                default:
                    addUserMessage = "添加用户过程中出现错误!";
                    break;
            }
            return addSuccess;
        }
        #endregion



        //
        // POST: /User/Delete/5

        [HttpGet]
        public ActionResult Delete(int userId)
        {
            UserBLL userbll = new UserBLL();
            UserInfo user = userbll.GetUser(userId);
            if (user != null)
            {
                userbll.DeleteUser(userId);
            }
            return RedirectToAction("Index");
        }
    }
}
