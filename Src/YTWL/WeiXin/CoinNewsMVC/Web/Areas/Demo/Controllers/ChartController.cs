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

namespace Web.Areas.Demo.Controllers
{
    public class ChartController : Controller
    {
        //
        // GET: /Demo/Chart/

        public ActionResult Index()
        {
            return View();
        }
        #region Chart
        [AuthorizeFilter]
        [HttpPost]
        public JsonResult GetChart()
        {

            B_Manager_log b_log = new B_Manager_log();
            var res = b_log.GetNavClick(7);
            List<string> legend = new List<string>();
            List<Series> list = new List<Series>();
            foreach (var item in res.Keys)
            {
                if (item == "图表工具" || item == "数据备份" || item == "短信工具" || item == "栏目管理") continue;
                legend.Add(item);
                Series menuSeries = new Series();
                menuSeries.name = item;
                menuSeries.type = "line";
                menuSeries.areaStyle = new itemStyle() { normal = new object() };
                menuSeries.data = res[item].ToList();
                //menuSeries.stack = "总量";
                list.Add(menuSeries);
            }

            xAxis xaxis = new xAxis()
            {
                boundaryGap = false,
                type = "category",
                data = new List<object>() { DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-5).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd") }
            };


            List<object> data = new List<object>();
            data.Add(legend);
            data.Add(xaxis);
            data.Add(list);

            return Json(data);
        }
        #endregion
    }
}
