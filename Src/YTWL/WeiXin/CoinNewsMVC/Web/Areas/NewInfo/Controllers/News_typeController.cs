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

namespace Web.Areas.NewInfo.Controllers
{
    public class News_typeController : BaseController
    {
        //
        // GET: /NewInfo/News_type/
        [AuthorizeFilter]
        public ActionResult Index()
        {
            return View();
        }


        #region News_type
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetNews_typeList(int limit = 10, int offset = 1)
        {
            B_News_type b_nt = new B_News_type();
            List<Order> order = new List<Order>() { Order.Asc("id") };
            List<SearchTemplate> st = new List<SearchTemplate>() 
            {
                new SearchTemplate(){key="parent_id",value=0,searchType = EnumBase.SearchType.Eq},
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
            };
            List<Domain.News_type> list = new List<Domain.News_type>();
            var list_nt = b_nt.GetList(st, order);
            var list_nt_count = b_nt.GetCount(st);
            foreach (var item in list_nt)
            {
                list.Add(item);
                st = new List<SearchTemplate>() 
                {
                    new SearchTemplate(){key="parent_id",value=item.id,searchType = EnumBase.SearchType.Eq}
                };
                var list_ntsub = b_nt.GetList(st, order);
                foreach (var sub in list_ntsub)
                {
                    list.Add(sub);
                }

            }
            var total = list_nt_count;

            return Json(new { total = total, rows = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult AddNews_type(int txt_parent_id, string txt_title, int? txt_sort_id, string txt_seo_title, string txt_seo_keywords, string txt_seo_description)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_News_type b_nt = new DAO.BLL.B_News_type();
            Domain.News_type m_nt = new Domain.News_type();
            m_nt.parent_id = txt_parent_id;
            m_nt.title = txt_title;
            m_nt.sort_id = txt_sort_id;
            m_nt.seo_title = txt_seo_title;
            m_nt.seo_keywords = txt_seo_keywords;
            m_nt.seo_description = txt_seo_description;
            var res = b_nt.Save(m_nt);
            if (res > 0)
            {
                json.msg = "添加成功!";
            }
            else
            {
                json.msg = "添加失败!";
                json.status = -1;
            }

            return Json(json);
        }


        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetNews_type(int id)
        {
            B_News_type b_nt = new B_News_type();
            var res = b_nt.Get(id);
            return Json(res);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult EditNews_type(int id, int txt_parent_id, string txt_title, int? txt_sort_id, string txt_seo_title, string txt_seo_keywords, string txt_seo_description)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_News_type b_nt = new DAO.BLL.B_News_type();
            var m_nt = b_nt.Get(id);
            m_nt.parent_id = txt_parent_id;
            m_nt.title = txt_title;
            m_nt.sort_id = txt_sort_id;
            m_nt.seo_title = txt_seo_title;
            m_nt.seo_keywords = txt_seo_keywords;
            m_nt.seo_description = txt_seo_description;
            b_nt.Update(m_nt);
            json.msg = "修改成功!";
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelNews_type(string ids)
        {
            Common.Json json = new Common.Json();
            B_News_type b_nt = new B_News_type();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_nt.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }
        #endregion
    }
}
