using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public static class TimerHelper
    {
        static public double GetTimeStamp(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            double t = (dt - startTime).TotalSeconds;   //除10000调整为13位      
            return Math.Round(t);
        }
        /// <summary>
        /// 返回的是nano
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public long GetTimeStampMilliSeconds(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (long)(dt - startTime).TotalSeconds*1000;   //除10000调整为13位      
            return t;
        }
        static public DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);


            return dtStart.AddSeconds(Convert.ToDouble(timeStamp));
        }
        static public DateTime ConvertStringToDateTime(double timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);

            return dtStart.AddSeconds(timeStamp);
        }
        static public string GetTimeStampNonce()
        {
            return GetTimeStamp(DateTime.Now).ToString();
        }
        /// <summary>
        /// 根据时间间隔返回redis需要的时间key 该函数应该也可以使用到文件目录中
        /// </summary>
        /// <param name="tp"></param>
        /// <param name="inputTimeStamp"></param>
        /// <returns></returns>
        static public DateTime GetStartTimeStampByPreiod(TimePreiod tp,DateTime inputTimeStamp)
        {
            int min = 0;
            switch (tp)
            {
                case TimePreiod.m1:
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, inputTimeStamp.Minute, 0);
                case TimePreiod.m5:
                    min = inputTimeStamp.Minute- inputTimeStamp.Minute%5;
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, min, 0);
                case TimePreiod.m10:
                    min = inputTimeStamp.Minute - inputTimeStamp.Minute % 10;
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, min, 0);
                case TimePreiod.m30:
                    min = inputTimeStamp.Minute - inputTimeStamp.Minute % 30;
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, min, 0);
                case TimePreiod.h1:
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, 0, 0);
                case TimePreiod.h4:
                    int hour = inputTimeStamp.Hour - inputTimeStamp.Hour % 4;
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, hour, 0, 0);
                case TimePreiod.d1:
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, 0, 0, 0);
                default:
                    return new DateTime(inputTimeStamp.Year, inputTimeStamp.Month, inputTimeStamp.Day, inputTimeStamp.Hour, inputTimeStamp.Minute, 0);
            }
        }
    }
}
