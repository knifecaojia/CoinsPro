using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public static class TimerHelper
    {
        static public long GetTimeStamp(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (long)(dt - startTime).TotalSeconds;   //除10000调整为13位      
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
    }
}
