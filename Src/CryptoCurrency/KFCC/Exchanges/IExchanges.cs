using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCC.Exchanges
{
    /// <summary>
    /// 交易所接口
    /// </summary>
    public interface IExchanges
    {
        /// <summary>
        /// 交易所名
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 交易所地址
        /// </summary>
        string ExchangeUrl { get; set; }
        /// <summary>
        /// 交易所备注
        /// </summary>
        string Remark { get; set; }
    }
}
