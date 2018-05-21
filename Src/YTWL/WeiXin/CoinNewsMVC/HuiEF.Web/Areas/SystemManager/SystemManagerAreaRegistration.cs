using System.Web.Mvc;

namespace Web.Areas.SystemManager
{
    public class SystemManagerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SystemManager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SystemManager_default",
                "SystemManager/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
