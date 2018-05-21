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

namespace Web.Areas.Demo.Controllers
{
    public class Send_smsController : Controller
    {
        //
        // GET: /Demo/Default1/

        public ActionResult Index()
        {
            return View();
        }

        #region Send_sms

        [AuthorizeFilter]
        public JsonResult AddSend_sms(string appkey, string secret, string SmsFreeSignName, string SmsParam, string RecNum, string SmsTemplateCode)
        {
            Sms sms = new Sms();
            var res = sms.Send(appkey, secret, SmsFreeSignName, SmsParam, RecNum, SmsTemplateCode);
            Common.Json json = new Common.Json();
            if (res == null || !res.Success)
            {
                json.status = -1;
                json.msg = "短信发送失败!";
            }
            else
            {
                json.msg = "短信发送成功!";
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
