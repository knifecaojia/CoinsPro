using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using BinacneETF;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Concurrent;

namespace BinanceMon
{
    class Program
    {
        static DataManage tradeDataManage;
        static Thread GetCnyPrice;
        static Thread ReConnectThread;//币安24小时切断wss连接，需要重连
        static DateTime LastConnectTimeHour;//上次连接时间
        //上述计价信息通过线程每分钟更新一次
        static List<string> Symbols;
        static CommonLab.Log log = new CommonLab.Log("loginfo.txt");
        const string DataPath = "ExchangeDataFiles";
        static void Main(string[] args)
        {
            if (!Directory.Exists(DataPath))//如果不存在就创建file文件夹　　             　　                
                Directory.CreateDirectory(DataPath);//创建该文件夹　　    
            tradeDataManage = new DataManage(log);
            Symbols = new List<string>();
            Console.Write("Monitoring:");
            log.log("Start");
            using (System.IO.TextReader tr = new System.IO.StreamReader("Symbols.txt"))
            {
                string line = tr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    Symbols.Add(line);
                    Console.Write(line + "  ");
                    line = tr.ReadLine();
                }
            }
            Console.WriteLine();
            BinacneETF.Config.Exchange = new ExchangeInfo();
            CommonLab.HiPerfTimer hitime = new CommonLab.HiPerfTimer();
            hitime.Start();
            Config.Exchange.GetExchangeInfo();
            hitime.Stop();
            Console.WriteLine("获取服务器信息延时:" + hitime.Duration.ToString() + "秒");
            Console.WriteLine("当前时间:" + DateTime.UtcNow.ToString() + "");
            Console.WriteLine("Serv时间:" + Config.Exchange.ServerTime.ToString() + "");
            Console.WriteLine("修正系统时间.......");
            if ((DateTime.UtcNow - Config.Exchange.ServerTime).TotalMilliseconds > 0)

                CommonLab.TimerHelper.setSystemTime(Config.Exchange.ServerTime.AddSeconds(-hitime.Duration / 2));
            else
                CommonLab.TimerHelper.setSystemTime(Config.Exchange.ServerTime.AddSeconds(hitime.Duration / 2));
            Console.WriteLine("当前时间:" + DateTime.UtcNow.ToString() + "");
            Console.ReadKey();
            GetCnyPrice = new Thread(new ThreadStart(GetCNY));
            GetCnyPrice.IsBackground = true;
            GetCnyPrice.Start();
            BinacneETF.Config.UpdateTradeEvent += Config_UpdateTradeEvent;
            BinacneETF.Config.UpdateTickerEvent += Config_UpdateTickerEvent;

            BinacneETF.Config.Exchange.Start();//收集ticker
            Thread.Sleep(3000);
            LastConnectTimeHour = DateTime.Now;
            BinacneETF.Config.Exchange.StartCollectTrade(Symbols);//收集交易信息
            tradeDataManage.Start();

            //插入中间线程
            Thread midThread = new Thread(midThreadMethod);
            midThread.IsBackground = true;
            midThread.Start();
         
