using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CP.DAL;
using CP.Model;

namespace CP.BLL
{
    public class UserBLL
    {
        private readonly UserDAL userDAL = new UserDAL();

        /// <summary>
        /// 添加用户(在用户插入memership表之后调用)
        /// </summary>
        /// <param name="userInfo">用户对象</param>
        /// <returns>true成功，false失败</returns>
        public int AddUser(UserInfo userInfo)
        {
            return userDAL.AddUser(userInfo);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns>true成功，false失败</returns>
        public bool DeleteUser(int userId)
        {
            return userDAL.DeleteUser(userId);
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUser(int userId)
        {
            return userDAL.GetUser(userId);
        }



        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUser(int userId,string userName)
        {
            return userDAL.GetUser(0,userName);
        }


        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> GetUserList()
        {
            return userDAL.GetUserList();
        }


    }
}
