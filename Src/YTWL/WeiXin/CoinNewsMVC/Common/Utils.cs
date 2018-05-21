using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class Utils
    {
        public static string getIp()
        {
            try
            {
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                    return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
                else
                    return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception err)
            {
                string s = err.Message;
                return System.Web.HttpContext.Current.Request.UserHostAddress;
            }
        }

        public static string getCode() 
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
