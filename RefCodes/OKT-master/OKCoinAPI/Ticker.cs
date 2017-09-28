using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKCoinAPI
{
    public enum TickerType 
    { 
        BTC, 
        LTC 
    }

    public class Ticker
    {
        public DateTime Time;
        public decimal High;
        public decimal Low;
        public decimal Buy;
        public decimal Sell;
        public decimal Last;
        public decimal Volume;
    }

}
