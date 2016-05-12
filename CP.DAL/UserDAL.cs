using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Gyyx.Core.DBUtility;
using CP.Model;
using System.Data;
using System.Web.Security;


namespace CP.DAL
{
    public class UserDAL
    {
        #region SQL语句

        private const string AddUserSql = @"insert into TUser ([membershipId],[truename])
                                                    values (@membershipId,@truename);SELECT @@IDENTITY";

        private const string DeleteUserSql = @"delete from [TUser] where UserId=@userId;select @@ROWCOUNT;";

        private const string GetUserSql = @"select * from TUser where userid = @userId";

        private const string GetUserListSql = @"select * from TUser";



        #endregion

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userInfo">用户对象</param>
        /// <returns>true成功，false失败</returns>
        public int AddUser(UserInfo userInfo)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxyAdmin");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@membershipId",SqlDbType.UniqueIdentifier),
                                     new SqlParameter("@truename",SqlDbType.VarChar)
                                     };
            parameters[0].Value = userInfo.Member.ProviderUserKey;
            parameters[1].Value = userInfo.TrueName;
            object obj = dbo.ExecuteScalar(CommandType.Text, AddUserSql, parameters);
            if (obj != null)
                return Convert.ToInt32(obj.ToString());
            return 0;

        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns>true成功，false失败</returns>
        public bool DeleteUser(int userId)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxyAdmin");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@userId",SqlDbType.Int)
                                     };
            parameters[0].Value = userId;

            return dbo.ExecuteNonQuery(CommandType.Text, DeleteUserSql, parameters) == 1;
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUser(int userId)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxyAdmin");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@userId",SqlDbType.Int)
                                     };
            parameters[0].Value = userId;

            DataRow row = dbo.GetDataRow(CommandType.Text, GetUserSql, parameters);
            UserInfo user = null;

            if (row != null && row["userid"] != DBNull.Value)
            {
                user = new UserInfo
                {
                    UserId = Convert.ToInt32(row["userid"]),
                    TrueName = row["truename"].ToString(),
                    MembershipId = Guid.Parse(row["membershipId"].ToString()),
                };
            }
            return user;
        }


        /// <summary>
        /// <!--通过userId或者userName获取用户-->
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserInfo GetUser(int userId, string userName)
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxyAdmin");

            SqlParameter[] parameters = { 
                                     new SqlParameter("@userId",SqlDbType.Int),
                                     new SqlParameter("@userName",SqlDbType.VarChar)
                                     };
            parameters[0].Value = userId;
            parameters[1].Value = userName;

            DataRow row = dbo.GetDataRow(CommandType.StoredProcedure, "TUser_GetUser", parameters);
            UserInfo user = null;

            if (row != null && row["userid"] != DBNull.Value)
            {
                user = new UserInfo
                {
                    UserId = Convert.ToInt32(row["userid"]),
                    TrueName = row["truename"].ToString(),
                    MembershipId = Guid.Parse(row["membershipId"].ToString())
                };
            }
            return user;
        }


        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public List<UserInfo> GetUserList()
        {
            IDataHelper dbo = DataHelperFactory.Create("CosmeticProxyAdmin");

            DataTable datable = dbo.GetDataTable(CommandType.Text, GetUserListSql);

            if (datable == null || datable.Rows.Count <= 0)
            {
                return new List<UserInfo>();
            }

            List<UserInfo> userList = new List<UserInfo>();
            userList.AddRange(from DataRow row in datable.Rows
                              select new UserInfo
                                       {
                                           UserId = Convert.ToInt32(row["userid"]),
                                           MembershipId = Guid.Parse(row["membershipId"].ToString())
                                       });

            return userList;
        }


        /// <summary>
        /// 初始化MembershipUser信息
        /// </summary>
        /// <param name="user"></param>
        public void Init(UserInfo user)
        {
            if (user != null)
                user.RefreshMembershipUser(Membership.GetUser(user.MembershipId, false));
        }
    }
}
