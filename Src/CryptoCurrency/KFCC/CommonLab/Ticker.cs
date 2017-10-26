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

        public void UpdateTickerBuyTrade(Trade t)
        {
            Last = t.Price;
            Volume = t.Amount;
            if (High < t.Price)
                High = t.Price;
            if (Low > t.Price)
                Low = t.Price;
            ExchangeTimeStamp = t.ExchangeTimeStamp;
            LocalServerTimeStamp = TimerHelper.GetTimeStamp(DateTime.Now);
        }
        public void UpdateTickerBuyDepth(Depth d)
        {
            Sell = d.Asks[0].Price;
            Buy = d.Bids[0].Price;
        }
    }
    public class TradingInfo
    {
        public Ticker t;
        public Depth d;
    }
    public class Trade
    {
        public string TradeID;
        public double Amount;
        public double Price;
        public TradeType Type;
        public double ExchangeTimeStamp;//时间戳 交易所返回的
        public double LocalServerTimeStamp;//本地时间戳
        public string OrderID;//成交的交易号
    }
    public enum TradeType
    {
        Buy,
        Sell,

    }
}
