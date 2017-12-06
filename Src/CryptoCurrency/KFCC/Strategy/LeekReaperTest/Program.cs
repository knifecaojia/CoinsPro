using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using KFCC.ExchangeInterface;
using System.Threading;

namespace LeekReaperTest
{
    class Program
    {
        static CommonLab.Log log;
        static IExchanges exchange;
        static TradePair tradepair;
        static double Vol;
        static double bidPrice;
        static double askPrice;
        static double Price;
        static int LastTradeID;
        static Depth OrderBook;
        static List<double> prices;
        static Account account;
        static double FromSymbolFree;
        static double ToSymbolFree;
        static DateTime LastBalanceTime;
        static string FromSymbol, ToSymbol;
        const int BalanceTimeMinsec = 10000;
        const double MinTradeAmount = 0.01;//最小交易下单
        static Thread BalanceThread;
        static Thread CancelOrderThread;
        static Account startaccount = null;
        static void Main(string[] args)
        {
            log = new Log(DateTime.Now.ToString("yyyyMMddHHmmss")+".txt");
            prices = new List<double>(15);
            for (int i = 0; i < 15; i++)
            {
                prices.Add(0);
            }
            LastBalanceTime = DateTime.Now;
            FromSymbol = "LTC";
            ToSymbol = "BTC";
            tradepair = new TradePair(FromSymbol, ToSymbol);
            exchange = new KFCC.EOkCoin.OkCoinExchange("a8716cf5-8e3d-4037-9a78-6ad59a66d6c4", "CF44F1C9F3BB23B148523B797B862D4C", "", "");
            exchange.Subscribe(tradepair, CommonLab.SubscribeTypes.RESTAPI);
            
            MainLoop();
            Console.ReadKey();
        }
        static void UpdateTrades()
        {
            string raw = "";
            Trade[] trades = exchange.GetTrades(exchange.GetLocalTradingPairString(tradepair), out raw);
            Vol = 0.7 * Vol;
            if (trades != null && trades.Length > 0)
            {
                double sum = 0;
                foreach (Trade t in trades)
                {
                    if (Convert.ToInt32(t.TradeID) > LastTradeID)
                    {
                        sum += t.Amount;
                    }
                }
                Vol = Vol + 0.3 * sum;
                LastTradeID = Convert.ToInt32(trades[trades.Length - 1].TradeID);
                Price = Convert.ToDouble(trades[trades.Length - 1].Price);
            }
        }
        static void UpdateOrderBook()
        {
            string raw;
            OrderBook = exchange.GetDepth(exchange.GetLocalTradingPairString(tradepair), out raw);
            bidPrice = OrderBook.Bids[0].Price * 0.618 + OrderBook.Asks[0].Price * 0.382;//需要调整价格
            askPrice = OrderBook.Bids[0].Price * 0.382 + OrderBook.Asks[0].Price * 0.618;

            //prices = prices[1.. - 1] + [(
            //    (orderBook.bids[0].limitPrice + orderBook.asks[0].limitPrice) / 2 * 0.7 +
            //    (orderBook.bids[1].limitPrice + orderBook.asks[1].limitPrice) / 2 * 0.2 +
            //    (orderBook.bids[2].limitPrice + orderBook.asks[2].limitPrice) / 2 * 0.1)]
            prices.RemoveAt(0);
            prices.Add(((OrderBook.Bids[0].Price + OrderBook.Asks[0].Price) / 2 * 0.7) + ((OrderBook.Bids[1].Price + OrderBook.Asks[1].Price) / 2 * 0.2) + ((OrderBook.Bids[2].Price + OrderBook.Asks[2].Price) / 2 * 0.1));
            Console.WriteLine("OrderBook delay:" + OrderBook.Delay+"ms");
        }

