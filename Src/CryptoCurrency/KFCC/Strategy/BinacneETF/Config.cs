using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinacneETF
{
    public static class Config
    {
        public static string ApiUrl = "https://api.binance.com";
        public static string ExchangeInformation = "/api/v1/exchangeInfo";
        public static CommonLab.Proxy Proxy { get { return GetProxy(); } set { value = GetProxy(); } }
        static CommonLab.Proxy proxy;
        public static CommonLab.Proxy GetProxy()
        {
            proxy = new CommonLab.Proxy("127.0.0.1", 1080);
            return proxy;
        }
        public static ExchangeInfo Exchange;
    }
}
