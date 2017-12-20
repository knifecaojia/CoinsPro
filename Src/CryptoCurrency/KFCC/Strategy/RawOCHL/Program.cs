using KFCC.EBitstamp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace RawOCHL
{
    class Program
    {
        public static Dictionary<string, exchangecahe> Cache = new Dictionary<string, exchangecahe>();
        static KFCC.ExchangeInterface.IExchanges exchangebitstamp = new BitstampExchange("SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa", "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l", "rqno1092", "caojia");
        static KFCC.ExchangeInterface.IExchanges exchangeokex = new KFCC.EOkCoin.OkCoinExchange("a8716cf5-8e3d-4037-9a78-6ad59a66d6c4", "CF44F1C9F3BB23B148523B797B862D4C", "", "");
        static KFCC.EHuobiExchange.HuobiExchange exchangehuobi = new KFCC.EHuobiExchange.HuobiExchange("cbf0909f-7842f68b-8c0db43c-04172", "7e022c00-19e4e4a8-2b3ed1d9-312e0", "0", "caojia");
        static CommonLab.TradePair ltc_btc = new CommonLab.TradePair("ltc", "btc");
        static string input = "";
        //static CommonLab.TradePair bch_btc = new CommonLab.TradePair("bch", "btc");
        //static CommonLab.TradePair btc_usdt = new CommonLab.TradePair("btc", "usdt");
        //static CommonLab.TradePair btc_usd = new CommonLab.TradePair("btc", "usd");
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString() + " Start to collecting data! 1-ok 2-bitstamp 3-huobi");
            input = Console.ReadLine();
            if (input == "1")
            {
                exchangeokex.Subscribe(ltc_btc, CommonLab.SubscribeTypes.WSS);
                //exchangeokex.Subscribe(bch_btc, CommonLab.SubscribeTypes.WSS);
                //exchangeokex.Subscribe(btc_usdt, CommonLab.SubscribeTypes.WSS);
                exchangeokex.TickerEvent += Exchange_TickerEvent;
            }
            else if (input == "2")
            {

                exchangebitstamp.Subscribe(ltc_btc, CommonLab.SubscribeTypes.WSS);
                //exchangebitstamp.Subscribe(bch_btc, CommonLab.SubscribeTypes.WSS);
                //exchangebitstamp.Subscribe(btc_usd, CommonLab.SubscribeTypes.WSS);
                exchangebitstamp.TickerEvent += Exchange_TickerEvent;
            }
            else
            {
                exchangehuobi.Subscribe(ltc_btc, CommonLab.SubscribeTypes.WSS);
                //exchangehuobi.Subscribe(bch_btc, CommonLab.SubscribeTypes.WSS);
                //exchangehuobi.Subscribe(btc_usdt, CommonLab.SubscribeTypes.WSS);
                exchangehuobi.TickerEvent += Exchange_TickerEvent;
            }


            //KFCC.ExchangeInterface.IExchanges exchangebitstamp = new BitstampExchange("SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa", "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l", "rqno1092", "caojia");
            //KFCC.ExchangeInterface.IExchanges exchangeokex = new KFCC.EOkCoin.OkCoinExchange("a8716cf5-8e3d-4037-9a78-6ad59a66d6c4", "CF44F1C9F3BB23B148523B797B862D4C", "", "");
            //KFCC.EHuobiExchange.HuobiExchange exchangehuobi = new KFCC.EHuobiExchange.HuobiExchange("cbf0909f-7842f68b-8c0db43c-04172", "7e022c00-19e4e4a8-2b3ed1d9-312e0", "0", "caojia");






            
          
           
            Cache.Add(exchangebitstamp.Name, new exchangecahe());
            Cache.Add(exchangehuobi.Name, new exchangecahe());
            Cache.Add(exchangeokex.Name, new exchangecahe());
            //Cache[exchangebitstamp.Name].ecahe.Add(exchangebitstamp.GetLocalTradingPairString(ltc_btc),new datacahe());
            //Cache[exchangebitstamp.Name].ecahe.Add(exchangebitstamp.GetLocalTradingPairString(bch_btc), new datacahe());
            //Cache[exchangebitstamp.Name].ecahe.Add(exchangebitstamp.GetLocalTradingPairString(btc_usd), new datacahe());

           // Cache[exchangeokex.Name].ecahe.Add(exchangeokex.GetLocalTradingPairString(ltc_btc), new datacahe());
            //Cache[exchangeokex.Name].ecahe.Add(exchangeokex.GetLocalTradingPairString(bch_btc), new datacahe());
            //Cache[exchangeokex.Name].ecahe.Add(exchangeokex.GetLocalTradingPairString(btc_usdt), new datacahe());

            //Cache[exchangehuobi.Name].ecahe.Add(exchangehuobi.GetLocalTradingPairString(ltc_btc), new datacahe());
            //Cache[exchangehuobi.Name].ecahe.Add(exchangehuobi.GetLocalTradingPairString(bch_btc), new datacahe());
            //Cache[exchangehuobi.Name].ecahe.Add(exchangehuobi.GetLocalTradingPairString(btc_usdt), new datacahe());
            Thread t = new Thread(export);
            t.IsBackground = true;
            t.Start();
            while (Console.ReadLine() != "exit")
            {
                Console.WriteLine("type 'exit' to quit");
            }
        }

        private static void Exchange_TickerEvent(object sender, CommonLab.Ticker t, CommonLab.EventTypes et, CommonLab.TradePair tp)
        {
            Console.WriteLine(DateTime.Now.ToString() + " "+((KFCC.ExchangeInterface.IExchanges)sender).Name + " tp:"+tp.ToString()+" tk:" + t.ToString());
        }
        private static DateTime exporttime;
        private static double ts=0;
        private static void export()
        {
            Thread.Sleep(1000);
            while (true)
            {
                exporttime = DateTime.Now;
                ts = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now.ToUniversalTime());
                Thread t = new Thread(new ParameterizedThreadStart(ExporttoDisk));
                if (input == "1")
                {
                    t.Start(exchangeokex);
                }
                else if (input == "2")
                {

                    t.Start(exchangebitstamp);
                }
                else
                {

                    t.Start(exchangehuobi);
                }
               
              
                Thread.Sleep(1000);
            }
        }
        private void UpdateRawinSec()
        {
            while (true)
            {
                Cache[exchangebitstamp.Name].ecahe[exchangebitstamp.GetLocalTradingPairString(ltc_btc)].tl.Add(exchangebitstamp.SubscribedTradingPairs[exchangebitstamp.GetLocalTradingPairString(ltc_btc)].TradeInfo.t);
                Cache[exchangeokex.Name].ecahe[exchangeokex.GetLocalTradingPairString(ltc_btc)].tl.Add(exchangeokex.SubscribedTradingPairs[exchangeokex.GetLocalTradingPairString(ltc_btc)].TradeInfo.t);
                Cache[exchangehuobi.Name].ecahe[exchangehuobi.GetLocalTradingPairString(ltc_btc)].tl.Add(exchangehuobi.SubscribedTradingPairs[exchangehuobi.GetLocalTradingPairString(ltc_btc)].TradeInfo.t);



                Thread.Sleep(1000);
            }
        }
        private static void ExporttoDisk(object obj)
        {
            KFCC.ExchangeInterface.IExchanges e = (KFCC.ExchangeInterface.IExchanges)obj;
            string year = exporttime.Year.ToString();
            string month = exporttime.Month.ToString();
            string day = exporttime.Day.ToString();
            string path = @"raw/" + e.Name;// + @"/" + year + @"/" + month + @"/";
           
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            foreach (KeyValuePair<string,KFCC.ExchangeInterface.SubscribeInterface> item in e.SubscribedTradingPairs)
            {
                string file = path + @"/" +item.Key+"/"+ year + @"/" + month + @"/"+day + ".txt";
                CommonLab.Log log = new CommonLab.Log(file);
                CommonLab.Ticker t = item.Value.TradeInfo.t.Clone();
                //double ts= CommonLab.TimerHelper.GetTimeStamp(exporttime.AddSeconds(-exporttime.Second).ToUniversalTime());
                t.ExchangeTimeStamp = ts;
                log.RawLog(t.ToOCHLString());
                
            }
        }
    }
    public class exchangecahe
    {
        public Dictionary<string, datacahe> ecahe = new Dictionary<string, datacahe>();
       
    }
    public class datacahe
    {
        public List<CommonLab.Ticker> tl = new List<CommonLab.Ticker>(60);
    }
}
