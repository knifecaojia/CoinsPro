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
            LocalServerTimeStamp = TimerHelper.GetTimeStamp(DateTime.Now);
        }
        public void UpdateTickerBuyDepth(Depth d)
        {
            Sell = d.Asks[0].Price;
            Buy = d.Bids[0].Price;
        }
        public override string  ToString()
        {
            return "H:"+High+ " L:" + Low + " S:" + Sell + " B:" + Buy + " La:" + Last + " V:" + Volume + " O:"+ Open;
        }
    }
    public class TradingInfo
    {
        public Ticker t;
        public Depth d;
        public CommonLab.SubscribeTypes type;
        public  string tradingpair;
        public TradePair tp;
        public TradingInfo(CommonLab.SubscribeTypes _type,string _tradingpair,TradePair _tp)
        {
            t = new Ticker();
            d = new Depth();
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
            if (t == "buy")
                return TradeType.Buy;
            else
                return TradeType.Sell;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-------------------------------------\r\n");
            sb.Append("成交时间:" + TimerHelper.ConvertStringToDateTime(ExchangeTimeStamp).ToLocalTime().ToString()+"\r\n");
            sb.Append("交易ID:"+TradeID+"类别:"+ Type.ToString()+"\r\n");
            sb.Append("价格:" + Price.ToString() + "   数量:" + Amount.ToString() + "\r\n");
            return sb.ToString();

        }
    }
    public enum TradeType
    {
        Buy=0,
        Sell=1,

    }
}
