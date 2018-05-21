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
    public class ManagerController : BaseController
    {
        //
        // GET: /SystemManager/Manager/

        public ActionResult Index()
        {
            return View();
        }
        #region Manager

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetManagerList(int limit = 10, int offset = 1, string user_name = "", string mobile = "")
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            B_Manager b_manager = new B_Manager();
            List<Order> order = new List<Order>() { Order.Desc("id") };
            List<SearchTemplate> st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="user_name",value=user_name,searchType=Common.EnumBase.SearchType.Eq},
                    new SearchTemplate(){key="mobile",value=mobile,searchType=Common.EnumBase.SearchType.Eq},
                    new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
                };
            var list_manager = b_manager.GetList(st, null);
            var total = b_manager.GetCount(st);
            return this.MyJson(new { total = total, rows = list_manager }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult AddManager(string txt_user_name, string txt_role_id, string txt_real_name, string txt_mobile, string txt_email, string txt_password, string txt_is_lock)
        {
            Common.Json json = new Common.Json();
            B_Manager b_manager = new B_Manager();
            List<SearchTemplate> st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="user_name",value=txt_user_name,searchType=Common.EnumBase.SearchType.Eq}
                };
            var res = b_manager.GetCount(st);
            if (res > 0)
            {
                json.status = -1;
                json.msg = "用户名已存在!";
                json.pitchId = "txt_user_name";
                return Json(json);
            }
            st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="mobile",value=txt_mobile,searchType=Common.EnumBase.SearchType.Eq}
                };
            res = b_manager.GetCount(st);
            if (res > 0)
            {
                json.status = -1;
                json.msg = "手机号已存在!";
                json.pitchId = "txt_mobile";
                return Json(json);
            }
            Domain.Manager model = new Domain.Manager();
            model.user_name = txt_user_name;
            model.real_name = txt_real_name;
            model.mobile = txt_mobile;
            model.email = txt_email;
            model.password = Common.Encrypt.md5(txt_password);
            model.is_lock = txt_is_lock;
            model.add_time = DateTime.Now;
            B_Manager_role b_mr = new B_Manager_role();
            //必须给一个角色对象,这里相当于又执行了一条update,可以在这里修改对应角色的内容(这就是NHibernate搞的鬼)
            model.manager_role = b_mr.Get(Convert.ToInt32(txt_role_id));
            res = b_manager.Save(model);
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
        public JsonResult GetManager(int id)
        {
            DAO.BLL.B_Manager b_manager = new B_Manager();
            var res = b_manager.Get(id);
            return Json(res);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult EditManager(string id, string txt_user_name, string txt_role_id, string txt_real_name, string txt_mobile, string txt_email, string txt_password, string txt_is_lock)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_Manager b_manager = new DAO.BLL.B_Manager();
            var m_manager = b_manager.Get(Convert.ToInt32(id));
            if (m_manager.user_name != txt_user_name)
            {
                List<SearchTemplate> st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="user_name",value=txt_user_name,searchType=Common.EnumBase.SearchType.Eq}
                };
                var res = b_manager.GetCount(st);
                if (res > 0)
                {
                    json.status = -1;
                    json.msg = "用户名已存在!";
                    json.pitchId = "txt_user_name";
                    return Json(json);
                }
            }
            if (m_manager.mobile != txt_mobile)
            {
                List<SearchTemplate> st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="mobile",value=txt_mobile,searchType=Common.EnumBase.SearchType.Eq}
                };
                var res = b_manager.GetCount(st);
                if (res > 0)
                {
                    json.status = -1;
                    json.msg = "手机号已存在!";
                    json.pitchId = "txt_mobile";
                    return Json(json);
                }
            }
            m_manager.user_name = txt_user_name;
            m_manager.real_name = txt_real_name;
            m_manager.mobile = txt_mobile;
            m_manager.email = txt_email;
            if (m_manager.password != Common.Encrypt.md5(txt_password))
            {
                m_manager.password = Common.Encrypt.md5(txt_password);
            }
            m_manager.is_lock = txt_is_lock;
            m_manager.add_time = DateTime.Now;
            B_Manager_role b_mr = new B_Manager_role();
            //cascade：有all、save-update、delete、none几个选项，表示  该表做一些操作时 是否作用于 关联的表，比如在一对多关系中如果cascade="all"时，那么父表所做的操作都会作用于子表，比如删除某个用户，那么这个用户下的文章也会关联一起删除。
            m_manager.manager_role = b_mr.Get(Convert.ToInt32(txt_role_id));
            b_manager.Update(m_manager);
            json.msg = "修改成功!";
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelManager(string ids)
        {
            Common.Json json = new Common.Json();
            B_Manager b_manager = new B_Manager();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_manager.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }
        #endregion
    }
}
