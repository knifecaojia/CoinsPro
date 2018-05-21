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
    public class Manager_roleController : Controller
    {
        //
        // GET: /SystemManager/Manager_role/

        public ActionResult Index()
        {
            //获取所有的后台导航
            B_Navigation b_nav = new B_Navigation();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };

            List<Domain.Navigation> list = new List<Domain.Navigation>();
            var list_nav = b_nav.LoadAll(order, 0);
            foreach (var nav in list_nav)
            {
                list.Add(nav);
                //查询二级菜单
                var list_sub_nav = b_nav.LoadAll(order, nav.id);
                foreach (var sub_nav in list_sub_nav)
                {
                    list.Add(sub_nav);
                }
            }
            ViewBag.list_nav = list;
            return View();
        }

        #region Manager_role
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetManager_roleList(int limit = 10, int offset = 1)
        {
            B_Manager_role b_mr = new B_Manager_role();
            List<Order> order = new List<Order>() { Order.Desc("id") };
            var list_user = b_mr.GetList(null, order);

            var total = b_mr.GetCount(null);

            return Json(new { total = total, rows = list_user }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult AddManager_role(string txt_role_name, string txt_action_type)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_Manager_role b_mr = new DAO.BLL.B_Manager_role();
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="role_name",value=txt_role_name,searchType = Common.EnumBase.SearchType.Eq}
            };
            var res = b_mr.GetCount(st);
            if (res > 0)
            {
                json.status = -1;
                json.msg = "角色名已存在!";
                json.pitchId = "txt_role_name";
                return Json(json);
            }
            Domain.Manager_role model = new Domain.Manager_role();
            model.role_name = txt_role_name;
            res = b_mr.Save(model);
            if (res <= 0)
            {
                json.status = -1;
                json.msg = "添加失败!";
                return Json(json);
            }
            B_Manager_role_value b_mrv = new B_Manager_role_value();
            b_mrv.Update(txt_action_type, res);
            json.msg = "添加成功!";

            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult CheckManager_role_value(int role_id, int nav_id, string action_type)
        {
            B_Manager_role_value b_mrv = new B_Manager_role_value();
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="role_id",value=role_id,searchType=Common.EnumBase.SearchType.Eq},
                new SearchTemplate(){key="nav_id",value=nav_id,searchType=Common.EnumBase.SearchType.Eq}
            };
            var list = b_mrv.GetList(st, null);
            if (list.Count == 0)
            {
                return Json("false");
            }
            else
            {
                if (list[0].action_type.Contains(action_type))
                {
                    return Json("true");
                }
                else
                {
                    return Json("false");
                }
            }
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetManager_role(int id)
        {
            B_Manager_role b_mr = new B_Manager_role();
            var res = b_mr.Get(id);
            return Json(res);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult EditManager_role(int id, string txt_role_name, string txt_action_type)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_Manager_role b_mr = new DAO.BLL.B_Manager_role();
            var m_mr = b_mr.Get(id);
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="role_name",value=txt_role_name,searchType = Common.EnumBase.SearchType.Eq}
            };
            var res = b_mr.GetCount(st);
            if (m_mr.role_name != txt_role_name)
            {
                if (res > 0)
                {
                    json.status = -1;
                    json.msg = "角色名已存在!";
                    json.pitchId = "txt_role_name";
                    return Json(json);
                }
            }
            Domain.Manager_role model = m_mr;
            model.role_name = txt_role_name;
            b_mr.Update(model);
            B_Manager_role_value b_mrv = new B_Manager_role_value();
            b_mrv.Update(txt_action_type, model.id);
            json.msg = "修改成功!";

            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelManager_role(string ids)
        {
            Common.Json json = new Common.Json();
            B_Manager_role b_mr = new B_Manager_role();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_mr.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }

        #endregion
    }
}
