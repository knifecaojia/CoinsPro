

using System;
using System.Collections.Generic;
using CommonLab;
namespace KFCC.BackTest
{
    public interface IDataHandler
    {
   
        //IList<CommonLab.TradePair>TradingPairs{get;}//回测或者实盘的订阅的交易对儿，目前
        //交易市场必须同时满足同时支持交易对儿的要求，但是如果不同交易品种的统计套利就无法使用了，所以需要
        //修改
        IDictionary<string, IList<TradePair>> ExchangesTradingPairs { get; }//交易所和交易对儿对应
        TickerType Type { get; set; }//ticker 类型  
        bool ContinueBacktest { get; set; }
        DateTime? CurrentTime { get; }//模拟回测时间
        IDictionary<string, IDictionary<TradePair, Ticker>> GetLast();//获取每个交易所 交易对儿的ticker
        decimal? GetLastClosePrice(string symbol);
        void Update();
    }
}
