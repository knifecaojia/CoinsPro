using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.helper
{
    /// <summary>
    /// 捕获程序出错日志
    /// </summary>
    public class Log4NetExceptionFilter : HandleErrorAttribute
    {

        public override void OnException(ExceptionContext filterContext)
        {
            string message = string.Format("消息类型：{0}<br>消息内容：{1}<br>引发异常的方法：{2}<br>引发异常源：{3}"
                  , filterContext.Exception.GetType().Name
                  , filterContext.Exception.Message
                   , filterContext.Exception.TargetSite
                   , filterContext.Exception.Source + filterContext.Exception.StackTrace
                   );

            //记录日志
            CZLogger.Logger.Error(message);

            //抛出异常信息
            filterContext.Controller.TempData["ExceptionAttributeMessages"] = message;

            //转向
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult("/Home/Error/");
        }
    }
}