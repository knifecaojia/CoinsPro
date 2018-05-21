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
    public class DictionController : Controller
    {
        //
        // GET: /Demo/Diction/

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeFilter]
        public ActionResult GetWord(int id = -1)
        {
            ViewData["obj"] = id;
            return PartialView("~/Areas/Demo/Views/Diction/_word.cshtml");
        }

        #region Organization_type

        public ActionResult Organization_type()
        {
            return PartialView("~/Areas/Demo/Views/Diction/_organization_type.cshtml");
        }
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetOrganization_typeList(int limit = 10, int offset = 1)
        {
            B_Organization_type b_orgtype = new B_Organization_type();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };

            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
            };
            var list_user = b_orgtype.GetList(st, order);

            var total = b_orgtype.GetCount(st);

            return Json(new { total = total, rows = list_user }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult AddOrganization_type(FormCollection form)
        {
            Common.Json json = new Common.Json();
            B_Organization_type b_orgtype = new B_Organization_type();
            Domain.Organization_type m_orgtype = new Domain.Organization_type();
            m_orgtype.orgtype_name = form["txt_name"];
            m_orgtype.sort_id = Convert.ToInt32(form["txt_sort_id"]);
            m_orgtype.remark = form["txt_remark"];
            var res = b_orgtype.Save(m_orgtype);
            if (res <= 0)
            {
                json.status = -1;
                json.msg = "添加失败!";
                return Json(json);
            }
            json.msg = "添加成功!";
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetOrganization_type(int id)
        {
            B_Organization_type b_orgtype = new B_Organization_type();
            var res = b_orgtype.Get(id);
            return Json(res);
        }
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult EditOrganization_type(FormCollection form)
        {
            Common.Json json = new Common.Json();
            B_Organization_type b_orgtype = new B_Organization_type();
            Domain.Organization_type m_orgtype = b_orgtype.Get(Convert.ToInt32(form["id"]));
            m_orgtype.orgtype_name = form["txt_name"];
            m_orgtype.sort_id = Convert.ToInt32(form["txt_sort_id"]);
            m_orgtype.remark = form["txt_remark"];
            b_orgtype.Update(m_orgtype);
            json.msg = "修改成功!";
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelOrganization_type(string ids)
        {
            Common.Json json = new Common.Json();
            B_Organization_type b_orgtype = new B_Organization_type();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_orgtype.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }

        #endregion
    }
}
