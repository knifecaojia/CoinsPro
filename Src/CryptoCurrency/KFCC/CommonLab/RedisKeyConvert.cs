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
        public static string GetRedisKey(RedisKeyType redisKeyType, TimePeriodType timePeriod, string exchangename, string symbol, DateTime timestamp)
        {
            DateTime t = TimerHelper.GetStartTimeStampByPreiod(timePeriod, timestamp);
            string str = redisKeyType+ "@" + timePeriod + "@" +exchangename+"@"+ symbol.ToLower()+"@"+ t.ToString("yyyy.MM.dd HH:mm");
            return str;
        }
        public static int GetRedisKeyExpiredTime(TimePeriodType timePeriod)
        {
            switch (timePeriod)
            {
                case TimePeriodType.d1:
                    return -1;
                case TimePeriodType.h4:
                    return -1;
                case TimePeriodType.h1:
                    return -1;
                default:
                    return  48 * 60 * 60;
            }
        }
    }
    public enum TimePeriodType
    {
        m1=1,
        m5=2,
        m10=3,
        m30=4,
        h1=5,
        h4=6,
        d1=7
        
    }
    public enum RedisKeyType
    {
        Ticker,
        Trade,
    }
}
