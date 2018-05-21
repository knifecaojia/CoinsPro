using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuiEF.Web.MyFilters
{
    /// <summary>
    /// 为所有的action设置viewdata
    /// </summary>
    public class ViewDataFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.HAAb = "测试OnActionExecuting";
            base.OnActionExecuting(filterContext);
        }
    }
}