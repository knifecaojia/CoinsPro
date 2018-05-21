using System.Web;
using System.Web.Mvc;
using Web.helper;

namespace Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new Log4NetExceptionFilter());
            filters.Add(new AuthorizeFilterAttribute());
        }
    }
}