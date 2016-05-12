using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CP.Model;

namespace CP.AdminWeb.Models
{
    [Serializable]
    public class UserListViewModel 
    {
        public IEnumerable<UserInfo> UserList { get; set; }
    }
}