using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class ExchangeEventWarper
    {
        public delegate void TickerEventHander(object sender,Ticker t,EventTypes et,string tradingpair);
        public delegate void DepthEventHander(object sender, Depth d, EventTypes et, string tradingpair);
    }
    public enum EventTypes
    {
        RESTAPI,
        WSS,
        THIRDWSS
    }
    public enum SubscribeTypes
    {
        RESTAPI,
        WSS,
        THIRDWSS
    }
    
}
