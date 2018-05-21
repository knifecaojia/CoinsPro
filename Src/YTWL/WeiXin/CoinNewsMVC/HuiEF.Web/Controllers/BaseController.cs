using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuiEF.Web.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base

        #region Ajax请求 返回的消息
        /// <summary>
        /// 成功  
        /// </summary>
        /// <returns></returns>
        public ActionResult Success()
        {
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Success(object data)
        {
            return Json(new { success = true, data = data }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ActionResult Fail(string msg)
        {
            return Json(new { success = false, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 未登录
        /// </summary>
        /// <returns></returns>
        protected ActionResult NotLogin()
        {
            return Content(ToJson(new
            {
                success = false,
                code = 401,
                httpstatus = "Unauthorized",
                message = "未登录或登录已超时"
            }), "application/json");
        }

        /// <summary>
        /// 拒绝访问
        /// </summary>
        /// <returns></returns>
        protected ActionResult NoAccess()
        {
            return Content(ToJson(new
            {
                success = false,
                code = 410,
                httpstatus = "No Access",
                message = "拒绝访问"
            }), "application/json");
        }

        #endregion

        #region EmptyView 返回一个空的控制器视图
        /// <summary>
        /// 返回一个空的控制器视图
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ActionResult EmptyView(string icon = "zhengminhudongziji01", string msg = "暂无相关信息", string color = "")
        {
            ViewBag.Icon = icon;
            ViewBag.Message = msg;
            ViewBag.Color = color;
            return View("~/Views/Home/Empty.cshtml");
        }

        #endregion

        #region Jsonp(object data) 返回JSONP格式数据

        /// <summary>
        /// 返回JSONP格式数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected JavaScriptResult Jsonp(object data)
        {
            string callback = Request["callback"];
            if (string.IsNullOrEmpty(callback)) { callback = "_callback"; }
            var json = ToJson(data);
            var result = callback + "(" + json + ")";
            return new JavaScriptResult()
            {
                Script = result
            };
        }
        #endregion

        #region 公共方法
        protected string ToJson(object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }


        /// <summary>
        /// 获取客户端提交的数据文本
        /// </summary>
        /// <returns></returns>
        protected string GetInputStream()
        {
            byte[] byts = new byte[Request.InputStream.Length];
            Request.InputStream.Read(byts, 0, byts.Length);
            return System.Text.Encoding.UTF8.GetString(byts);
        }

        #endregion

    }
}