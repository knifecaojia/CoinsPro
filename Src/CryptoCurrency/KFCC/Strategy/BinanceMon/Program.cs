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
        public double CNYPrice = 1;//人民币计价 每十分钟刷新一次；
        public  double ETH2BTCPrice;//eth的btc计价
        public  double BNB2BTCPrice;//bnb的btc 计价
        public  double USDT2BTCPrice;//usdt的btc计价
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
        public Dictionary<string, CommonLab.TradePeriod> TradeData = new Dictionary<string, CommonLab.TradePeriod>();
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
            if (timeStamp.Year < 2018)
                return;
            foreach (CommonLab.TimePeriodType type in Enum.GetValues(typeof(CommonLab.TimePeriodType)))
            {
                string key = CommonLab.RedisKeyConvert.GetRedisKey(CommonLab.RedisKeyType.Trade, type, CommonLab.ExchangeNameConvert.GetShortExchangeName("binance"), symbol, timeStamp);
                lock (LockObject)
                {
                    if (TradeData.ContainsKey(key))
                    {
                        TradeData[key].Update(t, GetLegalCurrency(symbol));
                    }
                    else
                    {
                        CommonLab.TradePeriod tpm = new CommonLab.TradePeriod(symbol, t, GetLegalCurrency(symbol));
                        TradeData.Add(key, tpm);
                    }
                }
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
                lock (LockObject)
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

                            //需要存入redis之后移除

                            keys.Add(item.Key);
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
                            Console.WriteLine(DateTime.UtcNow.ToString() + "- WI" + item.Key + " into redis.");



                        }
                    }
                    catch (Exception e)
                    {
                        log.log(e.Message + e.StackTrace);
                    }
                }
                foreach (string key in keys)
                {
                    TradeData.Remove(key);
                }
                Thread.Sleep(30 * 1000);
            }
        }

    }
}
