using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace HuiEF.Web.MyFilters
{
    /// <summary>
    /// 登录验证过滤器
    /// 在 FilterConfig.cs文件中添加filters.Add(new Filter.LoginValidateAttribute());
    /// </summary>
    public class CheckPermissionAttribute : System.Web.Mvc.AuthorizeAttribute
    {

        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {


            //跳过权限验证
            //检查 被请求的 方法 和 控制器是否有 AllowAnonymous 标签，如果有，则不验证；如果没有，则验证
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false) &&
                !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false))
            {

                //获取当前请求的 区域 
                string strAreaName = null;
                if (filterContext.RouteData.DataTokens.Keys.Contains("area"))
                {
                    strAreaName = filterContext.RouteData.DataTokens["area"].ToString().ToLower();
                }
                if (!string.IsNullOrEmpty(strAreaName)) { return; }
                string strContrllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
                string strActionName = filterContext.ActionDescriptor.ActionName.ToLower();

                short HttpMethod = filterContext.HttpContext.Request.HttpMethod.ToLower() == "get" ? (short)0 : (short)1;
                if (IsLogin())
                {
                    return;
                }
                else
                {
                    filterContext.Result = new RedirectResult("/User/Login");
                }


            }

        }

        /// <summary>
        /// 检查用户是否登录
        /// </summary>
        /// <returns></returns>
        private bool IsLogin()
        {
            var httpContext = HttpContext.Current;
            //检查session
            var sessionUser = httpContext.Session[Common.SessionKey.SK_ADMIN_USER_INFO];
            if (sessionUser != null) { return true; }
            else { return false; }
            ////检查cookie
            //var cookieUserNative = httpContext.Request.Cookies[Common.SessionKey.SK_ADMIN_USER_INFO];
            //if (cookieUserNative == null) { return false; }//没有cookie

            //string cookieUser = Common.Encrypt.AESECBEncrypt.Decrypt(cookieUserNative.Value);
            ////使用cookie登录
            //if (!string.IsNullOrEmpty(cookieUser) && cookieUser.Contains("|"))
            //{
            //    Service.Member memberService = new Service.Member();
            //    var userinfo = memberService.Login(cookieUser.Split('|')[0], cookieUser.Split('|')[1]);
            //    if (userinfo == null) { return false; }
            //    else { return true; }
            //}
            //else { return false; }
        }
    }
}