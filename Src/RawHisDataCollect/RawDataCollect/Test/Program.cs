using KFSoft.CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpResult hr = new HttpResult();
            HttpHelper hh = new HttpHelper();
            HttpItem hi = new HttpItem();
            string url = @"https://min-api.cryptocompare.com/data/histohour?fsym=BTC&tsym=USD&limit=60&e=Bitstamp&toTs=" + TimeHelper.GetTimeStamp(DateTime.Now); ;
            url = @"http://2017.ip138.com/ic.asp";
            hi.URL = url;
            hi.ProxyIp = "127.0.0.1:1080";
            hr = hh.GetHtml(hi);
            string rawjson = hr.Html;
            Console.WriteLine(rawjson);
            Console.ReadKey();
        }
    }
}
