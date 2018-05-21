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

namespace Web.Areas.SystemManager.Controllers
{
    public class OrganizationController : BaseController
    {
        //
        // GET: /SystemManager/Organization/

        public ActionResult Index()
        {
            return View();
        }
        #region Organization
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetOrganizationList(int limit = 10, int offset = 1)
        {
            B_Organization b_org = new B_Organization();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };

            List<Domain.Navigation> list = new List<Domain.Navigation>();
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=Common.EnumBase.SearchType.Eq},
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
            };
            var list_org = b_org.GetList(st, order);
            var list_org_cout = b_org.GetCount(st);
            return Json(new { total = list_org_cout, rows = list_org }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetOrganizationSubList(string parent_id)
        {
            B_Organization b_org = new B_Organization();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="parent_id",value=Convert.ToInt32(parent_id),searchType=Common.EnumBase.SearchType.Eq}
            };
            var list_sub_org = b_org.GetList(st, order);
            return Json(list_sub_org, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        [AuthorizeFilter]
        public JsonResult AddOrganization(FormCollection form)
        {
            Common.Json json = new Common.Json();
            B_Organization b_org = new B_Organization();
            B_Organization_type b_orgtype = new B_Organization_type();
            B_Manager b_manager = new B_Manager();
            Domain.Organization model = new Domain.Organization();

            model.name = form["txt_name"];
            model.parent_id = Convert.ToInt32(form["txt_parent_id"]);
            if (model.parent_id != 0)
            {
                var m = b_org.Get(model.parent_id);
                if (string.IsNullOrEmpty(model.parent_ids))
                {
                    model.parent_ids = model.parent_id + ",";
                }
                else
                {
                    model.parent_ids = m.parent_ids + m.parent_id + ",";
                }
                model.levels = m.levels + 1;
            }
            else
            {
                model.levels = 0;
            }
            model.orgtype = b_orgtype.Get(Convert.ToInt32(form["txt_orgtype_id"]));
            model.sort_id = Convert.ToInt32(form["txt_sort_id"]);
            model.add_manager = b_manager.Get(Convert.ToInt32(base.User.Identity.Name));
            model.add_time = DateTime.Now;
            model.status = Convert.ToInt32(form["txt_status"]);
            var res = b_org.Save(model);
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
        [ValidateInput(false)]
        [AuthorizeFilter]
        public JsonResult Editorganization(FormCollection form)
        {
            Common.Json json = new Common.Json();
            B_Organization b_org = new B_Organization();
            B_Organization_type b_orgtype = new B_Organization_type();
            B_Manager b_manager = new B_Manager();
            Domain.Organization model = b_org.Get(Convert.ToInt32(form["id"]));

            model.name = form["txt_name"];
            model.parent_id = Convert.ToInt32(form["txt_parent_id"]);
            if (model.parent_id != 0)
            {
                var m = b_org.Get(model.parent_id);
                if (string.IsNullOrEmpty(model.parent_ids))
                {
                    model.parent_ids = model.parent_id + ",";
                }
                else
                {
                    model.parent_ids = m.parent_ids + m.parent_id + ",";
                }
                model.levels = m.levels + 1;
            }
            else
            {
                model.levels = 0;
            }
            model.orgtype = b_orgtype.Get(Convert.ToInt32(form["txt_orgtype_id"]));
            model.sort_id = Convert.ToInt32(form["txt_sort_id"]);
            model.status = Convert.ToInt32(form["txt_status"]);
            b_org.Update(model);
            json.msg = "修改成功!";
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetOrganization(int id)
        {
            B_Organization b_org = new B_Organization();
            var model = b_org.Get(id);
            return Json(model);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelOrganization(string ids)
        {
            Common.Json json = new Common.Json();
            B_Organization b_org = new B_Organization();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_org.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }
        #endregion
    }
}
