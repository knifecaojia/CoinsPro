using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using KFCC.EBitstamp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonLab.TradePair tp = new CommonLab.TradePair("btc","USD");
           
            KFCC.ExchangeInterface.IExchanges exchange = new BitstampExchange("SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa", "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l", "rqno1092", "caojia");
            //exchange.Subscribe(tp, CommonLab.SubscribeTypes.WSS);
            //exchange.TickerEvent += Exchange_TickerEvent;
            //exchange.DepthEvent += Exchange_DepthEvent;
            string raw;
            exchange.GetAccount(out raw);

            Console.WriteLine(raw);
            
            //string r ;
            //CommonLab.Ticker t = exchange.GetTicker("btcusd",out r);
            //Console.WriteLine(r);

            //Thread.Sleep(1000);

            //CommonLab.Depth d = exchange.GetDepth("btcusd", out r);
            //Console.WriteLine(r);
            Console.ReadKey();
        }

        private static void Exchange_DepthEvent(object sender, CommonLab.Depth d, CommonLab.EventTypes et, string tradingpair)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(DateTime.Now.ToString()+" de:{0},{1}", d.ExchangeTimeStamp, d.ToString(5));
        }

        private static void Exchange_TickerEvent(object sender, CommonLab.Ticker t, CommonLab.EventTypes et, string tradingpair)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(DateTime.Now.ToString() + " te:{0},{1}", t.ExchangeTimeStamp, t.ToString());
        }
    }
}
