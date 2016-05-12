using System;
using System.Web.Security;
using CP.BLL;
using CP.Model;

namespace CP.Service
{   
    class CachedUser
    {
        public DateTime Created;
        public UserInfo User;
    }

    public class UserService 
    {
        private UserInfo _userInfo;
        static string applicationnName = AdminContext.ApplicationnName;
        static UserService _instance;
        static object lock_obj = new object();
        private readonly UserBLL userBll = new UserBLL();

        public static UserService Create()
        {
            lock (lock_obj)
            {
                if (_instance == null)
                {
                    _instance = new UserService();
                }
            }
            return _instance;
        }

        #region 获取用户

        /// <summary>
        /// 填充用户的UserName
        /// </summary>
        /// <param name="user"></param>
        public void GetUserName(UserInfo user)
        {
            if (user == null) return;
            MembershipUser mUser = Membership.GetUser(user.MembershipId);
            if (mUser == null) return;
            user.UserName = mUser.UserName;
            user.Email = mUser.Email;
        }


        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUser(string userName)
        {
            UserInfo user = userBll.GetUser(0,GetLoggedOnUsername());
            GetUserName(user);
            return user;
        }

        /// <summary>
        /// 根据userId获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserInfo GetUserById(int userId)
        {
            UserInfo user = userBll.GetUser(userId);
            GetUserName(user);
            return user;
        }




        #endregion

        #region 当前登录用户名
        /// <summary>
        /// 当前登录用户名
        /// </summary>
        /// <returns></returns>
        public string GetLoggedOnUsername()
        {
            AdminContext context = AdminContext.Current;

            if (!context.IsWebRequest || context.Context.User == null || context.Context.User.Identity.Name == string.Empty)
                return "Anonymous";

            return context.UserName;
        }
        #endregion

        #region 创建用户

        public MembershipCreateStatus Create(UserInfo user, bool sendEmail)
        {
            return Create(user, sendEmail, false, false);
        }

        private MembershipUser MembershipCreateUser(UserInfo user, out MembershipCreateStatus status)
        {
            if (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.PassWord) && !string.IsNullOrEmpty(user.Email))
            {
                return Membership.CreateUser(user.UserName, user.PassWord, user.Email, null, null, true, out status);

            }
            status = MembershipCreateStatus.InvalidPassword;
            return null;
        }

        public MembershipCreateStatus Create(UserInfo user, bool sendEmail, bool ignoreDisallowNames, bool isServiceMan)
        {
            //当前上下文
            AdminContext csContext = AdminContext.Current;
            MembershipCreateStatus status = MembershipCreateStatus.InvalidUserName;
            string password = user.PassWord;

            user.Member = MembershipCreateUser(user, out status);
            //创建用户             
            if (user.Member != null && status == MembershipCreateStatus.Success)
            {
                int userId = userBll.AddUser(user);
                if (userId > 0)
                {
                    user.UserId = userId;
                    csContext.User = user;
                    status = MembershipCreateStatus.Success;
                }
            }

            return status;
        }

        #endregion

        #region 更新用户

        public bool MembershipUserSave(UserInfo user)
        {
            if (user.Member != null)
            {
                Membership.UpdateUser(user.Member);
                return true;
            }
            return false;
        }
        #endregion

        #region 验证用户
        /// <summary>
        /// 验证用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int ValidUser(UserInfo user)
        {
            if (!Membership.ValidateUser(user.UserName, user.PassWord))
            {
                return 0;
            }
            UserInfo userInfo = userBll.GetUser(0,user.UserName);
            if (userInfo == null)
                return 0;
            AdminContext.SetCurrentCookie(userInfo, true);
            return 1;
        }
        #endregion

        #region 注销用户
        /// <summary>
        /// 注销用户
        /// </summary>
        /// <returns></returns>
        public void LoginOut()
        {
            AdminContext.LoginOut();
        }
        #endregion

        #region 删除用户

        public bool DeleteUser(int UserID)
        {
            return userBll.DeleteUser(UserID);
        }

        #endregion 删除用户

        #region 将用户密码重置为一个自动生成的新密码
        /// <summary>
        /// 将用户密码重置为一个自动生成的新密码
        /// </summary>
        /// <returns></returns>
        public string ResetPassword(int userId)
        {
            string pass = GetUserById(userId).Member.ResetPassword();
            return pass;
        }

        #endregion


        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
