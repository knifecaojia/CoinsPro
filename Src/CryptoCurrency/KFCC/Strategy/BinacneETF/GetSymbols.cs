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
        List<TradingSymbol> Symbols;
        Dictionary<string, List<TradingSymbol>> SymbolsClassByQuoteAsset;
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
                    
                }
            }
            catch
            {

            }
            SymbolsClassByQuoteAsset = symbols;
            return symbols;
        }
    }

}
