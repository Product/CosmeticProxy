using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CP.Common
{
    public class QueryStringHelper
    {
        /// <summary>
        /// 转换成整型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int ParseToIntValue(string key)
        {
            int intValue = -1;
            HttpRequest request = HttpContext.Current.Request;
            if (!string.IsNullOrEmpty(request[key]))
            {
                try
                {
                    int.TryParse(request[key], out intValue);
                }
                catch (ArgumentException ex)
                {

                }
            }
            return intValue;
        }

        /// <summary>
        /// 转换成字符串类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ParseToStringValue(string key)
        {
            string stringValue = string.Empty;
            HttpRequest request = HttpContext.Current.Request;
            if (!string.IsNullOrEmpty(request[key]))
            {
                try
                {
                    stringValue = request[key];
                }
                catch (ArgumentException ex)
                {

                }
            }

            return stringValue;
        }
    }
}
