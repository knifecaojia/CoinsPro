using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using System.Diagnostics;
using CZLogger;
using System.Globalization;
using Domain;
using System.Threading;
namespace Web.helper
{
    /// <summary>
    ///　权限拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeFilterAttribute : ActionFilterAttribute
    {

        private readonly string Key = "_thisOnActionMonitorLog_";
        // OnActionExecuted 在执行操作方法后由 ASP.NET MVC 框架调用。
        // OnActionExecuting 在执行操作方法之前由 ASP.NET MVC 框架调用。
        // OnResultExecuted 在执行操作结果后由 ASP.NET MVC 框架调用。
        // OnResultExecuting 在执行操作结果之前由 ASP.NET MVC 框架调用。

        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 记录日志(所有的请求)
            MonitorLog MonLog = new MonitorLog();
            MonLog.ExecuteStartTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
            MonLog.ControllerName = filterContext.RouteData.Values["controller"] as string;
            MonLog.ActionName = filterContext.RouteData.Values["action"] as string;
            MonLog.FormCollections = filterContext.HttpContext.Request.Form;//form表单提交的数据
            MonLog.QueryCollections = filterContext.HttpContext.Request.QueryString;//Url 参数
            //Logger.Info(MonLog.GetLoginfo());
            filterContext.Controller.ViewData[Key] = MonLog;
            #endregion

            //忽略home的权限验证
            var controllerName = filterContext.RouteData.Values["controller"].ToString().ToLower();
            if (controllerName == "home") return;



            var actionName = filterContext.RouteData.Values["action"].ToString().ToLower();

            //忽略首页index的权限验证
            //if (actionName == "index") return;


            var actionType = ActionType(actionName);

            DAO.BLL.B_Navigation b_nav = new DAO.BLL.B_Navigation();
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="controllerName",value=controllerName,searchType=Common.EnumBase.SearchType.Eq}
            };
            IList<Domain.Navigation> list_nav = b_nav.GetList(st, null);
            if (list_nav.Count == 0)
            {
                filterContext.Result = new ContentResult { Content = @"抱歉,没有找到该操作！" };
                return;
            }
            DAO.BLL.B_Manager_role_value b_mrv = new DAO.BLL.B_Manager_role_value();
            DAO.BLL.B_Manager b_manager = new DAO.BLL.B_Manager();
            var m_manager = b_manager.Get(Convert.ToInt32(filterContext.HttpContext.User.Identity.Name));
            st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="role_id",value=m_manager.manager_role.id,searchType=Common.EnumBase.SearchType.Eq},
                new SearchTemplate(){key="nav_id",value=list_nav[0].id,searchType=Common.EnumBase.SearchType.Eq}
            };
            var list_mrv = b_mrv.GetList(st, null);
            if (list_mrv.Count == 0)
            {
                filterContext.Result = new ContentResult { Content = @"抱歉,你不具有当前操作的权限！" };
                return;
            }
            if (!list_mrv[0].action_type.Contains(actionType))
            {
                //如果是查看,就返回一个空的视图,否则返回一个json
                if (actionType == EnumBase.Authorize.查看.Description())
                {
                    filterContext.Result = new ContentResult { Content = @"抱歉,你不具有当前操作的权限！" };// 直接返回 return Content("抱歉,你不具有当前操作的权限！")   
                }
                else
                {
                    Common.Json json = new Common.Json();
                    json.msg = "抱歉,你不具有当前操作的权限！";
                    json.status = -1;
                    filterContext.Result = new JsonResult() { Data = json };
                }
            }
        }

        private string ActionType(string actionName)
        {
            //根据当前操作名称的前缀,来判断用户的请求

            // Get      EnumBase.Authorize.查看.Description()
            // Edit     EnumBase.Authorize.修改.Description()
            // Add      EnumBase.Authorize.添加.Description()
            // Del      EnumBase.Authorize.删除.Description()
            // Exam     EnumBase.Authorize.审核.Description()

            if (actionName.ToLower().IndexOf("get") == 0)
            {
                return EnumBase.Authorize.查看.Description();
            }
            else if (actionName.IndexOf("edit") == 0)
            {
                return EnumBase.Authorize.修改.Description();
            }
            else if (actionName.IndexOf("add") == 0)
            {
                return EnumBase.Authorize.添加.Description();
            }
            else if (actionName.IndexOf("del") == 0)
            {
                return EnumBase.Authorize.删除.Description();
            }
            else if (actionName.IndexOf("exam") == 0)
            {
                return EnumBase.Authorize.审核.Description();
            }
            else if (actionName.IndexOf("download") == 0)
            {
                return EnumBase.Authorize.下载.Description();
            }
            else
            {
                return EnumBase.Authorize.查看.Description();
            }
        }

        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        Thread th_log;

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            //如果记录结束时间,就可能疏漏掉一些被重置的请求
            try
            {
                var MonLog = filterContext.Controller.ViewData[Key] as MonitorLog;
                MonLog.ExecuteEndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff", DateTimeFormatInfo.InvariantInfo));
                Logger.Info(MonLog.GetLoginfo());
            }
            catch
            {

            }

            var actionName = filterContext.RouteData.Values["action"].ToString().ToLower();
            var controllerName = filterContext.RouteData.Values["controller"].ToString().ToLower();
            //如果相同,则是用户第一次打开view的请求,此时并没有数据,页面加载完后,会主动通过GetList请求数据,我们只用把第二次记录下来即可
            if (actionName != "index" && !string.IsNullOrEmpty(controllerName))
            {
                var thisIp = Utils.getIp();
                try
                {
                    DAO.BLL.B_Manager_log b_log = new DAO.BLL.B_Manager_log();
                    DAO.BLL.B_Manager b_manager = new DAO.BLL.B_Manager();
                    DAO.BLL.B_Navigation b_nav = new DAO.BLL.B_Navigation();
                    Domain.Manager_log model = new Domain.Manager_log();
                    model.user_id = Convert.ToInt32(filterContext.HttpContext.User.Identity.Name);
                    var m_manager = b_manager.Get(Convert.ToInt32(filterContext.HttpContext.User.Identity.Name));
                    model.user_name = m_manager.user_name;
                    model.action_type = ActionType(actionName);
                    List<SearchTemplate> st = new List<SearchTemplate>()
                            {
                                new SearchTemplate(){key="controllerName",value=controllerName,searchType=Common.EnumBase.SearchType.Eq}
                            };
                    model.navigation = b_nav.GetList(st, null)[0];
                    MonitorLog MonLog = new MonitorLog();
                    model.remark = MonLog.GetCollections(filterContext.HttpContext.Request.Form);
                    model.user_ip = thisIp;
                    model.add_time = DateTime.Now;
                    b_log.Save(model);
                }
                catch
                {

                }
            }

        }
    }
}
