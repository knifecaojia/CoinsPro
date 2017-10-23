using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            KFCC.ExchangeInterface.IExchanges exchange = KFCC.EBitstamp.BitstampExchange.Create("","","","");
            string r ;
            CommonLab.Ticker t = exchange.GetTicker("btcusd",out r);
            Console.WriteLine(r);

            Thread.Sleep(1000);

            CommonLab.Depth d = exchange.GetDepth("btcusd", out r);
            Console.WriteLine(r);
            Console.ReadKey();
        }
    }
}
