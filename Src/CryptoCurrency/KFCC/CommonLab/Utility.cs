using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public static class Utility
    {
        public static string GetHttpContent(string url, Proxy p = null)
        {
            HttpResult hr = new HttpResult();
            HttpHelper hh = new HttpHelper();
            HttpItem hi = new HttpItem();
            string _url = url ;
            hi.URL = _url;
            if (p != null)
            {
                hi.ProxyIp = p.IP + ":" + p.Port;
            }
            hr = hh.GetHtml(hi);
            return  hr.Html;
        }
    }
}
