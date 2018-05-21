using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuiEF.Web.Controllers
{
    /// <summary>
    /// 创建人：惠
    /// 日期：2017-01-10
    /// 说明：用户相关类
    /// </summary>
    [AllowAnonymous]
    public class UserController : BaseController
    {
        HuiEF.Data.HuiEntities db = new Data.HuiEntities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {

            Session[Common.SessionKey.SK_ADMIN_USER_INFO] = "admin";
            //记录日志
            var userip = Utils.getIp();
            string[] location = new string[4];
            if (userip.Contains("."))
            {
                location = Hui.Utils.IPHelper.Find(userip);
            }
            var log = new Data.HuiEntity.Login_Log()
            {
                ip = userip,
                city = location[2],
                province = location[1],
                area = location[3],
                member_id = Session[Common.SessionKey.SK_ADMIN_USER_INFO].ToString(),
                create_time = DateTime.Now,
            };
            db.Login_Log.Add(log);
            db.SaveChanges();


            return RedirectToAction("Index", "Home");
        }
    }
}
