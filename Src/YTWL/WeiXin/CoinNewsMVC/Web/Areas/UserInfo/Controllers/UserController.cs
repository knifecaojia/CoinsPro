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


namespace Web.Areas.UserInfo.Controllers
{
    public class UserController : BaseController
    {
        //
        // GET: /UserInfo/User/

        public ActionResult Index()
        {
            return View();
        }


        #region User

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult GetUserList(int limit = 10, int offset = 1, string user_name = "", string mobile = "")
        {
            B_Users b_user = new B_Users();
            List<Order> order = new List<Order>() { Order.Desc("id") };

            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="user_name",value=user_name,searchType=Common.EnumBase.SearchType.Like},
                new SearchTemplate(){key="mobile",value=mobile,searchType=Common.EnumBase.SearchType.Like},
                new SearchTemplate(){key="",value=new int[]{offset,limit},searchType=Common.EnumBase.SearchType.Paging}
            };
            var list_user = b_user.GetList(st, order);

            var total = b_user.GetCount(st);

            return Json(new { total = total, rows = list_user }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult AddUser(string txt_user_name, string txt_mobile, string txt_email, string txt_nick_name, string txt_password)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_Users b_user = new DAO.BLL.B_Users();
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="user_name",value=txt_user_name,searchType=Common.EnumBase.SearchType.Eq},
            };
            var res = b_user.GetCount(st);
            if (res > 0)
            {
                json.status = -1;
                json.msg = "用户名已存在!";
                json.pitchId = "txt_user_name";
                return Json(json);
            }
            st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="mobile",value=txt_mobile,searchType=Common.EnumBase.SearchType.Eq},
                };
            res = b_user.GetCount(st);
            if (res > 0)
            {
                json.status = -1;
                json.msg = "手机号已存在!";
                json.pitchId = "txt_mobile";
                return Json(json);
            }
            Domain.Users m_user = new Domain.Users();
            m_user.user_name = txt_user_name;
            m_user.mobile = txt_mobile;
            m_user.password = Common.Encrypt.md5(txt_password);
            m_user.email = txt_email;
            m_user.nick_name = txt_nick_name;
            res = b_user.Save(m_user);
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
        public JsonResult GetUser(int id)
        {
            DAO.BLL.B_Users b_user = new B_Users();
            var res = b_user.GetUser(id);
            return Json(res);
        }
        [HttpPost]
        [AuthorizeFilter]
        public JsonResult EditUser(int id, string txt_user_name, string txt_mobile, string txt_email, string txt_nick_name, string txt_password)
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            Common.Json json = new Common.Json();
            DAO.BLL.B_Users b_user = new DAO.BLL.B_Users();
            var m_user = b_user.GetUser(id);
            if (m_user.user_name != txt_user_name)
            {
                List<SearchTemplate> st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="user_name",value=txt_user_name,searchType=Common.EnumBase.SearchType.Eq},
                };
                var res = b_user.GetCount(st);
                if (res > 0)
                {
                    json.status = -1;
                    json.msg = "用户名已存在!";
                    json.pitchId = "txt_user_name";
                    return Json(json);
                }
            }
            if (m_user.mobile != txt_mobile)
            {
                List<SearchTemplate> st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="mobile",value=txt_mobile,searchType=Common.EnumBase.SearchType.Eq},
                };
                var res = b_user.GetCount(st);
                if (res > 0)
                {
                    json.status = -1;
                    json.msg = "手机号已存在!";
                    json.pitchId = "txt_mobile";
                    return Json(json);
                }
            }
            m_user.user_name = txt_user_name;
            m_user.mobile = txt_mobile;
            if (m_user.password != txt_password)
            {
                m_user.password = Common.Encrypt.md5(txt_password);
            }
            m_user.email = txt_email;
            m_user.nick_name = txt_nick_name;
            b_user.Update(m_user);
            json.msg = "修改成功!";
            return Json(json);
        }

        [HttpPost]
        [AuthorizeFilter]
        public JsonResult DelUser(string ids)
        {
            Common.Json json = new Common.Json();
            DAO.BLL.B_Users b_user = new DAO.BLL.B_Users();
            foreach (var id in ids.Split(new char[] { ',' }))
            {
                b_user.Delete(Convert.ToInt32(id));
            }
            json.msg = "成功删除" + ids.Split(new char[] { ',' }).Length + "条记录!";
            return Json(json);
        }


        #endregion
    }
}
