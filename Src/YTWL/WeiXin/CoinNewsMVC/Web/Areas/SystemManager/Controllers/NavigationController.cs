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
    public class NavigationController : Controller
    {
        //
        // GET: /SystemManager/Navigation/

        public ActionResult Index()
        {
            return View();
        }

        #region Navigation
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetNavList(int limit = 10, int offset = 1)
        {
            B_Navigation b_nav = new B_Navigation();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };

            List<Domain.Navigation> list = new List<Domain.Navigation>();
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=Common.EnumBase.SearchType.Eq},
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
            };
            var list_nav = b_nav.GetList(st, order);
            var list_nav_cout = b_nav.GetCount(st);
            foreach (var nav in list_nav)
            {
                list.Add(nav);
                st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="parent_id",value=nav.id,searchType=Common.EnumBase.SearchType.Eq}
                };
                //查询二级菜单
                var list_sub_nav = b_nav.GetList(st, order);
                foreach (var sub_nav in list_sub_nav)
                {
                    list.Add(sub_nav);
                }
            }
            return Json(new { total = list_nav_cout, rows = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetNavSubList(string index, string parent_id, int limit = 10, int offset = 1)
        {
            B_Navigation b_nav = new B_Navigation();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="parent_id",value=Convert.ToInt32(parent_id),searchType=Common.EnumBase.SearchType.Eq},
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
            };
            var list_nav = b_nav.GetList(st, order);
            return Json(list_nav, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        [AuthorizeFilter]
        public JsonResult AddNav(string txt_parent_id, string txt_icon_url, string txt_title, string txt_link_url, string txt_sort_id, string txt_is_lock, string txt_action_type)
        {
            Common.Json json = new Common.Json();
            B_Navigation b_nav = new B_Navigation();
            Domain.Navigation model = new Domain.Navigation();

            model.icon_url = txt_icon_url;
            model.title = txt_title;
            model.link_url = txt_link_url;
            if (!string.IsNullOrEmpty(model.link_url) && model.link_url != "#") 
            {
                model.controllerName = model.link_url.Substring(model.link_url.LastIndexOf("/")+1);
            }
            model.sort_id = Convert.ToInt32(txt_sort_id);
            model.is_lock = txt_is_lock;
            model.parent_id = Convert.ToInt32(txt_parent_id);
            model.action_type = txt_action_type;
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            var res = b_nav.Save(model);
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
        public JsonResult EditNav(int id, string txt_parent_id, string txt_icon_url, string txt_title, string txt_link_url, string txt_sort_id, string txt_is_lock, string txt_action_type)
        {
            Common.Json json = new Common.Json();
            B_Navigation b_nav = new B_Navigation();
            Domain.Navigation model = b_nav.GetNav(id);
            model.icon_url = txt_icon_url;
            model.title = txt_title;
            model.link_url = txt_link_url;
            if (!string.IsNullOrEmpty(model.link_url) && model.link_url != "#")
            {
                model.controllerName = model.link_url.Substring(model.link_url.LastIndexOf("/") +1);
            }
            else 
            {
                model.controllerName = "";
            }
            model.sort_id = Convert.ToInt32(txt_sort_id);
            model.is_lock = txt_is_lock;
            model.parent_id = Convert.ToInt32(txt_parent_id);
            model.action_type = txt_action_type;
            if (model.parent_id == 0)
            {
                model.channel_id = 1;
            }
            else
            {
                model.channel_id = 2;
            }
            b_nav.Update(model);
            json.msg = "修改成功!";
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetNav(int id)
        {
            B_Navigation b_nav = new B_Navigation();
            var model = b_nav.GetNav(id);
            return Json(model);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelNav(string ids)
        {
            Common.Json json = new Common.Json();
            B_Navigation b_nav = new B_Navigation();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_nav.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }
        #endregion

        public ActionResult GetFont()
        {
            return PartialView("~/Areas/SystemManager/Views/Navigation/_font.cshtml");
        }
    }
}
