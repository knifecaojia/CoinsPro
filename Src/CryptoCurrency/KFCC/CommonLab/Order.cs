﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class Order
    {

        //Info	:交易所返回的原始结构
        public string Id { get; set; }//	:交易单唯一标识
        public double Price { get; set; }//	:下单价格
        public double AvgPrice { get; set; }//平均成交价格
        public double Amount { get; set; }//	:下单数量
        public double DealAmount { get; set; }//	:成交数量
        public OrderStatus Status { get; set; }//	:订单状态, 参考常量里的订单状态
        public OrderType Type { get; set; }//	:订单类型, 参考常量里的订单类型
        public string TradingPair { get; set; }//交易对儿，Symbol
        public DateTime CreatDate { get; set; }//交易生成时间


    }
    public class List_Order_DistinctBy_OrderID : IEqualityComparer<Order>
    {
        public bool Equals(Order x, Order y)
        {
            if (x.Id == y.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Order obj)
        {
            return 0;
        }
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
        /// <summary>
        /// 订单状态不明
        /// </summary>
        ORDER_STATE_UNKOWN

    }
    public enum OrderType
    {

        ORDER_TYPE_BUY,//	:买单
        ORDER_TYPE_SELL,//	:卖单
        ORDER_TYPE_MARKETBUY,//市价买单
        ORDER_TYPE_MARKETSELL,//市价卖单
        ORDER_TYPE_UNKOWN

    }
}
