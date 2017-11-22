using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    /// <summary>
    /// Kline的bar数据
    /// </summary>
    public class Bar
    {
        public double Id;
        public double Amount;
        public double Count;
        public double High;//	:最高价
        public double Low;//	:最低价
        public double Close;//	:最后成交价 
        public double Volume;//	:最近成交量
        public double Open;//开盘价
        public double ExchangeTimeStamp;//时间戳 交易所返回的
        public double LocalServerTimeStamp;//本地时间戳
    }
}
