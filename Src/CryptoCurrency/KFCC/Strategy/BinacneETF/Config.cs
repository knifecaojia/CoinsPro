using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
namespace BinacneETF
{
    public static class Config
    {
        public static bool IsStrategyWorking = false;//策略是否运行？
        public static DateTime StrategyStartTime;
        public static StrategyConfig strategyConfig = StrategyConfig.LoadConfig();
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
        public delegate void UpdateServiceStatusEventHandle(System.Drawing.Color c, string Msg);
        public static event UpdateServiceStatusEventHandle UpdateServiceStatusEvent;
        public static void RaiseUpdateConsoleEvent(System.Drawing.Color c, string Msg) { if(UpdateConsoleEvent!=null) UpdateConsoleEvent(c, Msg); Config.Log.log(Msg); }
        public static void RaiseUpdateServiceStatusEvent(System.Drawing.Color c, string Msg) { if (UpdateServiceStatusEvent != null) UpdateServiceStatusEvent(c, Msg); Config.Log.log(Msg); }
        public static MyEventClass Events = MyEventClass.Instance;
        public delegate void UpdateTickersHandle();
        public delegate void UpdateTradeHandle(CommonLab.Trade t);
        public static event UpdateTradeHandle UpdateTradeEvent;
        public static event UpdateTickersHandle UpdateTickerEvent;
        public static void RaiseUpdateTickerEvent() { if (UpdateTickerEvent != null) UpdateTickerEvent(); }
        public static void RaiseUpdateTradeEvent(CommonLab.Trade t) { if (UpdateTradeEvent != null) UpdateTradeEvent(t); }

        #region ReadCoinConfig
        #endregion

    }
    [Serializable]
    public  class StrategyConfig
    {
        public List<string> SelectedSymbols { get; set; }
        public double InvestForce { get; set; }
        public StrategyConfig()
        {
            SelectedSymbols = new List<string>();
        }
        public static StrategyConfig LoadConfig()
        {
            if (File.Exists(@"Config\Coins.config"))
            {
                string json = "";
                using (StreamReader sr = new StreamReader(@"Config\Coins.config"))
                {
                    json = sr.ReadToEnd();
                }

                StrategyConfig config=Newtonsoft.Json.JsonConvert.DeserializeObject<StrategyConfig>(json);
            
            return config;
            }
            return new StrategyConfig();
        }
        public void SaveConfig(StrategyConfig config)
        {
            if (File.Exists(@"Config\Coins.config"))
            {
                File.Delete(@"Config\Coins.config");
            }
            string json = "";
            json = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            using (StreamWriter sw = new StreamWriter(@"Config\Coins.config"))
            {
                sw.Write(json);
            }
        }
        public bool IsSymbolInETF(string symbol)
        {
            if (SelectedSymbols.Where(item => item == symbol).ToArray().Count() > 0)
            {
                return true;
            }
            return false;

        }

        public void UpdateSymbol(string symbol, bool isadd)
        {
            bool inflag = false;
            foreach (var item in SelectedSymbols)
            {
                if (item == symbol)
                {
                    inflag = true;
                    if (!isadd)
                    {
                        SelectedSymbols.Remove(item);
                        break;
                    }
                }
            }
            if ((!inflag) && isadd)
            {
                SelectedSymbols.Add(symbol);
            }
        }
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
