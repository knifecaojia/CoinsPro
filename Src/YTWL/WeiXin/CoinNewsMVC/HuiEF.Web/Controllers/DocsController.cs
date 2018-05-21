using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HuiEF.Web.Controllers
{
    public class DocsController : BaseController
    {
        //
        // GET: /Docs/

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Add(FormCollection fc)
        {
            return Success();
        }
    }
}
