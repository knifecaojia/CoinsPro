using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CZLogger
{
   public class LogTools
    {
        /// <summary>
        /// 初始化Log4Net配置,所有应用通用
        /// </summary>
        public static void InitLog4Net()
        {
            string filePath = System.Threading.Thread.GetDomain().BaseDirectory;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath + "\\log4net.config");
            log4net.Config.XmlConfigurator.Configure(fileInfo);
        }
        public static void LoggerLastError()
        {
            var ex = HttpContext.Current.Server.GetLastError();
            var AbsoluteUri="";
            if (HttpContext.Current.Request.UrlReferrer != null)
                AbsoluteUri = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
            Logger.Error(ex, string.Format("出错页面:{0}", HttpContext.Current.Request.Url.AbsoluteUri), "上一个页面:" + AbsoluteUri + "");
        }
    }
}
