using Common;
using DAO.BLL;
using Domain;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.helper;

namespace Web.Areas.SystemSafety.Controllers
{
    public class LogController : BaseController
    {
        //
        // GET: /SystemSafety/Log/

        public ActionResult Index()
        {
            return View();
        }
        #region Log


        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetLogList(int limit = 10, int offset = 1, string user_name = "", DateTime? start_time = null, DateTime? end_time = null)
        {
            B_Manager_log b_log = new B_Manager_log();
            List<Order> order = new List<Order>() { Order.Desc("id") };
            List<SearchTemplate> st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="user_name",value=user_name,searchType=Common.EnumBase.SearchType.Eq},
                    new SearchTemplate(){key="add_time",value=start_time,searchType=Common.EnumBase.SearchType.Ge},
                    new SearchTemplate(){key="add_time",value=end_time,searchType=Common.EnumBase.SearchType.Le},
                    new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
                };
            var list_manager = b_log.GetList(st, order);
            var total = b_log.GetCount(st);
            return this.MyJson(new { total = total, rows = list_manager }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelLog(string ids)
        {
            Common.Json json = new Common.Json();
            B_Manager_log b_log = new B_Manager_log();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_log.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }
        #endregion
    }
}