        static void BalanceAccount()
        {
            string raw;

            while (true)
            {
                try

                {
                    account = exchange.GetAccount(out raw);
                    if (startaccount == null)
                        startaccount = account.Clone();
                    if (account == null)
                        return;
                    if (OrderBook == null)
                        return;
                    FromSymbolFree = account.Balances[FromSymbol.ToLower()].balance;
                    ToSymbolFree = account.Balances[ToSymbol.ToLower()].balance;
                    double p = FromSymbolFree / (ToSymbolFree / prices[prices.Count - 1] + FromSymbolFree);
                    if (p < 0.4)
                    {
                        log.log("Balance：" + FromSymbol + " / (" + FromSymbol + " + " + ToSymbol + ") Pencent is " + p.ToString() + " < 0.4 Start BUY.");
                        Console.WriteLine(FromSymbol + "/(" + FromSymbol + "+" + ToSymbol + ") Pencent is" + p.ToString() + "<0.4 Start BUY.");
                        double p1, p2, p3;
                        p1 = (OrderBook.Asks[0].Price * 0.3 + OrderBook.Bids[0].Price * 0.7);
                        p2 = (OrderBook.Asks[0].Price * 0.5 + OrderBook.Bids[0].Price * 0.5);
                        p3 = (OrderBook.Asks[0].Price * 0.7 + OrderBook.Bids[0].Price * 0.3);
                        if (p1 >= OrderBook.Asks[0].Price)
                            p1 = OrderBook.Bids[0].Price;
                        if (p2 >= OrderBook.Asks[0].Price)
                            p2 = OrderBook.Bids[0].Price;
                        if (p3 >= OrderBook.Asks[0].Price)
                            p3 = OrderBook.Bids[0].Price;
                        int oid = exchange.Buy(exchange.GetLocalTradingPairString(tradepair), p1, 0.01);
                        log.log("Trading:Balance BUY price:" + p1 + " orderbook sell at:"+OrderBook.Asks[0].Price+" amount:" + 0.01 + " orderid:" + oid);
                        oid = exchange.Buy(exchange.GetLocalTradingPairString(tradepair), p2, 0.01);
                        log.log("Trading:Balance BUY price:" + p2 + " orderbook sell at:" + OrderBook.Asks[0].Price + " amount:" + 0.01 + " orderid:" + oid);
                        oid = exchange.Buy(exchange.GetLocalTradingPairString(tradepair), p3, 0.01);
                        log.log("Trading:Balance BUY price:" + p3 + " orderbook sell at:" + OrderBook.Asks[0].Price + " amount:" + 0.01 + " orderid:" + oid);
                    }
                    if (p > 0.6)
                    {
                        log.log("Balance：" + FromSymbol + "/(" + FromSymbol + "+" + ToSymbol + ") Pencent is" + p.ToString() + ">0.6 Start SELL.");
                        Console.WriteLine(FromSymbol + "/(" + FromSymbol + "+" + ToSymbol + ") Pencent is" + p.ToString() + ">0.6 Start SELL.");
                        double p1, p2, p3;
                        p3 = (OrderBook.Asks[0].Price * 0.3 + OrderBook.Bids[0].Price * 0.7);
                        p2 = (OrderBook.Asks[0].Price * 0.5 + OrderBook.Bids[0].Price * 0.5);
                        p1 = (OrderBook.Asks[0].Price * 0.7 + OrderBook.Bids[0].Price * 0.3);
                        if (p1 <= OrderBook.Bids[0].Price)
                            p1 = OrderBook.Asks[0].Price;
                        if (p2 >= OrderBook.Bids[0].Price)
                            p2 = OrderBook.Asks[0].Price;
                        if (p3 >= OrderBook.Bids[0].Price)
                            p3 = OrderBook.Asks[0].Price;
                        int oid = exchange.Sell(exchange.GetLocalTradingPairString(tradepair), p1, 0.01);
                        log.log("Trading:Balance SELL price:" + p1 + " orderbook buy at:" + OrderBook.Bids[0].Price + " amount:" + 0.01 + " orderid:" + oid);
                        oid = exchange.Sell(exchange.GetLocalTradingPairString(tradepair), p2, 0.01);
                        log.log("Trading:Balance SELL price:" + p2 + " orderbook buy at:" + OrderBook.Bids[0].Price + " amount:" + 0.01 + " orderid:" + oid);
                        oid = exchange.Sell(exchange.GetLocalTradingPairString(tradepair), p3, 0.01);
                        log.log("Trading:Balance SELL price:" + p3 + " orderbook buy at:" + OrderBook.Bids[0].Price + " amount:" + 0.01 + " orderid:" + oid);
                    }
                    double snet = startaccount.Balances[FromSymbol.ToLower()].balance + startaccount.Balances[ToSymbol.ToLower()].balance / OrderBook.Asks[0].Price;
                    double nnet= account.Balances[FromSymbol.ToLower()].balance + account.Balances[ToSymbol.ToLower()].balance / OrderBook.Asks[0].Price;
                    double cp = nnet / snet;
                    cp = cp * 100;
                    log.log("Start " + FromSymbol + ": " + startaccount.Balances[FromSymbol.ToLower()].balance + " | " + ToSymbol + ": " + startaccount.Balances[ToSymbol.ToLower()].balance + "Net:" + snet + "Now " + FromSymbol + ": " + account.Balances[FromSymbol.ToLower()].balance + " | " + ToSymbol + ": " + account.Balances[ToSymbol.ToLower()].balance + " Net:" + nnet + " " + cp + "%");
                    Console.WriteLine("Start " + FromSymbol + ": " + startaccount.Balances[FromSymbol.ToLower()].balance + " | " + ToSymbol + ": " + startaccount.Balances[ToSymbol.ToLower()].balance + "Net:"+snet+  "Now " + FromSymbol + ": " + account.Balances[FromSymbol.ToLower()].balance + " | " + ToSymbol + ": " + account.Balances[ToSymbol.ToLower()].balance+" Net:"+nnet+" "+cp+"%");
                    Thread.Sleep(BalanceTimeMinsec);
                    exchange.CancelAllOrders();
                }
                catch (Exception e)
                {
                    log.log("error:" + e.Message);
                }
            }
        
        }
        static void CancelOldOrders()
        {
            string raw;
            List<Order> Orders = ((KFCC.EOkCoin.OkCoinExchange)exchange).GetOrdersStatus(exchange.GetLocalTradingPairString(tradepair), out raw);
            if (Orders == null || Orders.Count == 0)
                return;
            foreach (Order o in Orders)
            {
                if ((DateTime.Now - o.CreatDate.ToLocalTime()).TotalSeconds > 20)
                {
                    exchange.CancelOrder(o.Id, exchange.GetLocalTradingPairString(tradepair), out raw);
                }
            }
            Thread.Sleep(60000);
        }
        static int numTick = 0;
        static double BurstThresholdPct = 0.00005;
        const double BurstThresholdVol = 10;
        static Order o;
        static void MainLoop()
        {
            CancelOrderThread = new Thread(CancelOldOrders);
            CancelOrderThread.IsBackground = true;
            CancelOrderThread.Start();

            BalanceThread = new Thread(BalanceAccount);
            BalanceThread.IsBackground = true;
            BalanceThread.Start();
            for (numTick = 0; ; numTick++)
            {
                UpdateTrades();
                UpdateOrderBook();
               
                bool bull = false, bear = false;
                double tradeamount = 0;
                double burstPrice = prices[prices.Count - 1] * BurstThresholdPct;
                //Console.WriteLine("Tick:" + numTick + ",LastPrice:" + prices[prices.Count - 1] + "burstPrice:" + burstPrice.ToString("F12"));

                double maxlastsix = 0, maxlastfive = 0;
                for (int i = 6; i > 1; i--)
                {
                    if (maxlastsix < prices[prices.Count - i])
                        maxlastsix = prices[prices.Count - i];
                    if (i > 2)
                    {
                        if (maxlastfive < prices[prices.Count - i])
                            maxlastfive = prices[prices.Count - i];
                    }
                }
                double diff = prices[prices.Count - 1] - maxlastsix;
                double difffive = prices[prices.Count - 1] - maxlastfive;
                if (diff!=0)
                    Console.WriteLine("maxlastsix:{0},maxlastfive{1},Diff{2}", maxlastsix, maxlastfive, diff.ToString("F12"));
                if (numTick > 2 && (diff > burstPrice || (difffive > burstPrice && prices[prices.Count - 1] > prices[prices.Count - 2])))
                {
                    bull = true;
                    tradeamount = ToSymbolFree / bidPrice * 0.99;
                }
                else if (numTick > 2 && (diff < burstPrice || (difffive < burstPrice && prices[prices.Count - 1] < prices[prices.Count - 2])))
                {

                    bear = true;
                    tradeamount = FromSymbolFree;
                }
                if (Vol < BurstThresholdVol)
                {
                    tradeamount *= Vol / BurstThresholdVol;
                }

                if (numTick < 5)
                {
                    tradeamount *= 0.8;
                }

                if (numTick < 10)
                {
                    tradeamount *= 0.8;
                }
                if ((!bull && !bear) || tradeamount < MinTradeAmount)
                {
                    Thread.Sleep(300);
                    continue; 
                }

                double[] da = prices.ToArray();
                double[] da1 = new double[14];
                Array.Copy(da, 0, da1, 0, 14);

                if (bull && prices[prices.Count - 1] < da1.Max()) tradeamount *= 0.90;
                if (bear && prices[prices.Count - 1] > da1.Min()) tradeamount *= 0.90;
                if (Math.Abs(prices[prices.Count - 1] - prices[prices.Count - 2]) > burstPrice * 2) tradeamount *= 0.90;
                if (Math.Abs(prices[prices.Count - 1] - prices[prices.Count - 2]) > burstPrice * 3) tradeamount *= 0.90;
                if (Math.Abs(prices[prices.Count - 1] - prices[prices.Count - 2]) > burstPrice * 4) tradeamount *= 0.90;
                if (OrderBook.Asks[0].Price - OrderBook.Bids[0].Price > burstPrice * 2) tradeamount *= 0.90;
                if (OrderBook.Asks[0].Price - OrderBook.Bids[0].Price > burstPrice * 3) tradeamount *= 0.90;
                if (OrderBook.Asks[0].Price - OrderBook.Bids[0].Price > burstPrice * 4) tradeamount *= 0.90;

                double tradePrice;
                if (bull)
                    tradePrice = bidPrice;
                else
                    tradePrice = askPrice;
                while (tradeamount >= MinTradeAmount)
                {
                    string raw;
                    int orderid=0;
                    try
                    {
                        if (bull&&tradeamount<account.Balances[FromSymbol.ToLower()].available)
                        {
                            orderid = exchange.Buy(exchange.GetLocalTradingPairString(tradepair), bidPrice, tradeamount);
                            log.log("Trading:BUY price:" + bidPrice + " amount:" + tradeamount + " orderid:" + orderid);
                        }
                        else if(tradeamount < account.Balances[ToSymbol.ToLower()].available/ askPrice)
                        {
                            orderid = exchange.Sell(exchange.GetLocalTradingPairString(tradepair), askPrice, tradeamount);
                            log.log("Trading:SELL price:" + askPrice + " amount:" + tradeamount + " orderid:" + orderid);
                        }
                    }
                    catch(Exception e)
                    {
                        log.log("error:" + e.Message);
                    }

                    Thread.Sleep(200);
                    if (orderid > 0)
                    {
                        while (true)
                        {
                            o = exchange.GetOrderStatus(orderid.ToString(), exchange.GetLocalTradingPairString(tradepair), out raw);
                            if (o != null)
                            {
                                if (o.Status == OrderStatus.ORDER_STATE_PENDING)
                                {
                                    exchange.CancelOrder(o.Id.ToString(), exchange.GetLocalTradingPairString(tradepair), out raw);
                                    log.log("OrderCancel: orderid:" + orderid);
                                   
                                }
                                else
                                    break;
                            }
                           
                        }
                        log.log("OrderDetails: orderid:" + orderid+ " price:"+o.Price+" amount:"+ o.Amount+" dealamount:"+o.DealAmount+" staus:"+o.Status);
                        tradeamount -= o.DealAmount;
                        tradeamount *= 0.9;
                        if (o.Status == OrderStatus.ORDER_STATE_CANCELED)
                        {
                            UpdateOrderBook();
                            while (bull && bidPrice - tradePrice > +0.1)
                            {
                                tradeamount *= 0.99;
                                tradePrice += 0.1;
                            }
                            while (bear && askPrice - tradePrice < -0.1)
                            {
                                tradeamount *= 0.99;
                                tradePrice -= 0.1;
                            }
                        }
                    }

           
                }
                numTick = 0;
            }
        }
    }
}
