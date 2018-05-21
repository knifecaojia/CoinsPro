using System.Web.Mvc;

namespace Web.Areas.SystemSafety
{
    public class SystemSafetyAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SystemSafety";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SystemSafety_default",
                "SystemSafety/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