            while (!(Console.ReadKey().KeyChar.ToString() == "q"))
            {

            }
            return;
        }
        private static void midThreadMethod()
        {
            while (true)
            {
                if ((DateTime.Now - LastConnectTimeHour).TotalHours > 2)
                {
                   
                    try
                    {
                        if (ReConnectThread != null)
                        {
                            ReConnectThread.Abort();
                            ReConnectThread = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.log(ex.Message + ex.StackTrace);
                        ReConnectThread = null;
                    }
                    Config.Exchange.Stop();
                    Thread.Sleep(300);
                    ReConnectThread = new Thread(new ThreadStart(CheckReconnect));
                    ReConnectThread.IsBackground = true;
                    ReConnectThread.Start();
                }
                Thread.Sleep(1000 * 60);
            }
        }
        private static void CheckReconnect()
        {




            BinacneETF.Config.Exchange.StartCollectTrade(Symbols);//收集交易信息
            tradeDataManage.Start();
            LastConnectTimeHour = DateTime.Now;
            log.log("TID:" + Thread.CurrentThread.ManagedThreadId.ToString()+" Reconnect Binance Wss Services.");



        }
        private static void GetCNY()
        {
            while (true)
            {
                Console.WriteLine("收集人民币价格指数...");
                double cnyPrice= CommonLab.LocalBitcoinPrice.getLocalBitcoinPrice("CNY", BinacneETF.Config.Proxy);
                if (DataManage.CNYPrice > 100)
                {
                    if ((cnyPrice / DataManage.CNYPrice) < 0.8 && (cnyPrice / DataManage.CNYPrice) > 1.2)
                    {
                        log.log("收集人民币价格指数...CNY:" + cnyPrice + "原价格：" + DataManage.CNYPrice + "变化不合理，舍弃！");
                    }
                    else
                    {
                        DataManage.CNYPrice = cnyPrice;
                        log.log("收集人民币价格指数...CNY:" + cnyPrice + "原价格：" + DataManage.CNYPrice);

                    }
                }
                else
                {
                    DataManage.CNYPrice = cnyPrice;
                    log.log("初始人民币价格指数...CNY:" + cnyPrice + "原价格：" + DataManage.CNYPrice);

                }
                Console.WriteLine("CNY:"+ DataManage.CNYPrice);
                //log.log("收集人民币价格指数...CNY:" + DataManage.CNYPrice);
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
                DataManage.ETH2BTCPrice= ethbtc[0].Ticker.Last;
            }
            if (bnbbtc.Count() > 0)
            {
                DataManage.BNB2BTCPrice = bnbbtc[0].Ticker.Last;
            }
            if (btcusdt.Count() > 0)
            {
                DataManage.USDT2BTCPrice =1/btcusdt[0].Ticker.Last;
            }
            try
            {
                foreach (BinacneETF.TradingSymbol symbol in BinacneETF.Config.Exchange.Symbols)
                {
                    tradeDataManage.AddTicker(symbol.Symbol, symbol.Ticker);
                }
            }
            catch (Exception e)
            {
                log.log(e.Message + e.StackTrace);
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
    public class DataManage
    {
        public static double CNYPrice = 1;//人民币计价 每十分钟刷新一次；
        public static double ETH2BTCPrice;//eth的btc计价
        public static double BNB2BTCPrice;//bnb的btc 计价
        public static double USDT2BTCPrice;//usdt的btc计价
        static object LockObject = new object();
        static object LockTickObject = new object();
        static CommonLab.Log log;
        private Thread UpdateRedisThread;
        private Thread UpdateDiskFileThread;
        public DataManage(CommonLab.Log _log)
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
        public Dictionary<string, BinacneETF.BATicker> TickerData = new Dictionary<string, BinacneETF.BATicker>();
        public ConcurrentDictionary<string, CommonLab.TradePeriod> TradeData = new ConcurrentDictionary<string, CommonLab.TradePeriod>();
        private ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("10.18.20.108:9527,abortConnect=false,ssl=false,password=...");
        IDatabase db = null;
        public void Start()
        {
            int databaseNumber = 0;
            object asyncState = new object();
            db = redis.GetDatabase(databaseNumber, asyncState);
            UpdateRedisThread = new Thread(new ThreadStart(UpdateRedis));
            UpdateRedisThread.IsBackground = true;
            UpdateRedisThread.Start();
            UpdateDiskFileThread = new Thread(new ThreadStart(ExportTodisk));
            UpdateDiskFileThread.IsBackground = true;
            UpdateDiskFileThread.Start();
        }

        public void AddTicker(string symbol, BinacneETF.BATicker t)
        {
            
            DateTime timeStamp = CommonLab.TimerHelper.ConvertStringToDateTime(t.ExchangeTimeStamp);
            if (timeStamp.Year < 2018)
                return;
            foreach (CommonLab.TimePeriodType type in Enum.GetValues(typeof(CommonLab.TimePeriodType)))
            {
                string key = CommonLab.RedisKeyConvert.GetRedisKey(CommonLab.RedisKeyType.Ticker, type, CommonLab.ExchangeNameConvert.GetShortExchangeName("binance"), symbol, timeStamp);
                lock (LockTickObject)
                {
                    if (TickerData.ContainsKey(key))
                    {
                        TickerData[key].UpdateTickerByTicker(t);
                    }
                    else
                    {
                       // CommonLab.Ticker ticker = new CommonLab.TradePeriod(symbol, t, GetLegalCurrency(symbol));
                        TickerData.Add(key, t);
                    }
                }
            }
        }
        /// <summary>
        /// 新增交易信息
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="t"></param>
        public void AddTrade(string symbol, CommonLab.Trade t)
        {
            DateTime timeStamp = CommonLab.TimerHelper.ConvertStringToDateTime(t.ExchangeTimeStamp);
            string key = "";
            if (timeStamp.Year < 2018)
                return;
            int count = 0;
            try
            {
                foreach (CommonLab.TimePeriodType type in Enum.GetValues(typeof(CommonLab.TimePeriodType)))
                {
                    key = CommonLab.RedisKeyConvert.GetRedisKey(CommonLab.RedisKeyType.Trade, type, CommonLab.ExchangeNameConvert.GetShortExchangeName("binance"), symbol, timeStamp);
                    //if (DateTime.Now.Hour == 0 && DateTime.Now.Minute < 5)
                    //{
                    //    log.log("key:" + key + "  TradeData："+ TradeData.Count);
                    //}
                    //count++;
                    lock (TradeData)
                    {
                        count++;
                        if (TradeData.ContainsKey(key))
                        {
                            count++;
                            TradeData[key].Update(t, GetLegalCurrency(symbol));
                        }
                        else
                        {
                            count++;
                            CommonLab.TradePeriod tpm = new CommonLab.TradePeriod(symbol, t, GetLegalCurrency(symbol));
                            count++;
                            TradeData.TryAdd(key, tpm);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(TradeData);
                log.log(count+"key:"+key+"\r\nORE"+e.Message + e.StackTrace+e.Source+"\r\n"+ json);
                Console.WriteLine("key:" + key + "\r\nORE" + e.Message + e.StackTrace + e.Source + "\r\n" + json);
                //Console.ReadKey();
            }
           
            
            
        }
        private void ExportTodisk()
        {
            while (true)
            {
                List<string> keys = new List<string>();
                // string key = "TradePerMin@" + symbol + "@" + CommonLab.TimerHelper.ConvertStringToDateTime(t.ExchangeTimeStamp).ToString("yyMMddHHmm") + "@" + direction;
                lock (LockTickObject)
                {
                    try
                    {

                        foreach (KeyValuePair<string, BinacneETF.BATicker> item in TickerData)
                        {
                            string Key = item.Key.ToString();
                            //string str = redisKeyType+ "@" + timePeriod + "@" +exchangename+"@"+ symbol.ToLower()+"@"+ t.ToString("yyyy.MM.dd HH:mm");

                            DateTime t = Convert.ToDateTime(Key.Split('@')[4]);
                            string type = Key.Split('@')[0];
                            string period = Key.Split('@')[1];
                            string exchangename = CommonLab.ExchangeNameConvert.GetLongExchangeName(Key.Split('@')[2]);
                            string symbol = Key.Split('@')[3];
                            CommonLab.TimePeriodType periodType = (CommonLab.TimePeriodType)Enum.Parse(typeof(CommonLab.TimePeriodType), period);
                            // bool FlushFlag = false;
                            switch (periodType)
                            {
                                case CommonLab.TimePeriodType.m1:
                                    if ((DateTime.UtcNow - t).TotalMinutes >= 1)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.m5:
                                    if ((DateTime.UtcNow - t).TotalMinutes >= 5)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.m10:
                                    if ((DateTime.UtcNow - t).TotalMinutes >= 10)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.m30:
                                    if ((DateTime.UtcNow - t).TotalMinutes >= 30)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.h1:
                                    if ((DateTime.UtcNow - t).TotalHours >= 1)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.h4:
                                    if ((DateTime.UtcNow - t).TotalHours >= 4)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.d1:
                                    if ((DateTime.UtcNow - t).TotalHours >= 24)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                default:
                                    break;
                            }
                            string jsont = item.Value.ToOCHLString();
                            if (CommonLab.RedisKeyConvert.GetRedisKeyExpiredTime(periodType) == -1)
                            {
                                db.StringSet(item.Key, jsont);
                            }
                            else
                                db.KeyExpire(item.Key, DateTime.UtcNow.AddMinutes(48 * 60));


                        }
                    }
                    catch (Exception e)
                    {
                        log.log(e.Message + e.StackTrace);
                    }


                    foreach (string key in keys)
                    {
                        DateTime t = Convert.ToDateTime(key.Split('@')[4]);
                        string year = t.Year.ToString();
                        string month = t.Month.ToString();
                        string day = t.Day.ToString();
                        string type = key.Split('@')[0];
                        string period = key.Split('@')[1];
                        string exchangename = CommonLab.ExchangeNameConvert.GetLongExchangeName(key.Split('@')[2]);
                        string symbol = key.Split('@')[3];

                        string file = "ExchangeDataFiles" + @"/" + type + "/" + period + "/" + "/" + exchangename + "/" + symbol + "/" + year + @"/" + month + @"/" + day + ".txt";
                        CommonLab.Log log = new CommonLab.Log(file);

                        //double ts= CommonLab.TimerHelper.GetTimeStamp(exporttime.AddSeconds(-exporttime.Second).ToUniversalTime());

                        log.RawLog(TickerData[key].ToOCHLString());
                        TickerData.Remove(key);
                    }
                }
                Thread.Sleep(60 * 1000);
            }
           
           
        }
        private void UpdateRedis()
        {
            while (true)
            {
                List<string> keys = new List<string>();
                // string key = "TradePerMin@" + symbol + "@" + CommonLab.TimerHelper.ConvertStringToDateTime(t.ExchangeTimeStamp).ToString("yyMMddHHmm") + "@" + direction;
                lock (TradeData)
                {
                    try
                    {

                        foreach (KeyValuePair<string, CommonLab.TradePeriod> item in TradeData)
                        {
                            string Key = item.Key.ToString();
                            DateTime t = Convert.ToDateTime(Key.Split('@')[4]);
                            string type = Key.Split('@')[0];
                            string period = Key.Split('@')[1];
                            string exchangename = CommonLab.ExchangeNameConvert.GetLongExchangeName(Key.Split('@')[2]);
                            string symbol = Key.Split('@')[3];
                            CommonLab.TimePeriodType periodType = (CommonLab.TimePeriodType)Enum.Parse(typeof(CommonLab.TimePeriodType), period);
                            switch (periodType)
                            {
                                case CommonLab.TimePeriodType.m1:
                                    if ((DateTime.UtcNow - t).TotalMinutes >= 2)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.m5:
                                    if ((DateTime.UtcNow - t).TotalMinutes >= 6)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.m10:
                                    if ((DateTime.UtcNow - t).TotalMinutes >= 11)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.m30:
                                    if ((DateTime.UtcNow - t).TotalMinutes >= 31)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.h1:
                                    if ((DateTime.UtcNow - t).TotalHours >= 1.1)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.h4:
                                    if ((DateTime.UtcNow - t).TotalHours >= 4.1)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                case CommonLab.TimePeriodType.d1:
                                    if ((DateTime.UtcNow - t).TotalHours >= 24.1)
                                    {
                                        keys.Add(item.Key);
                                    }
                                    break;
                                default:
                                    break;
                            }

                            //需要存入redis之后移除

                            //keys.Add(item.Key);
                            string jsont = JsonConvert.SerializeObject(item.Value);
                            if (CommonLab.RedisKeyConvert.GetRedisKeyExpiredTime(periodType) == -1)
                            {
                                db.StringSet(item.Key, jsont);
                            }
                            else
                            {
                                db.StringSet(item.Key, jsont);

                                db.KeyExpire(item.Key, DateTime.UtcNow.AddMinutes(48 * 60));
                            }


                            //Data.Remove(item.Key);
                            Console.WriteLine("TID:"+ Thread.CurrentThread.ManagedThreadId.ToString() +" "+DateTime.UtcNow.ToString() + "- WI" + item.Key + " into redis.");



                        }
                    }
                    catch (Exception e)
                    {
                        log.log(e.Message + e.StackTrace);
                    }
                }
                foreach (string key in keys)
                {
                    CommonLab.TradePeriod igonred = new CommonLab.TradePeriod("", null, 0);
                    TradeData.TryRemove(key,out igonred);
                }
                Thread.Sleep(30 * 1000);
            }
        }

    }
}
