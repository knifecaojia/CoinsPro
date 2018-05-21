using Common;
using DAO.BLL;
using Domain;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Web.helper;

namespace Web.Areas.NewInfo.Controllers
{
    public class WikiController : BaseController
    {
        //
        // GET: /NewInfo/Wiki/

        public ActionResult Index()
        {
            return View();
        }


        #region Wiki

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetWikiList(int limit = 10, int offset = 1)
        {
            B_Wiki b_wiki = new B_Wiki();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };

            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=Common.EnumBase.SearchType.Eq},
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
            };
            List<Domain.Wiki> list = new List<Wiki>();

            var list_wiki = b_wiki.GetList(st, order);
            foreach (var item in list_wiki)
            {
                list.Add(item);
                st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="parent_id",value=item.id,searchType=Common.EnumBase.SearchType.Eq}
                };
                var sub_wiki = b_wiki.GetList(st, order);
                list.AddRange(sub_wiki);
            }


            var total = b_wiki.GetCount(st);
            return Json(new { total = total, rows = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AuthorizeFilter]
        public JsonResult AddWiki(FormCollection form)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_Wiki b_wiki = new DAO.BLL.B_Wiki();
            B_Manager b_manager = new B_Manager();
            Domain.Wiki m_wiki = new Domain.Wiki();
            m_wiki.title = form["txt_title"];
            m_wiki.tags = form["txt_tags"];
            m_wiki.synopsis = form["txt_synopsis"];
            m_wiki.sort_id = Convert.ToInt32(form["txt_sort_id"]);
            m_wiki.content = form["txtContent"];
            m_wiki.manager = b_manager.Get(Convert.ToInt32(base.User.Identity.Name));
            m_wiki.add_time = DateTime.Now;
            m_wiki.parent_id = Convert.ToInt32(form["txt_parent_id"]);
            if (m_wiki.parent_id != 0)
            {
                var m = b_wiki.Get(m_wiki.parent_id);
                if (string.IsNullOrEmpty(m_wiki.parent_ids))
                {
                    m_wiki.parent_ids = m_wiki.parent_id + ",";
                }
                else
                {
                    m_wiki.parent_ids = m.parent_ids + m.parent_id + ",";
                }
                m_wiki.levels = m.levels + 1;
            }
            else
            {
                m_wiki.levels = 0;
            }
            var res = b_wiki.Save(m_wiki);
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
        public JsonResult GetWiki(int id)
        {
            DAO.BLL.B_Wiki b_wiki = new B_Wiki();
            var res = b_wiki.Get(id);
            return Json(res);
        }
        [HttpPost]
        [ValidateInput(false)]
        [AuthorizeFilter]
        public JsonResult EditWiki(FormCollection form)
        {
            Common.Json json = new Common.Json();
            B_Manager b_manager = new B_Manager();
            DAO.BLL.B_Wiki b_wiki = new DAO.BLL.B_Wiki();
            var m_wiki = b_wiki.Get(Convert.ToInt32(form["id"]));
            m_wiki.title = form["txt_title"];
            m_wiki.tags = form["txt_tags"];
            m_wiki.synopsis = form["txt_synopsis"];
            m_wiki.sort_id = Convert.ToInt32(form["txt_sort_id"]);
            m_wiki.content = form["txtContent"];
            m_wiki.manager = b_manager.Get(Convert.ToInt32(base.User.Identity.Name));
            m_wiki.add_time = DateTime.Now;
            m_wiki.parent_id = Convert.ToInt32(form["txt_parent_id"]);
            if (m_wiki.parent_id != 0)
            {
                var m = b_wiki.Get(m_wiki.parent_id);
                if (string.IsNullOrEmpty(m_wiki.parent_ids))
                {
                    m_wiki.parent_ids = m_wiki.parent_id + ",";
                }
                else
                {
                    m_wiki.parent_ids = m.parent_ids + m.parent_id + ",";
                }
                m_wiki.levels = m.levels + 1;
            }
            else
            {
                m_wiki.levels = 0;
            }
            b_wiki.Update(m_wiki);
            json.msg = "修改成功!";
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelWiki(string ids)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_Wiki b_wiki = new DAO.BLL.B_Wiki();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_wiki.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }


        #endregion
    }
}
