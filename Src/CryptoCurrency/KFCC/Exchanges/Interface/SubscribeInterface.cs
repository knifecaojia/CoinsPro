using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCC.ExchangeInterface
{
    public interface SubscribeInterface
    {
        CommonLab.TradingInfo TradeInfo { get; }


        event CommonLab.ExchangeEventWarper.TradeInfoEventHander TradeInfoEvent;
    }
}
