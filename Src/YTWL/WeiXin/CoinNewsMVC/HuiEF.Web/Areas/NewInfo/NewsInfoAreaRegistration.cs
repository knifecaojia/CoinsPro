using System.Web.Mvc;

namespace Web.Areas.NewInfo
{
    public class NewsInfoAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "NewInfo";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "NewInfo_default",
                "NewInfo/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
