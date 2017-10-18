using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class Record
    {

        public double Time;//	:一个时间戳, 精确到毫秒，与Javascript的 new Date().getTime() 得到的结果格式一样
        public double Open;//	:开盘价
        public double High;//	:最高价
        public double Low;//	:最低价
        public double Close;//	:收盘价
        public double Volume;//	:交易量
    }
}
