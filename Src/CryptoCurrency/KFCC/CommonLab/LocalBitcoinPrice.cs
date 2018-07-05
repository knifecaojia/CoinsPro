using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public static class LocalBitcoinPrice
    {
        /// <summary>
        /// 取得localbitcoin 法币对价
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static double getLocalBitcoinPrice(string symbol, CommonLab.Proxy proxy)
        {
            
            var url = "https://localbitcoins.com/instant-bitcoins/?action=sell&country_code=CN&amount=&currency="+symbol+"&place_country=CN&online_provider=ALL_ONLINE&find-offers=%E6%90%9C%E7%B4%A2";
            var doc = new HtmlDocument() ;
             var web = new HtmlWeb();
            try
            {
                if (proxy != null)
                {
                    doc = web.Load(url, proxy.IP, proxy.Port, "", "");
                }
                else
                {
                    doc = web.Load(url);
                }

                double totalprice = 0;
                double count = 0;
                for (int i = 2; i <= 16; i++)
                {
                    try
                    {
                        var value = doc.DocumentNode.SelectNodes("/html/body/div[1]/table/tr[" + i + "]/td[3]").First().InnerText;
                        string valuestr = value.ToString();
                        valuestr = valuestr.Trim();
                        valuestr = valuestr.Replace(symbol, "");
                        totalprice += Convert.ToDouble(valuestr);
                        count++;

                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }
                if (count > 0)
                {
                    double avgprice = totalprice / count;
                    return Math.Round(avgprice);
                }
                else
                    return 1;
            }
            catch
            {
                return 1;
            }
        }
    }
}
