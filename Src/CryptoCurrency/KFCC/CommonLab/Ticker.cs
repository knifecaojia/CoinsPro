using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class Ticker
    {

        //Info	:交易所返回的原始结构
        public double High;//	:最高价
        public double Low;//	:最低价
        public double Sell;//	:卖一价
        public double Buy;//	:买一价
        public double Last;//	:最后成交价
        public double Volume;//	:最近成交量
        public double Open;//开盘价
        public double ExchangeTimeStamp;//时间戳 交易所返回的
        public double LocalServerTimeStamp;//本地时间戳
        public double Delay;
        public void UpdateTickerBuyTrade(Trade t)
        {
            Last = t.Price;
            Volume = t.Amount;
            if (High < t.Price)
                High = t.Price;
            if (Low > t.Price)
                Low = t.Price;
            ExchangeTimeStamp = t.ExchangeTimeStamp;
            LocalServerTimeStamp = TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
        }
        public void UpdateTickerBuyDepth(Depth d)
        {
            Sell = d.Asks[0].Price;
            Buy = d.Bids[0].Price;
            if (High < Sell)
                High = Sell;
            if (Low > Buy)
                Low = Buy;
            ExchangeTimeStamp = d.ExchangeTimeStamp;
        }
        public override string ToString()
        {
            return "H:" + High + " L:" + Low + " S:" + Sell + " B:" + Buy + " La:" + Last + " V:" + Volume + " O:" + Open;
        }
        public string ToOCHLString()
        {
            return Open + "," + Last + "," + High + "," + Low + "," + Buy + "," + Sell + "," + Volume + "," + ExchangeTimeStamp;
        }
        public void UpdateTickerPeriod(Ticker t)
        {
            if (t.High > High)
                High = t.High;
            if (t.Low < Low)
                Low = t.Low;
            Sell = t.Sell;
            Buy = t.Buy;
            Last = t.Last;
            Volume = t.Volume;//存疑
            ExchangeTimeStamp = t.ExchangeTimeStamp;
        }
        public Ticker Clone()
        {
            Ticker t = new Ticker();
            //Info	:交易所返回的原始结构
            t.High = High;//	:最高价
            t.Low = Low;//	:最低价
            t.Sell = Sell;//	:卖一价
            t.Buy = Buy;//	:买一价
            t.Last = Last;//	:最后成交价
            t.Volume = Volume;//	:最近成交量
            t.Open = Open;//开盘价
            t.ExchangeTimeStamp = ExchangeTimeStamp;//时间戳 交易所返回的
            t.LocalServerTimeStamp = LocalServerTimeStamp;//本地时间戳
            t.Delay = Delay;
            return t;
        }
    }
    public enum TickerType
    {
        m1 = 0,
        m3 = 1,
        m5 = 2,
        m15 = 3,
        m30 = 4,
        h1 = 5,
        d1 = 6
    }
    public class TradingInfo
    {
        public Ticker t;
        public Depth d;
        public Trade trade;
        public CommonLab.SubscribeTypes type;
        public string tradingpair;
        public TradePair tp;
        public TradingInfo(CommonLab.SubscribeTypes _type, string _tradingpair, TradePair _tp)
        {
            t = new Ticker();
            d = new Depth();
            trade = new Trade();
            type = _type;
            tradingpair = _tradingpair;
            tp = _tp;
        }


    }
    public class Trade
    {
        public string TradeID;
        public double Amount;
        public double Price;
        public TradeType Type;
        public double ExchangeTimeStamp;//时间戳 交易所返回的
        public double LocalServerTimeStamp;//本地时间戳
        public string BuyOrderID;//成交的交易号
        public string SellOrderID;//成交的交易号

        static public TradeType GetType(string t)
        {
            if (t == "buy" || t == "bid")
                return TradeType.Buy;
            else
                return TradeType.Sell;
        }
        public Trade Clone()
        {
            Trade t = new Trade();
            t.TradeID = TradeID;
            t.Amount = Amount;
            t.Price = Price;
            t.Type = Type;
            t.ExchangeTimeStamp = ExchangeTimeStamp;//时间戳 交易所返回的
            t.LocalServerTimeStamp = LocalServerTimeStamp;//本地时间戳
            t.BuyOrderID = BuyOrderID;//成交的交易号
            t.SellOrderID = SellOrderID;//成交的交易号
            return t;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-------------------------------------\r\n");
            sb.Append("成交时间:" + TimerHelper.ConvertStringToDateTime(ExchangeTimeStamp).ToLocalTime().ToString() + "\r\n");
            sb.Append("交易ID:" + TradeID + "类别:" + Type.ToString() + "\r\n");
            sb.Append("价格:" + Price.ToString() + "   数量:" + Amount.ToString() + "\r\n");
            return sb.ToString();

        }
    }
    public class TradeCacheManage
    {
        private int CacheMins;
        public int LastTradeID;
        public List<Trade> Trades;
        public TradeCacheManage(int min)
        {
            Trades = new List<Trade>();
            CacheMins = min;

        }
        public void Add(Trade t)
        {
            Trades.Add(t.Clone());
            LastTradeID = Convert.ToInt32(t.TradeID);
            if (Trades.Count > 0)
            {
                for (int i = 0; i < Trades.Count; i++)
                {
                    if ((DateTime.Now - CommonLab.TimerHelper.ConvertStringToDateTime(Trades[i].ExchangeTimeStamp).ToLocalTime()).TotalMinutes > CacheMins)
                    {
                        Trades.RemoveAt(i);
                        i--;
                    }
                    else
                        break;


                }
            }
        }
        public double Avg()
        {
            if (Trades.Count > 0)
                return Trades.Average(item => item.Price);
            return 0;
        }
    }
    public enum TradeType
    {
        Buy = 0,
        Sell = 1,

    }

    /// <summary>
    /// 每分钟成交数据 用于redis存储 
    /// </summary>
    public class TradePeriod
    {
        public string symbol;
        /// <summary>
        /// 这一分钟的交易笔数
        /// </summary>
        public int tradecountBuy=0;
        public double quoteSymbolVolumBuy;//定价币种成交量
        public double baseSymbolVolumBuy;//交易币种成交量
        public double CNYVolumBuy;//成交cny市价计量 价格取自localbitcoin 15个报价平均值

        public int tradecountSell=0;
        public double quoteSymbolVolumSell;//定价币种成交量
        public double baseSymbolVolumSell;//交易币种成交量
        public double CNYVolumSell;//成交cny市价计量 价格取自localbitcoin 15个报价平均值
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">symbol</param>
        /// <param name="t">trade 信息</param>
        /// <param name="ex">汇率</param>
        public TradePeriod(string s, Trade t,double ex)
        {
            symbol = s;
            if (t == null)
            {
                baseSymbolVolumBuy = 0;
                quoteSymbolVolumBuy = 0;
                CNYVolumBuy =0;
                tradecountBuy = 0;
                baseSymbolVolumSell = 0;
                quoteSymbolVolumSell = 0;
                CNYVolumSell = 0;
                tradecountSell = 0;
                return;
            }
             if (t.Type == TradeType.Buy)
            {
                baseSymbolVolumBuy = t.Amount;
                quoteSymbolVolumBuy = t.Price * t.Amount;
                CNYVolumBuy = quoteSymbolVolumBuy * ex;
                tradecountBuy = 1;
            }
            else
            {
                baseSymbolVolumSell = t.Amount;
                quoteSymbolVolumSell = t.Price * t.Amount;
                CNYVolumSell = quoteSymbolVolumSell * ex;
                tradecountSell = 1;
            }

        }
        public void Update(Trade t,double ex)
        {
            if (t.Type == TradeType.Buy)
            {
                baseSymbolVolumBuy += t.Amount;
                quoteSymbolVolumBuy += t.Price * t.Amount;
                CNYVolumBuy += quoteSymbolVolumBuy * ex;
                tradecountBuy++;
            }
            else
            {
                baseSymbolVolumSell += t.Amount;
                quoteSymbolVolumSell += t.Price * t.Amount;
                CNYVolumSell += quoteSymbolVolumSell * ex;
                tradecountSell++;
            }
           
        }
    }
}
