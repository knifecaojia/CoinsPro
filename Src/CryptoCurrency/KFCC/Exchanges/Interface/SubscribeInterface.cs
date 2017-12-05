using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KFCC.ExchangeInterface
{
    public interface SubscribeInterface
    {
        CommonLab.TradingInfo TradeInfo { get; }
        CommonLab.TradePair Tp { get; set; }
        Thread CheckTread { get; set; }
        event CommonLab.ExchangeEventWarper.TradeInfoEventHander TradeInfoEvent;
        void Close();
    }
}
