using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Domain;
namespace HuiEF.Web.Controllers
{
    /// <summary>
    /// 创建人：惠
    /// 创建日期：2017-01-05
    /// 说明：项目主页
    /// </summary>
    public class HomeController : Controller
    {
        HuiEF.Data.HuiEntities db = new Data.HuiEntities();
        //
        // GET: /Home/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Home()
        {
            //var logs = db.Login_Log.OrderByDescending(r => r.create_time).Take(45).ToList();
            //return View(logs);
            return Redirect("/SiteFiles/huiiconfont/demo_fontclass.html");

        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult TestSqlite()
        {
            HuiEF.Data.HuiEntities db = new Data.HuiEntities();
            var list = db.Test.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}
