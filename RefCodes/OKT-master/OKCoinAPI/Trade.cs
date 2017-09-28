using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKCoinAPI
{
    /// <summary>
    /// 买卖类型：
    /// 限价单（buy/sell）
    /// 市价单（buy_market/sell_market）
    /// </summary>
    public enum TradeType
    {
        buy,
        sell,
        buy_market,
        sell_market
    }

    public class Trade
    {
        static string[] tradeTypeName = new string[]{"buy", "sell", "buy_market", "sell_market"};

        public TradeType tradeType;

        /// <summary>
        /// //买卖类型： 限价单（buy/sell） 市价单（buy_market/sell_market）
        /// </summary>
        public string type
        {
            get 
            {
                int index = (int)tradeType;
                string typeName = tradeTypeName[index];
                return typeName;
            }
        }
        
        /// <summary>
        /// 下单价格 [限价买单(必填)： 大于等于0，小于等于1000000 | 市价买单(必填)： BTC :最少买入0.01个BTC 的金额(金额>0.01*卖一价) / LTC :最少买入0.1个LTC 的金额(金额>0.1*卖一价)]（市价卖单不传price）
        /// </summary>
        public decimal price;

        /// <summary>
        /// 交易数量 [限价卖单（必填）：BTC 数量大于等于0.01 / LTC 数量大于等于0.1 | 市价卖单（必填）： BTC :最少卖出数量大于等于0.01 / LTC :最少卖出数量大于等于0.1]（市价买单不传amount）
        /// </summary>
        public decimal amount;


        public Trade()
        {
        }

        public Trade(TradeType tradeType, decimal price, decimal amount)
        {
            this.tradeType = tradeType;
            this.price = price;
            this.amount = amount;
        }

    }



}
