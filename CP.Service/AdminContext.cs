using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using CP.Model;

namespace CP.Service
{

    public sealed class AdminContext
    {

        #region Private Containers
        private static AuthenticationSection authenticationSection;
        private HybridDictionary _items = new HybridDictionary();
        private NameValueCollection _queryString = null;
        private string _siteUrl = null, _rewrittenUrlName = null;
        private Uri _currentUri;

        string rolesCacheKey = null;

        string authenticationType = "forms";

        bool _isUrlReWritten = false;
        bool _isEmpty = false;
        string _rawUrl;

        HttpContext _httpContext = null;
        DateTime requestStartTime = DateTime.Now;
        UserInfo anonymousUser;
        #endregion

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize(NameValueCollection qs, Uri uri, string rawUrl, string siteUrl)
        {
            authenticationSection = (AuthenticationSection)System.Configuration.ConfigurationManager.GetSection("system.web/authentication");
            _queryString = qs;
            _siteUrl = siteUrl;
            _currentUri = uri;
            _rawUrl = rawUrl;

            if (_httpContext == null || _httpContext.User == null || !_httpContext.User.Identity.IsAuthenticated || !int.TryParse(_httpContext.User.Identity.Name, out _currentUserId))
            {
                _currentUserId = -1;
            }
            anonymousUser = new UserInfo();
            anonymousUser.UserId = -1;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private AdminContext(Uri uri, string siteUrl)
        {
            Initialize(new NameValueCollection(), uri, uri.ToString(), siteUrl);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="includeQS">是否初始化HTTP的QueryString</param>
        private AdminContext(HttpContext context, bool includeQS)
        {
            this._httpContext = context;

            if (includeQS)
            {
                Initialize(new NameValueCollection(context.Request.QueryString), context.Request.Url, context.Request.RawUrl, null);
            }
            else
            {
                Initialize(null, context.Request.Url, context.Request.RawUrl, null);
            }
        }

        #endregion

        #region Create

        /// <summary>
        /// 创建CSContext空类
        /// </summary>
        /// <returns></returns>
        public static AdminContext CreateEmptyContext()
        {
            AdminContext csContext = new AdminContext(new Uri("http://CreateEmptyContext"), "http://CreateEmptyContext");
            csContext._isEmpty = true;
            SaveContextToStore(csContext);
            return csContext;
        }

        /// <summary>
        /// 创建CSContext空类
        /// </summary>
        /// <param name="settingsID">站点ID</param>
        /// <returns></returns>
        public static AdminContext Create()
        {
            AdminContext csContext = new AdminContext(new Uri("http://CreateContextBySettingsID"), "http://CreateContextBySettingsID");
            SaveContextToStore(csContext);
            return csContext;
        }


        /// <summary>
        /// 创建当前HTTP信息的CSContext类
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns></returns>
        public static AdminContext Create(HttpContext context)
        {
            return Create(context, false);
        }

        /// <summary>
        ///创建CSContext
        /// </summary>
        public static AdminContext Create(HttpContext context, bool isReWritten)
        {
            AdminContext csContext = new AdminContext(context, true);
            SaveContextToStore(csContext);
            return csContext;
        }


        /// <summary>
        /// 创建CSContext
        /// </summary>
        public static AdminContext Create(Uri uri, string appName)
        {
            AdminContext csContext = new AdminContext(uri, appName);
            SaveContextToStore(csContext);
            return csContext;
        }
        #endregion

        #region Core Properties

        public IDictionary Items
        {
            get { return _items; }
        }

        public object this[string key]
        {
            get
            {
                if (this.IsWebRequest)
                {
                    return this._items[key];
                }
                return null;
            }
            set
            {
                if (this.IsWebRequest)
                {
                    this._items[key] = value;
                }
            }
        }

        /// <summary>
        /// 保存HTTP查询字符串变量集合
        /// </summary>
        public NameValueCollection QueryString
        {
            get { return _queryString; }
        }

        /// <summary>
        /// HttpContext 是否为空
        /// </summary>
        public bool IsWebRequest
        {
            get { return this.Context != null; }
        }
        /// <summary>
        /// 验证类型
        /// </summary>
        public string AuthenticationType
        {
            get { return authenticationType; }
            set { authenticationType = value.ToLower(); }
        }

        public string RewrittenUrlName
        {
            get { return _rewrittenUrlName; }
            set { _rewrittenUrlName = value; }
        }

        public HttpContext Context
        {
            get
            {
                return _httpContext;
            }
        }



        #endregion


        #region Status Properties

        public bool IsUrlReWritten { get { return _isUrlReWritten; } set { _isUrlReWritten = value; } }

        #endregion


        #region  用户信息

        private UserInfo _currentUser = null;
        private int _currentUserId;
        /// <summary>
        /// 获取当前用户
        /// </summary>
        public UserInfo User
        {
            get
            {
                if (_currentUser == null && this.IsWebRequest && _currentUserId > 0)
                {
                    _currentUser = UserService.Create().GetUserById(_currentUserId);
                }
                if (_currentUser == null)
                {
                    _currentUser = anonymousUser;
                }
                return _currentUser;
            }
            internal set { _currentUser = value; }
        }

        public int CurrentUserId
        {
            get
            {
                return _currentUserId;
            }
            set
            {
                _currentUserId = value;
            }
        }
        /// <summary>
        /// 是否登陆
        /// </summary>
        public bool IsLogin
        {
            get
            {
                return User != null && User.UserId > 0;
            }
        }
        #endregion

        #region helper

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public static string ApplicationnName
        {
            get
            {
                return Membership.ApplicationName;
            }
        }

        /// <summary>
        /// 系统相对根路径
        /// </summary>
        public static string ApplicationPath
        {
            get
            {
                string applicationPath = "/";

                if (HttpContext.Current != null)
                    applicationPath = HttpContext.Current.Request.ApplicationPath;
                if (applicationPath == "/")
                {
                    return string.Empty;
                }
                else
                {
                    return applicationPath;
                }
            }
        }

        public Guid GetGuidFromQueryString(string key)
        {
            Guid returnValue = Guid.Empty;
            string queryStringValue;

            queryStringValue = QueryString[key];

            if (queryStringValue == null)
                return returnValue;
            try
            {
                if (queryStringValue.IndexOf("#") > 0)
                    queryStringValue = queryStringValue.Substring(0, queryStringValue.IndexOf("#"));

                returnValue = new Guid(queryStringValue);
            }
            catch { }

            return returnValue;
        }


        public int GetIntFromQueryString(string key, int defaultValue)
        {
            string queryStringValue;

            queryStringValue = this.QueryString[key];

            if (queryStringValue == null)
                return defaultValue;

            try
            {
                if (queryStringValue.IndexOf("#") > 0)
                    queryStringValue = queryStringValue.Substring(0, queryStringValue.IndexOf("#"));

                defaultValue = Convert.ToInt32(queryStringValue);
            }
            catch { }

            return defaultValue;

        }
        #endregion

        #region Common QueryString Properties


        string queryText = null;
        string returnUrl = null;
        string url = null;

        int userID = -2;
        string userName = null;
        Guid roleID = Guid.Empty;



        public int UserID
        {
            get
            {
                if (userID == -2)
                    userID = this.GetIntFromQueryString("UserID", -1);

                return userID;
            }
            set { userID = value; }
        }

        public string UserName
        {
            get
            {
                if (userName == null)
                {
                    userName = QueryString["UserName"];
                }

                return userName;
            }
            set
            {
                userName = value;
            }
        }

        /// <summary>
        /// 查询字符串中returnUrl变量（返回URL）
        /// </summary>
        public string ReturnUrl
        {
            get
            {
                if (returnUrl == null)
                    returnUrl = QueryString["returnUrl"];

                return returnUrl;
            }
            set { returnUrl = value; }
        }

        /// <summary>
        /// 查询字符串中url变量
        /// </summary>
        public string Url
        {
            get
            {
                if (url == null)
                    url = QueryString["url"];

                return url;
            }
            set { url = value; }
        }


        #endregion

        #region State

        private static readonly string dataKey = "MAP-AdminContext";
        [ThreadStatic]
        private static AdminContext currentContext = null;

        /// <summary>
        /// 获取HTTP请求对应的CSContext信息
        /// </summary>
        public static AdminContext Current
        {
            get
            {
                HttpContext httpContext = HttpContext.Current;
                AdminContext context = null;
                if (httpContext != null)
                {
                    if (httpContext.Items.Contains(dataKey))
                    {
                        context = httpContext.Items[dataKey] as AdminContext;
                    }
                    else
                    {
                        context = new AdminContext(httpContext, true);
                        SaveContextToStore(context);
                    }
                }
                return context;

            }
        }

        private static void SaveContextToStore(AdminContext context)
        {
            if (context.IsWebRequest)
            {
                context.Context.Items[dataKey] = context;
            }
            else
            {
                currentContext = context;
            }
        }

        #endregion

        #region 用户登录


        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="user"></param>
        /// <param name="createPersistentCookie"></param>
        public static void SetCurrentCookie(UserInfo user, bool createPersistentCookie)
        {
            if (user == null || user.UserId < 1)
                return;
            AdminContext csContext = AdminContext.Current;
            csContext.User = user;
            csContext.CurrentUserId = user.UserId;

            HttpCookie cookie = GetAuthCookie(user.UserId, createPersistentCookie, null);
            HttpContext.Current.Response.Cookies.Remove(cookie.Name);
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.Request.Cookies.Remove(cookie.Name);
            HttpContext.Current.Request.Cookies.Add(cookie);
        }

        private static string GetReturnUrl()
        {

            string str = HttpContext.Current.Request.QueryString["ReturnUrl"];
            if (str == null)
            {
                str = HttpContext.Current.Request.Form["ReturnUrl"];
                if ((!string.IsNullOrEmpty(str) && !str.Contains("/")) && str.Contains("%"))
                {
                    str = HttpUtility.UrlDecode(str);
                }
            }

            if (string.IsNullOrEmpty(str))
            {

                str = string.Empty;
            }
            return str;
        }
        public static HttpCookie GetAuthCookie(int UserID, bool createPersistentCookie, string cookieDomain)
        {
            string strCookiePath = FormsAuthentication.FormsCookiePath;
            string cookiedata = string.Empty;

            DateTime timeout;
            if (createPersistentCookie)
            {
                timeout = DateTime.Now.AddDays(30);
            }
            else
            {
                timeout = DateTime.Now.AddMinutes((double)authenticationSection.Forms.Timeout.TotalMinutes);
            }
            if (string.IsNullOrEmpty(cookieDomain))
            {
                cookieDomain = authenticationSection.Forms.Domain;
            }


            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, UserID.ToString(), DateTime.Now, timeout, createPersistentCookie, cookiedata, strCookiePath);

            string encTicket = FormsAuthentication.Encrypt(ticket);

            if (string.IsNullOrEmpty(encTicket))
            {
                throw new HttpException("Unable_to_encrypt_cookie_ticket");
            }
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            string domain = "gyyx.cn";

            if (!string.IsNullOrEmpty(cookieDomain))
            {
                cookie.Domain = cookieDomain;
            }
            bool urlIsDomain = HttpContext.Current.Request.Url.Host.ToString().EndsWith(domain);
            if (urlIsDomain)
            {
                cookie.Domain = domain;
            }
            cookie.HttpOnly = true;
            cookie.Path = strCookiePath;
            cookie.Secure = FormsAuthentication.RequireSSL;
            if (createPersistentCookie)
            {
                cookie.Expires = ticket.Expiration;
            }
            return cookie;
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        public static void LoginOut()
        {
            string domain = "gyyx.cn";
            bool urlIsDomain;

            #region 注销登录
            FormsAuthentication.SignOut();
            HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
            {
                cookie = new HttpCookie(FormsAuthentication.FormsCookieName);
            }

            cookie.Path = "/";
            urlIsDomain = HttpContext.Current.Request.Url.Host.ToString().EndsWith(domain);
            if (urlIsDomain)
            {
                cookie.Domain = domain;
            }
            cookie.Value = "";
            cookie.Expires = DateTime.Now.AddYears(-10);

            HttpContext.Current.Response.Cookies.Remove(cookie.Name);
            HttpContext.Current.Response.Cookies.Add(cookie);
            HttpContext.Current.Request.Cookies.Remove(cookie.Name);
            HttpContext.Current.Request.Cookies.Add(cookie);

            #endregion
        }

        #endregion
    }
}
