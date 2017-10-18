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
        /// <summary>
        /// 该交易所是否原生支持wss
        /// </summary>
        bool SportWSS { get; set; }
        /// <summary>
        /// 是否支持第三方wss 类似cryptocompare这样的api
        /// </summary>
        bool SportThirdPartWSS { get; set; }



        #region 事件
        event CommonLab.ExchangeEventWarper.TickerEventHander TickerEvent;
        event CommonLab.ExchangeEventWarper.DepthEventHander DepthEvent;
        #endregion
    }
}
