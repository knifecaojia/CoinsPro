using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;

namespace Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            AuthorizeRequest += new EventHandler(MvcApplication_AuthorizeRequest);
        }

        void MvcApplication_AuthorizeRequest(object sender, EventArgs e)
        {
            IIdentity id = Context.User.Identity;
            if (id.IsAuthenticated)
            {

                var roles = new DAO.BLL.B_Manager().GetRoles(id.Name);
                Context.User = new GenericPrincipal(id, roles);
            }
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }
        protected void Application_Start()
        {
            CZLogger.LogTools.InitLog4Net();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}