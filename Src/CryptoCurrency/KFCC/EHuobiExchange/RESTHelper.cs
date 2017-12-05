using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public Thread CheckTread { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TradePair Tp { get; set; }

        public event ExchangeEventWarper.TradeInfoEventHander TradeInfoEvent;

        public RESTHelper(CommonLab.TradePair tp, Ticker t, Depth d)
        {

            _tradingpair = tp.FromSymbol.ToLower() + "_" + tp.ToSymbol.ToLower();
            _tradinginfo = new TradingInfo(SubscribeTypes.RESTAPI, _tradingpair,tp);
            _tradinginfo.t = t;
            _tradinginfo.d = d;
            Tp = tp;
         
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
