using KFCC.EBitstamp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
namespace RawOCHL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now.ToString() + " Start to collecting data!");
            CommonLab.TradePair ltc_btc = new CommonLab.TradePair("ltc", "btc");
            CommonLab.TradePair bch_btc = new CommonLab.TradePair("bch", "btc");
            CommonLab.TradePair btc_usdt = new CommonLab.TradePair("btc", "usdt");
            CommonLab.TradePair btc_usd = new CommonLab.TradePair("btc", "usd");


            KFCC.ExchangeInterface.IExchanges exchangebitstamp = new BitstampExchange("SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa", "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l", "rqno1092", "caojia");
            KFCC.ExchangeInterface.IExchanges exchangeokex = new KFCC.EOkCoin.OkCoinExchange("a8716cf5-8e3d-4037-9a78-6ad59a66d6c4", "CF44F1C9F3BB23B148523B797B862D4C", "", "");
            KFCC.EHuobiExchange.HuobiExchange exchangehuobi = new KFCC.EHuobiExchange.HuobiExchange("cbf0909f-7842f68b-8c0db43c-04172", "7e022c00-19e4e4a8-2b3ed1d9-312e0", "0", "caojia");

            exchangebitstamp.Subscribe(ltc_btc, CommonLab.SubscribeTypes.WSS);
            exchangebitstamp.Subscribe(bch_btc, CommonLab.SubscribeTypes.WSS);
            exchangebitstamp.Subscribe(btc_usd, CommonLab.SubscribeTypes.WSS);

            exchangeokex.Subscribe(ltc_btc, CommonLab.SubscribeTypes.WSS);
            exchangeokex.Subscribe(bch_btc, CommonLab.SubscribeTypes.WSS);
            exchangeokex.Subscribe(btc_usdt, CommonLab.SubscribeTypes.WSS);

            exchangehuobi.Subscribe(ltc_btc, CommonLab.SubscribeTypes.WSS);
            exchangehuobi.Subscribe(bch_btc, CommonLab.SubscribeTypes.WSS);
            exchangehuobi.Subscribe(btc_usdt, CommonLab.SubscribeTypes.WSS);

            exchangebitstamp.TickerEvent += Exchange_TickerEvent;
            exchangeokex.TickerEvent += Exchange_TickerEvent;
            exchangehuobi.TickerEvent += Exchange_TickerEvent;
        }

        private static void Exchange_TickerEvent(object sender, CommonLab.Ticker t, CommonLab.EventTypes et, CommonLab.TradePair tp)
        {
            Console.WriteLine(DateTime.Now.ToString() + " "+((KFCC.ExchangeInterface.IExchanges)sender).Name + " tp:"+tp.ToString()+" tk:" + t.ToString());
        }
        private DateTime exporttime;
        private void export()
        {
            exporttime = DateTime.Now;

        }
        private void ExporttoDisk(KFCC.ExchangeInterface.IExchanges e)
        {
            string year = exporttime.Year.ToString();
            string month = exporttime.Month.ToString();
            string day = exporttime.Day.ToString();
            string path = @"raw/" + e.Name + @"/" + year + @"/" + month + @"/";
            string file = path + day + ".txt";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            
            foreach (KeyValuePair<string,KFCC.ExchangeInterface.SubscribeInterface> item in e.SubscribedTradingPairs)
            {

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
