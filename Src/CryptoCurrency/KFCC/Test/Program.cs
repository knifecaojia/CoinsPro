using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using KFCC.EBitstamp;
using System.Data;
using KFCC.EBinance;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonLab.TradePair tp = new CommonLab.TradePair("ltc","btc");
            string raw;




            //KFCC.ExchangeInterface.IExchanges exchange = new BitstampExchange("SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa", "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l", "rqno1092", "caojia");
            //exchange.Subscribe(tp, CommonLab.SubscribeTypes.WSS);
            //exchange.TickerEvent += Exchange_TickerEvent;
            //exchange.DepthEvent += Exchange_DepthEvent;

            //exchange.GetAccount(out raw);



            //string r ;
            //CommonLab.Ticker t = exchange.GetTicker("btcusd",out r);
            //Console.WriteLine(r);

            //Thread.Sleep(1000);

            //CommonLab.Depth d = exchange.GetDepth("btcusd", out r);
            //Console.WriteLine(r);
            //测试获取ticker 获取depth
            #region 交易所Okex现货测试

            KFCC.ExchangeInterface.IExchanges exchange = new KFCC.EOkCoin.OkCoinExchange("a8716cf5-8e3d-4037-9a78-6ad59a66d6c4", "CF44F1C9F3BB23B148523B797B862D4C", "", "");
            exchange.Subscribe(tp, CommonLab.SubscribeTypes.WSS);
            //exchange.TradeEvent += Exchange_TradeEvent;
            ////CommonLab.Ticker t=exchange.GetTicker(exchange.GetLocalTradingPairString(tp),out raw);
            exchange.TickerEvent += Exchange_TickerEvent;
            exchange.DepthEvent += Exchange_DepthEvent;
            //CommonLab.Trade[] trades=exchange.GetTrades(exchange.GetLocalTradingPairString(tp), out raw, "54892004");
            //foreach (CommonLab.Trade t in trades)
            //{
            //    Console.WriteLine(t.ToString());
            //}
            //Console.WriteLine(trades.Length.ToString());
            //Console.WriteLine(raw);
            //List<CommonLab.Order> orders = exchange.GetHisOrders(exchange.GetLocalTradingPairString(tp));
            //orders = orders.Distinct(new CommonLab.List_Order_DistinctBy_OrderID()).ToList();
            //DataTable dt = CommonLab.Utility.ToDataTable(orders);
            //CommonLab.CsvHelper.SaveCSV(dt, "orders.csv");
            #endregion
            //测试获取账户信息
            //Console.WriteLine(raw);
            //int orderid=exchange.Buy(exchange.GetLocalTradingPairString(tp), t.Sell * 0.9, 1);
            //Console.WriteLine("Order ID:" + orderid);
            //exchange.CancelOrder("41055418", exchange.GetLocalTradingPairString(tp), out raw);
            //Console.WriteLine(raw);
            //exchange.GetAccount(out raw);
            //Console.WriteLine(raw);
            #region 交易所huobi测试

            //KFCC.EHuobiExchange.HuobiExchange exchange = new KFCC.EHuobiExchange.HuobiExchange("cbf0909f-7842f68b-8c0db43c-04172", "7e022c00-19e4e4a8-2b3ed1d9-312e0", "0", "caojia");
            //exchange.Subscribe(tp, CommonLab.SubscribeTypes.WSS);
            //exchange.TickerEvent += Exchange_TickerEvent;
            //exchange.DepthEvent += Exchange_DepthEvent;
            //exchange.GetAccount(out raw);
            //Console.WriteLine(exchange.Account.ToString(true));
            //CommonLab.Ticker t = exchange.GetTicker(exchange.GetLocalTradingPairString(tp), out raw);
            //Console.WriteLine(raw);

            //Console.WriteLine(exchange.GetOrders(exchange.GetLocalTradingPairString(tp)));
            //交易测试
            //exchange.Buy(exchange.GetLocalTradingPairString(tp), t.Sell * 0.9, 0.001);
            //订单状态查询
            // exchange.GetOrderStatus("209244267", exchange.GetLocalTradingPairString(tp),out raw);
            //exchange.CancelOrder("209244267", exchange.GetLocalTradingPairString(tp), out raw);
            //exchange.TickerEvent += Exchange_TickerEvent;
            //exchange.DepthEvent += Exchange_DepthEvent;
            //exchange.Subscribe(tp, CommonLab.SubscribeTypes.WSS);

            //CommonLab.Ticker t = exchange.GetTicker(exchange.GetLocalTradingPairString(tp), out raw);
            // Console.WriteLine(DateTime.Now.ToString() + " te:{0},{1}", t.ExchangeTimeStamp, t.ToString());
            #endregion



            #region Bitstamp test
            //KFCC.ExchangeInterface.IExchanges exchange = new BitstampExchange("SkDFzpEwvEHyXl45Bvc0nlHXPeP3e1Wa", "hIW0CYUK1NvbZR73N5rPDO0yly4GgK3l", "rqno1092", "caojia");
            //exchange.Subscribe(tp, CommonLab.SubscribeTypes.WSS);
            //exchange.TickerEvent += Exchange_TickerEvent;
            //exchange.DepthEvent += Exchange_DepthEvent;
            #endregion



            #region biance测试
            //            EspHWtI5WbB3FVUoywxqpE9SkawJKQcrb3q2vu54b428uGdNdIyZlESi29DIBS4n
            //Secret:
            //BT5OJjq1IQuVmfp8yInJMfiy8aMBdFbRIHSQoB8QyRMucbBQmjWPdI1Plzdz54o3
            //EBinanceExchange exchange = new KFCC.EBinance.EBinanceExchange("EspHWtI5WbB3FVUoywxqpE9SkawJKQcrb3q2vu54b428uGdNdIyZlESi29DIBS4n", "BT5OJjq1IQuVmfp8yInJMfiy8aMBdFbRIHSQoB8QyRMucbBQmjWPdI1Plzdz54o3", "rqno1092", "caojia");
            //exchange.proxy = new CommonLab.Proxy("127.0.0.1", 8888);
            //exchange.Subscribe(tp, CommonLab.SubscribeTypes.RESTAPI);
            //exchange.GetTicker(exchange.GetLocalTradingPairString(tp), out raw);
            //exchange.TickerEvent += Exchange_TickerEvent;
            //exchange.DepthEvent += Exchange_DepthEvent;
            //exchange.TradeEvent += Exchange_TradeEvent;
            //CommonLab.Trade[] trades= exchange.GetTrades(exchange.GetLocalTradingPairString(tp), out raw);
            //CommonLab.Ticker t = exchange.GetTicker(exchange.GetLocalTradingPairString(tp), out raw);
            //exchange.GetAccount(out raw);
           // Console.Write(exchange.Account.ToString(true));
            //int orderid=exchange.Sell(exchange.GetLocalTradingPairString(tp), t.Buy +0.000015, 0.1);
            //exchange.GetAccount(out raw);
            //List<CommonLab.Order> orders = exchange.GetOrdersStatus(exchange.GetLocalTradingPairString(tp), out raw);
           // foreach (CommonLab.Order o in orders)
            //{
           //     if (o.Status == CommonLab.OrderStatus.ORDER_STATE_PENDING||1==1)
           //     {
           //         exchange.CancelOrder(o.Id, exchange.GetLocalTradingPairString(tp),out raw);
           //     }
           // }
           // CommonLab.Order order = exchange.GetOrderStatus("13081100", exchange.GetLocalTradingPairString(tp), out raw);
           // Console.WriteLine(raw);
            #endregion
            Console.ReadKey();
        }



        private static void Exchange_TradeEvent(object sender, CommonLab.Trade t, CommonLab.EventTypes et, CommonLab.TradePair tp)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(DateTime.Now.ToString() + " trade-e:{0},{1}", t.ExchangeTimeStamp, t.ToString());

        }

        private static void Exchange_DepthEvent(object sender, CommonLab.Depth d, CommonLab.EventTypes et, CommonLab.TradePair tp)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(DateTime.Now.ToString()+" de:{0},{1}", d.ExchangeTimeStamp, d.ToString(5));
        }

        private static void Exchange_TickerEvent(object sender, CommonLab.Ticker t, CommonLab.EventTypes et, CommonLab.TradePair tp)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(DateTime.Now.ToString() + " te:{0},{1}", t.ExchangeTimeStamp, t.ToString());
        }
    }
}
