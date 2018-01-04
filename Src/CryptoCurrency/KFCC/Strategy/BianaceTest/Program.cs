using KFCC.Exchanges.EBinance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BianaceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            EBinanceExchange exchange = new EBinanceExchange("EspHWtI5WbB3FVUoywxqpE9SkawJKQcrb3q2vu54b428uGdNdIyZlESi29DIBS4n", "BT5OJjq1IQuVmfp8yInJMfiy8aMBdFbRIHSQoB8QyRMucbBQmjWPdI1Plzdz54o3", "rqno1092", "caojia");
            CommonLab.TradePair elfbtc = new CommonLab.TradePair("elf", "btc");
            CommonLab.TradePair elfeth = new CommonLab.TradePair("elf", "eth");
            CommonLab.TradePair ethbtc = new CommonLab.TradePair("eth", "btc");
            exchange.Subscribe(elfbtc, CommonLab.SubscribeTypes.RESTAPI);
            exchange.Subscribe(elfeth, CommonLab.SubscribeTypes.RESTAPI);
            
            exchange.Subscribe(ethbtc, CommonLab.SubscribeTypes.RESTAPI);
            while (true)
            {
                string raw;
                CommonLab.Ticker elfbtcticker = exchange.GetTicker(exchange.GetLocalTradingPairString(elfbtc), out raw);
                CommonLab.Ticker elfethticker = exchange.GetTicker(exchange.GetLocalTradingPairString(elfeth), out raw);
                CommonLab.Ticker ethbtcticker = exchange.GetTicker(exchange.GetLocalTradingPairString(ethbtc), out raw);
                double elf = 1 / elfbtcticker.Sell;
                double eth = elfethticker.Buy * elf;
                double btc = eth*ethbtcticker.Sell;
                string s = $"1 btc change to elf {elf} changeto eth {eth} to btc={btc}";
                Console.WriteLine(s);
                 elf = 1 / elfbtcticker.Sell*0.999;
                 eth = elfethticker.Buy * elf*0.999;
                 btc = eth * ethbtcticker.Sell*0.999;
                Console.BackgroundColor = ConsoleColor.Red;
                s = $"1 btc change to elf {elf} changeto eth {eth} to btc={btc}";
                Console.WriteLine(s);
                Console.BackgroundColor = ConsoleColor.Black;
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
