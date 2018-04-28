using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
namespace BinacneETF
{
    public class ExchangeInfo
    {
        public string TimeZone;
        public double ServerTimeStamp;
        public DateTime ServerTime;
        public WssHelper wsshelper;
        public List<TradingSymbol> Symbols=new List<TradingSymbol>();
        public Dictionary<string, List<TradingSymbol>> SymbolsClassByQuoteAsset;
        public Dictionary<string, List<TradingSymbol>> GetExchangeInfo()
        {
            Dictionary<string, List<TradingSymbol>> symbols = new Dictionary<string, List<TradingSymbol>>();
            string url = Config.ApiUrl + Config.ExchangeInformation;
            string rawresponse = CommonLab.Utility.GetHttpContent(url, "GET", "", Config.Proxy);
            try
            {
                JObject JsonRaw = JObject.Parse(rawresponse);
                TimeZone = JsonRaw["timezone"].ToString();
                ServerTimeStamp = Convert.ToDouble(JsonRaw["serverTime"].ToString());
                ServerTime = CommonLab.TimerHelper.ConvertStringToDateTime(ServerTimeStamp/1000);
                JArray JaSymbols = JArray.Parse(JsonRaw["symbols"].ToString());
                for (int i = 0; i < JaSymbols.Count; i++)
                {
                    TradingSymbol symbol = new TradingSymbol();
                    symbol.Ticker = new BATicker();
                    JObject jSymbol = JObject.Parse(JaSymbols[i].ToString());
                    symbol.Symbol = jSymbol["symbol"].ToString();
                    symbol.Status = jSymbol["status"].ToString();
                    symbol.BaseAsset = jSymbol["baseAsset"].ToString();
                    symbol.baseAssetPrecision = Convert.ToInt32(jSymbol["baseAssetPrecision"].ToString());
                    symbol.QuoteAsset = jSymbol["quoteAsset"].ToString();
                    symbol.quoteAssetPrecision = Convert.ToInt32(jSymbol["quotePrecision"].ToString());
                    JArray Jafilters = JArray.Parse(jSymbol["filters"].ToString());
                    for (int j = 0; j < Jafilters.Count; j++)
                    {
                        switch (Jafilters[j]["filterType"].ToString())
                        {
                            case "PRICE_FILTER":
                                symbol.minPrice = Convert.ToDouble(Jafilters[j]["minPrice"].ToString());
                                symbol.maxPrice = Convert.ToDouble(Jafilters[j]["maxPrice"].ToString());
                                break;
                            case "LOT_SIZE":
                                symbol.minQty = Convert.ToDouble(Jafilters[j]["minQty"].ToString());
                                symbol.maxQty = Convert.ToDouble(Jafilters[j]["maxQty"].ToString());
                                break;
                        }
                        
                    }
                    if (!symbols.ContainsKey(symbol.QuoteAsset))
                    {
                        symbols.Add(symbol.QuoteAsset, new List<TradingSymbol>());

                    }
                    symbols[symbol.QuoteAsset].Add(symbol);
                    Symbols.Add(symbol);
                }
            }
            catch
            {

            }
            SymbolsClassByQuoteAsset = symbols;
            return symbols;
        }
        public override string ToString()
        {
            if (SymbolsClassByQuoteAsset == null)
                return base.ToString();
            else
            {
                return string.Format("交易所计价资产共{0}种，交易币对共{1}种", SymbolsClassByQuoteAsset.Count, Symbols.Count);
            }
        }
        public void Start()
        {
            wsshelper = new WssHelper();
            Config.RaiseUpdateConsoleEvent(System.Drawing.Color.Black, "开始监测交易所价格信息...");
            wsshelper.StartListen();
        }
        public void Stop()
        {
            wsshelper.Stop();
        }
    }
    public class BATicker
    {

        //Info	:交易所返回的原始结构
        public double High;//	:最高价
        public double Low;//	:最低价
        public double Sell;//	:卖一价
        public double SellQuantity;//	:卖一量
        public double BuyQuantity;//	:卖一量
        public double PriceChange;//价变
        public double PriceChangePct;//价变比
        public double Buy;//	:买一价
        public double Last;//	:最后成交价
        public double Volume;//	:最近成交量Quote
        public double VolumeBase;//	:最近成交量Base
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
        public BATicker Clone()
        {
            BATicker t = new BATicker();
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
}
