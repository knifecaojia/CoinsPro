using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Common;
using Domain;
using NHibernate.Criterion;
namespace Web.Controllers
{

    class A 
    {
        public string Name { get; set; }

        public List<int> k { get; set; }

        public List<string> s { get; set; }
    }

    public class HomeController : Controller
    {
        //
        // GET: /Home/

        //可以指点让某个用户访问,或者某个组的用户访问
        //[Authorize(Users = "?")]  
        public ActionResult Index()
        {
            //var name = User.Identity.Name;
            //string[] str = Roles.GetRolesForUser();

            //传递一个角色id
            DAO.BLL.B_Manager b_manager = new DAO.BLL.B_Manager();
            var model = b_manager.Get(Convert.ToInt32(User.Identity.Name));
            ViewData["manager"] = model;
            return View();
        }

        public ActionResult Login()
        {
            FormsAuthentication.SignOut();//安全退出
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
        public ActionResult Wiki()
        {
            DAO.BLL.B_Wiki b_wiki = new DAO.BLL.B_Wiki();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };

            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="parent_id",value=0,searchType=Common.EnumBase.SearchType.Eq}
            };
            var list_wiki = b_wiki.GetList(st,order);
            ViewData["list_wiki"] = list_wiki;
            return View();
        }
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        #region====================测试==============================
        public ActionResult Test() 
        {
            CacheRedis.RedisHelper rh = new CacheRedis.RedisHelper(true);
            A a = new A();
            a.Name = "haha";
            if (a.k == null) a.k = new List<int>();
            a.k.Add(1);
            a.k.Add(2);
            if (a.s == null) a.s = new List<string>();
            a.s.Add("aa");
            a.s.Add("bb");
            rh.Set<A>("998877", a);

            var res = rh.Get<A>("998877");
            return View();
        }

        [HttpPost]
        public JsonResult SetSession()
        {
            Common.Json json = new Common.Json();
            Session["test"] = "当前时间: " + DateTime.Now;
            json.msg = "写入成功!";
            json.status = 0;
            return Json(json);
        }

        [HttpPost]
        public JsonResult GetSession()
        {

            string a = "2017/2/22";
            string b = "16:40:51";
            DateTime dt = Convert.ToDateTime(a +" "+ b);

            Common.Json json = new Common.Json();
            try
            {
                var res = Session["test"].ToString();
                json.msg = res;
                json.status = 0;
            }
            catch
            {
                json.status = -1;
                json.msg = "获取session失败!";
            }
            return Json(json);
        }

        #endregion
        #region ===================内部方法==========================
        /// <summary>
        /// 普通登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userpwd"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult login(string un, string pwd, string code, string returnUrl)
        {
            Common.Json json = new Common.Json();
            if (Common.Encrypt.md5(code.ToLower()) != WebHelper.GetSession("lsj_pic_code"))
            {
                json.msg = "验证码不正确！";
                json.status = -1;
                return Json(json);
            }
            DAO.BLL.B_Manager b_manager = new DAO.BLL.B_Manager();
            if (ModelState.IsValid)
            {
                List<SearchTemplate> st = new List<SearchTemplate>()
                {
                    new SearchTemplate(){key="user_name",value=un,searchType=Common.EnumBase.SearchType.Eq},
                    new SearchTemplate(){key="password",value=Common.Encrypt.md5(pwd),searchType=Common.EnumBase.SearchType.Eq},
                    new SearchTemplate(){key="is_lock",value="√",searchType=Common.EnumBase.SearchType.Eq}
                };
                var list_model = b_manager.GetList(st, null);
                if (list_model.Count > 0)
                {
                    FormsAuthentication.SetAuthCookie(list_model[0].id.ToString(), true);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        json.returnUrl = returnUrl;
                    }
                    else
                    {
                        json.returnUrl = "/home/index";
                    }

                    DAO.BLL.B_Manager_log b_log = new DAO.BLL.B_Manager_log();
                    //记录用户所在地区
                    var userip = Utils.getIp();
                    string location = "";
                    if (userip.Contains("."))
                    {
                        location = Hui.Utils.IPHelper.GetFullName(userip);
                    }
                  
                    b_log.Add(list_model[0].id, Common.EnumBase.Authorize.登录.Description(), "manager", "用户登录："+ location, Utils.getIp());
                }
                else
                {
                    json.msg = "用户名或密码不正确！";
                    json.status = -1;
                }
            }

            return Json(json);
        }

        [HttpPost]
        public JsonResult GetSubWiki(int id)
        {
            DAO.BLL.B_Wiki b_wiki = new DAO.BLL.B_Wiki();
            List<Order> order = new List<Order>() { Order.Asc("sort_id") };
            List<SearchTemplate> st = new List<SearchTemplate>()
            {
                new SearchTemplate(){key="parent_id",value=id,searchType=Common.EnumBase.SearchType.Eq}
            };
            var list_wiki = b_wiki.GetList(st, order);;
            return Json(list_wiki);
        }
        #endregion ===================内部方法==========================



        [HttpGet]
        public ActionResult GetAuthCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }

        public JsonResult CheckLogin()
        {
            Common.Json json = new Json();
            if (User.Identity.IsAuthenticated)
            {
                json.msg = "有效";
                json.status = 0;
            }
            else
            {
                json.status = -1;
                json.msg = "登录失效";

            }
            return Json(json);
        }

    }
}
