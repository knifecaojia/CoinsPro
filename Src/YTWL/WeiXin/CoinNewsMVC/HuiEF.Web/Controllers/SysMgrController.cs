using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuiEF.Web.Controllers
{
    /// <summary>
    /// 创建人：惠
    /// 日期：2017-01-11
    /// 说明：系统管理
    /// </summary>
    public class SysMgrController : Controller
    {
        HuiEF.Data.HuiEntities db = new Data.HuiEntities();
        public ActionResult LoginLog()
        {
            var logs = db.Login_Log.OrderByDescending(r => r.create_time).Take(45).ToList();
            return View(logs);

        }

    }
}
