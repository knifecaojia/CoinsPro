using MyHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;

namespace HuiEF.Web.Controllers
{
    /// <summary>
    /// 创建人:惠
    /// 日期：2017-01-10
    /// 说明：爬虫演示
    /// </summary>
    public class SpiderController : BaseController
    {
        //
        // GET: /Spider/

        public ActionResult Index()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult DownHtml(string url, string xpath)
        {
            //获取HTML
            HttpHelper spider = new HttpHelper();
            var result = spider.GetHtml(new HttpItem() { URL = url });
            //解析HTML
            HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result.Html);
            var htmls = doc.DocumentNode.SelectNodes(xpath).Select(r => r.OuterHtml).ToList();
            return Success(htmls);
        }   

    }
}
