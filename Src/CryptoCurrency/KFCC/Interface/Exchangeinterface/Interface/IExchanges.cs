/*
 * Author:knifeandcj
 * Date:20171017
 * 交易所接口，定义交易所应支持的属性、方法、事件
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */





using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCC.ExchangeInterface
{
    /// <summary>
    /// 交易所接口
    /// </summary>
    public interface IExchanges
    {
        
        /// <summary>
        /// 交易所名
        /// </summary>
        
        string Name { get; }
        /// <summary>
        /// 交易所地址
        /// </summary>
        string ExchangeUrl { get;  }
        /// <summary>
        /// 交易所备注
        /// </summary>
        string Remark { get;  }
        string Secret { get; set; }
        string Key { get; set; }
        string UID { get; set; }
        string UserName { get; set; }
        CommonLab.Fee eFee { get; set; }//交易市场的交易手续费 一般
        CommonLab.Proxy proxy { get; set; }
        CommonLab.Account Account { get; set; }
        Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface> SubscribedTradingPairs { get;  }
        /// <summary>
        /// 该交易所是否原生支持wss
        /// </summary>
       // bool SportWSS { get; set; }
        /// <summary>
        /// 是否支持第三方wss 类似cryptocompare这样的api
        /// </summary>
        //bool SportThirdPartWSS { get; set; }



        #region 事件
        event CommonLab.ExchangeEventWarper.TickerEventHander TickerEvent;
        event CommonLab.ExchangeEventWarper.DepthEventHander DepthEvent;
        event CommonLab.ExchangeEventWarper.TradeEventHander TradeEvent;
        event CommonLab.ExchangeEventWarper.SubscribedEventHander SubscribedEvent;
        #endregion

        #region 方法
        void SetupExchage(string key, string secret, string uid, string username);
        void SetupFee(string maker, string taker);
        /// <summary>
        /// 订阅接口
        /// </summary>
        /// <param name="tradingpairs"></param>
        /// <param name="st"></param>
        /// <returns></returns>
        bool Subscribe(CommonLab.TradePair tp,CommonLab.SubscribeTypes st);
        void DisSubcribe(CommonLab.TradePair tp,CommonLab.SubscribeTypes st);
        /// <summary>
        /// 获取ticker
        /// </summary>
        /// <param name="tradingpair"></param>
        /// <returns></returns>
        CommonLab.Ticker GetTicker(string tradepair, out string rawresponse);
        /// <summary>
        /// 获取市场depth信息
        /// </summary>
        /// <param name="tradingpair"></param>
        /// <returns></returns>
        CommonLab.Depth GetDepth(string tradepair, out string rawresponse);
        /// <summary>
        /// 返回市场交易记录
        /// </summary>
        /// <param name="tradepair"></param>
        /// <param name="rawresponse"></param>
        /// <param name="since">从什么id开始的</param>
        /// <returns></returns>
        CommonLab.Trade[] GetTrades(string tradepair, out string rawresponse,string since="0");
        /// <summary>
        /// 通过交易对获取公开市场信息api的url 主要用于GET方式取得数据资源
        /// </summary>
        /// <param name="tradingpair"></param>
        /// <returns></returns>

        string GetPublicApiURL(string tradepair, string method);

        /// <summary>
        /// 通过交易对儿获取不通订阅方式下的交易对儿字符串
        /// </summary>
        /// <param name="t"></param>
        /// <param name="st"></param>
        /// <returns></returns>
        string GetLocalTradingPairString(CommonLab.TradePair t, CommonLab.SubscribeTypes st=CommonLab.SubscribeTypes.RESTAPI);



        //需要验证的交易函数
        //获取账户状态 
        void CheckSet();
        CommonLab.Account GetAccount(out string rawresponse);

        CommonLab.Order GetOrderStatus(string OrderID,string tradingpair, out string rawresponse);
        List<CommonLab.Order> GetHisOrders(string tradingpair);

        bool CancelOrder(string OrderID, string tradingpair, out string rawresponse);

        bool CancelAllOrders(string tradingpair="");
       

        string Buy(string Symbol, double Price, double Amount);

        string Sell(string Symbol, double Price, double Amount);
        #endregion
    }
}
