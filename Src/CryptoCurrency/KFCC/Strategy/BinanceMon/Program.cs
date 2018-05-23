using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BinacneETF;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace BinanceMon
{
    class Program
    {
        static TradeDataManage tradeDataManage;
        static Thread GetCnyPrice;
        static Thread ReConnectThread;//币安24小时切断wss连接，需要重连
        static DateTime LastConnectTimeHour;//上次连接时间
        //上述计价信息通过线程每分钟更新一次
        static List<string> Symbols;
        static CommonLab.Log log = new CommonLab.Log("loginfo.txt");
        static void Main(string[] args)
        {
            tradeDataManage = new TradeDataManage(log);
            Symbols = new List<string>();
            Console.Write("Monitoring:");
            log.log("Start");
            using (System.IO.TextReader tr = new System.IO.StreamReader("Symbols.txt"))
            {
                string line = tr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    Symbols.Add(line);
                    Console.Write(line+"  ");
                    line= tr.ReadLine();
                }
            }
            Console.WriteLine();
            BinacneETF.Config.Exchange = new ExchangeInfo();
            Config.Exchange.GetExchangeInfo();
            GetCnyPrice = new Thread(new ThreadStart(GetCNY));
            GetCnyPrice.IsBackground = true;
            GetCnyPrice.Start();
            BinacneETF.Config.UpdateTradeEvent += Config_UpdateTradeEvent;
            BinacneETF.Config.UpdateTickerEvent += Config_UpdateTickerEvent;
           
            BinacneETF.Config.Exchange.Start();//收集ticker
            Thread.Sleep(1000);
            LastConnectTimeHour = DateTime.Now;
            BinacneETF.Config.Exchange.StartCollectTrade(Symbols);//收集交易信息
            tradeDataManage.Start();
            ReConnectThread = new Thread(new ThreadStart(CheckReconnect));
            ReConnectThread.IsBackground = true;
            ReConnectThread.Start();
            while (!(Console.ReadKey().KeyChar.ToString() == "q"))
            {

            }
                return;
        }
        private static void CheckReconnect()
        {
            while (true)
            {
                if ((DateTime.Now - LastConnectTimeHour).TotalHours > 2)
                {
                    log.log("Reconnect Binance Wss Services.");
                    Config.Exchange.Stop();
                    Thread.Sleep(300);
                    BinacneETF.Config.Exchange.StartCollectTrade(Symbols);//收集交易信息
                    tradeDataManage.Start();
                    LastConnectTimeHour = DateTime.Now;

                }
                Thread.Sleep(1000 * 60);
            }
        }
        private static void GetCNY()
        {
            while (true)
            {
                Console.WriteLine("收集人民币价格指数...");

                tradeDataManage.CNYPrice = CommonLab.LocalBitcoinPrice.getLocalBitcoinPrice("CNY", BinacneETF.Config.Proxy);
                Console.WriteLine("CNY:"+ tradeDataManage.CNYPrice);
                log.log("收集人民币价格指数...CNY:" + tradeDataManage.CNYPrice);
                Thread.Sleep(10 * 60 * 1000);//每十分钟收集一次
            }
        }
        private static void Config_UpdateTickerEvent()
        {
            var ethbtc = BinacneETF.Config.Exchange.Symbols.Where(symbol => symbol.Symbol.ToLower() == "ethbtc").ToList();
            var bnbbtc = BinacneETF.Config.Exchange.Symbols.Where(symbol => symbol.Symbol.ToLower() == "bnbbtc").ToList();
            var btcusdt = BinacneETF.Config.Exchange.Symbols.Where(symbol => symbol.Symbol.ToLower() == "btcusdt").ToList();
            if (ethbtc.Count() > 0)
            {
                tradeDataManage.ETH2BTCPrice= ethbtc[0].Ticker.Last;
            }
            if (bnbbtc.Count() > 0)
            {
                tradeDataManage.BNB2BTCPrice = bnbbtc[0].Ticker.Last;
            }
            if (btcusdt.Count() > 0)
            {
                tradeDataManage.USDT2BTCPrice =1/btcusdt[0].Ticker.Last;
            }

        }

        private static void Config_UpdateTradeEvent(string symbol,CommonLab.Trade t)
        {
            try
            {

                tradeDataManage.AddTrade(symbol, t);
            }
            catch (Exception e)
            {
                log.log(e.Message + e.StackTrace);
            }
            
        }
    }
    public class TradeDataManage
    {
        public double CNYPrice = 1;//人民币计价 每十分钟刷新一次；
        public  double ETH2BTCPrice;//eth的btc计价
        public  double BNB2BTCPrice;//bnb的btc 计价
        public  double USDT2BTCPrice;//usdt的btc计价
        static object LockObject = new object();
        static CommonLab.Log log;
        private Thread UpdateRedisThread;
        public TradeDataManage(CommonLab.Log _log)
        {
            log = _log;
        }
        public double GetLegalCurrency(string symbol)
        {
            symbol = symbol.ToLower();
            if (symbol.Substring(symbol.Length - 4) == "usdt")
            {
                return USDT2BTCPrice * CNYPrice;
            }
            else if (symbol.Substring(symbol.Length - 3) == "eth")
            {
                return ETH2BTCPrice * CNYPrice;
            }
            else if (symbol.Substring(symbol.Length - 3) == "bnb")
            {
                return BNB2BTCPrice * CNYPrice;
            }
            else
            {
                return CNYPrice;
            }
        }
        public Dictionary<string, CommonLab.TradePerMin> Data = new Dictionary<string, CommonLab.TradePerMin>();
        private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("10.18.20.108:9527");
        IDatabase db = null;
        public void Start()
        {
            int databaseNumber = 0;
            object asyncState = new object();
            db = redis.GetDatabase(databaseNumber, asyncState);
            UpdateRedisThread = new Thread(new ThreadStart(UpdateRedis));
            UpdateRedisThread.IsBackground = true;
            UpdateRedisThread.Start();
            
        }
    
        /// <summary>
        /// 新增交易信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="t"></param>
        public void AddTrade(string symbol, CommonLab.Trade t)
        {
           
            string key = CommonLab.RedisKeyConvert.GetRedisKey(CommonLab.RedisKeyType.TradeM, CommonLab.ExchangeNameConvert.GetShortExchangeName("Binance"), symbol, CommonLab.TimerHelper.ConvertStringToDateTime(t.ExchangeTimeStamp));
            lock (LockObject)
            {
                if (Data.ContainsKey(key))
                {
                    Data[key].Update(t, GetLegalCurrency(symbol));
                }
                else
                {
                    CommonLab.TradePerMin tpm = new CommonLab.TradePerMin(symbol, t, GetLegalCurrency(symbol));
                    Data.Add(key, tpm);
                }
            }
        }

        private void UpdateRedis()
        {
            while (true)
            {
                List<string> keys = new List<string>();
                // string key = "TradePerMin@" + symbol + "@" + CommonLab.TimerHelper.ConvertStringToDateTime(t.ExchangeTimeStamp).ToString("yyMMddHHmm") + "@" + direction;
                lock (LockObject)
                {
                    try
                    {



                        foreach (KeyValuePair<string, CommonLab.TradePerMin> item in Data)
                        {
                            string Key = item.Key.ToString();
                            DateTime t = Convert.ToDateTime(Key.Split('@')[2]);
                            if ((DateTime.UtcNow - t).TotalMinutes >= 1)
                            {
                                //需要存入redis之后移除
                                keys.Add(item.Key);
                                string jsont = JsonConvert.SerializeObject(item.Value);
                                db.StringSet(item.Key, jsont);
                                 
                                //Data.Remove(item.Key);
                                Console.WriteLine(DateTime.UtcNow.ToString() + "- WI" + item.Key + " into redis.");
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        log.log(e.Message + e.StackTrace);
                    }
                }
                foreach (string key in keys)
                {
                    Data.Remove(key);
                }
                Thread.Sleep(5 * 1000);
            }
        }

    }
}
