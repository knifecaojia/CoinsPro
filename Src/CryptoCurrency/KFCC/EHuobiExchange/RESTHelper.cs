using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;

namespace KFCC.EHuobiExchange
{
    class RESTHelper : KFCC.ExchangeInterface.SubscribeInterface
    {
        private TradingInfo _tradinginfo;
        private string _tradingpair;
        public TradingInfo TradeInfo
        {
            get { return _tradinginfo; }
        }


        public event ExchangeEventWarper.TradeInfoEventHander TradeInfoEvent;

        public RESTHelper(CommonLab.TradePair tp, Ticker t, Depth d)
        {

            _tradingpair = tp.FromSymbol.ToLower() + "_" + tp.ToSymbol.ToLower();
            _tradinginfo = new TradingInfo(SubscribeTypes.RESTAPI, _tradingpair);
            _tradinginfo.t = t;
            _tradinginfo.d = d;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
