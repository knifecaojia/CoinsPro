using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKCoinAPI
{
    public class UnixTime
    {
        static readonly DateTime StartTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));


        public static DateTime FromInt32(int time)
        {
            return StartTime.AddSeconds(time);
        }

        public static int ToInt32(DateTime time)
        {
            if (time == DateTime.MinValue) return 0;

            return (int)((time - StartTime).TotalSeconds);
        }
    }

}
