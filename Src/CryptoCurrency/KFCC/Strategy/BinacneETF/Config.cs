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
        public static CommonLab.Log Log = new CommonLab.Log(@"log\" + DateTime.Now.ToString("yyyyMMdd")+".txt");
        public delegate void UpdateConsoleEventHandle(System.Drawing.Color c, string Msg);
        public static event UpdateConsoleEventHandle UpdateConsoleEvent;
        public static void RaiseUpdateConsoleEvent(System.Drawing.Color c, string Msg) { if(UpdateConsoleEvent!=null) UpdateConsoleEvent(c, Msg); Config.Log.log(Msg); }
        public static MyEventClass Events = MyEventClass.Instance;
        public delegate void UpdateTickersHandle();
        public static event UpdateTickersHandle UpdateTickerEvent;
        public static void RaiseUpdateTickerEvent() { if (UpdateTickerEvent != null) UpdateTickerEvent(); }

    }
    public sealed class MyEventClass
    {
        static MyEventClass instance = null;
        static readonly object padlock = new object();
        public event Config.UpdateConsoleEventHandle UpdateConsoleEvent;
        public MyEventClass()
        {
            
    }

        public static MyEventClass Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new MyEventClass();
                    }
                    return instance;
                }
            }
        }
    }
}
