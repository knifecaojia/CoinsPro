using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKCoinAPI
{
    public class Order
    {
        public decimal amount;          //委托数量
        public decimal avg_price;       //平均成交价
        public DateTime create_date;    //委托时间
        public decimal deal_amount;     //成交数量
        public string order_id;        //订单ID
        public string orders_id;       //订单ID(不建议使用)
        public decimal price;           //委托价格
        public string status;           //-1:已撤销  0:未成交  1:部分成交  2:完全成交 4:撤单处理中
        public string symbol;
        public string type;             //buy_market:市价买入 / sell_market:市价卖出

    }

}
