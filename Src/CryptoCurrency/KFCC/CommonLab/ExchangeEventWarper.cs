using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class ExchangeEventWarper
    {
        public delegate void TickerEventHander(object sender,Ticker t,EventTypes et);
        public delegate void DepthEventHander(object sender, Depth d, EventTypes et);
    }
    public enum EventTypes
    {
        RESTAPI,
        WSS,
        THIRDWSS
    }
}
