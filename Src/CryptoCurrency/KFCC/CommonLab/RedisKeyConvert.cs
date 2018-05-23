using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    static public class RedisKeyConvert
    {
        /// <summary>
        /// 通过输入变量获取rediskey 下一步为文件记录获取目录等级和文件名
        /// </summary>
        /// <param name="redisKeyType">redis 类型</param>
        /// <param name="exchangename">交易所短名字</param>
        /// <param name="symbol">币对儿</param>
        /// <param name="timestamp">记录的起始时间点</param>
        /// <returns></returns>
        public static string GetRedisKey(RedisKeyType redisKeyType,string exchangename, string symbol, DateTime timestamp)
        {
            string str = redisKeyType+"@"+exchangename+"@"+ symbol+"@"+ timestamp.ToString("yyyy.MM.dd HH:mm");
            return str;
        }
    }
    public enum TimePreiod
    {
        m1,
        m5,
        m10,
        m30,
        h1,
        h4,
        d1
        
    }
    public enum RedisKeyType
    {
        TickerM,
        Ticker5M,
        Ticker10M,
        Ticker30M,
        Ticker1h,
        Ticker4h,
        Ticker1d,
        TradeM,
        Trade5M,
        Trade10M,
        Trade30M,
        Tradeh,
        Trade4h,
        Trade1d,
    }
}
