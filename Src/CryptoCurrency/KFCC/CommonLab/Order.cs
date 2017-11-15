using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class Order
    {

        //Info	:交易所返回的原始结构
        public string Id;//	:交易单唯一标识
        public double Price;//	:下单价格
        public double Amount;//	:下单数量
        public double DealAmount;//	:成交数量
        public OrderStatus Status;//	:订单状态, 参考常量里的订单状态
        public OrderType Type;//	:订单类型, 参考常量里的订单类型

    }
    public enum OrderStatus
    {
        /// <summary>
        /// 等待中，下单量全部未完成
        /// </summary>
        ORDER_STATE_PENDING,//	:未完成
        /// <summary>
        /// 关闭 已经部分成交
        /// </summary>
        ORDER_STATE_CLOSED,//	:已关闭
        /// <summary>
        /// 撤销 全部未成交
        /// </summary>
        ORDER_STATE_CANCELED,//	:已取消
        /// <summary>
        /// 部分成交 等待中
        /// </summary>
        ORDER_STATE_PARTITAL,//
        /// <summary>
        /// 全部成交
        /// </summary>
        ORDER_STATE_SUCCESS,
        /// <summary>
        /// 撤单处理中，进行时
        /// </summary>
        ORDER_STATE_CANCELING,

    }
    public enum OrderType
    {

        ORDER_TYPE_BUY,//	:买单
        ORDER_TYPE_SELL,//	:卖单
        ORDER_TYPE_MARKETBUY,//市价买单
        ORDER_TYPE_MARKETSELL//市价卖单

    }
}
