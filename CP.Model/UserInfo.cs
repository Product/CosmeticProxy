using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;


namespace CP.Model
{
    [Serializable]
    public class UserInfo
    {
        public void RefreshMembershipUser(MembershipUser mu)
        {
            if (mu == null)
            {
                throw new Exception("A null MembershipUser is not valid to instantiate a new User");
            }

            this.memberUser = mu;
            this._userName = mu.UserName;
            this._email = mu.Email;
            this._membershipId = (Guid)mu.ProviderUserKey;
        }


        /// 成员管理Id
        private Guid _membershipId;

        ///<summary>
        ///  成员管理Id
        ///</summary>
        public Guid MembershipId
        {
            get { return _membershipId; }
            set { _membershipId = value; }
        }

        /// 用户Id
        private int _userId;

        ///<summary>
        ///  用户Id
        ///</summary>
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        /// 真实姓名
        private string _trueName;

        ///<summary>
        ///  真实姓名
        ///</summary>
        public string TrueName
        {
            get { return _trueName; }
            set { _trueName = value; }
        }

        /// 用户名称
        private string _userName;

        ///<summary>
        ///  用户名称
        ///</summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public int GroupId { get; set; }

        /// 用户密码
        private string _password;

        ///<summary>
        ///  用户密码
        ///</summary>
        public string PassWord
        {
            get { return _password; }
            set { _password = value; }
        }

        /// 电子邮件
        private string _email;

        ///<summary>
        ///  电子邮件
        ///</summary>
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                if (HasMember)
                {
                    Member.Email = _email;
                }
            }
        }


        public bool HasMember
        {
            get { return Member != null; }
        }


        private MembershipUser memberUser;

        public MembershipUser Member
        {
            get
            {
                if (memberUser == null && string.IsNullOrEmpty(this.UserName))
                {
                    RefreshMembershipUser(Membership.GetUser(this.UserName));
                }
                return memberUser;
            }
            set { memberUser = value; }
        }
    }
}
