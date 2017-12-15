using CommonLab;
using Newtonsoft.Json.Linq;
using PusherClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace KFCC.EBitstamp
{
    public class PusherHelper:KFCC.ExchangeInterface.SubscribeInterface
    {
        private string _tradingpair;
        private string _appkey;
        private Channel _live_trades_channel;
        private Channel _live_diff_order_book_channel;
        private Pusher _pusher;
        private TradingInfo _tradinginfo;
        public event ExchangeEventWarper.TradeInfoEventHander TradeInfoEvent;

        public TradingInfo TradeInfo { get { return _tradinginfo; } }

        public Thread CheckTread { get; set; }
        public TradePair Tp { get; set; }
        public PusherHelper(string tradingpair, Ticker t, Depth d, CommonLab.TradePair tp, string appkey = "de504dc5763aeef9ff52")
        {
            _tradingpair = tradingpair;
            _tradinginfo = new TradingInfo(SubscribeTypes.WSS, tradingpair,tp);
            _tradinginfo.t = t;
            _tradinginfo.d = d;
            Tp = tp;
            _appkey = appkey;
            _pusher = new Pusher(_appkey);
            _pusher.ConnectionStateChanged += _pusher_ConnectionStateChanged;
            _pusher.Error += _pusher_Error;
 
            _pusher.Connect();
        }
      
        public PusherHelper(string tradingpair, CommonLab.TradePair tp,string appkey= "de504dc5763aeef9ff52")
        {
            _tradingpair = tradingpair;

            _tradinginfo = new TradingInfo(SubscribeTypes.WSS,tradingpair,tp);
            Tp = tp;
            _appkey = appkey;
            _pusher = new Pusher(_appkey);
            _pusher.ConnectionStateChanged += _pusher_ConnectionStateChanged;
            _pusher.Error += _pusher_Error;
            _pusher.Connect();
        }
        public void UpdateDepth(Depth d)
        {
            _tradinginfo.d = d;
        }
        public void UpdateTicker(Ticker t)
        {
            _tradinginfo.t = t;
        }
        private void _pusher_Error(object sender, PusherException error)
        {
            throw new NotImplementedException();
        }

        private void _pusher_ConnectionStateChanged(object sender, ConnectionState state)
        {
            //throw new NotImplementedException();
            if (state == ConnectionState.Connected)
            {
                _live_trades_channel = _pusher.Subscribe("live_trades" + _tradingpair);
                _live_trades_channel.Subscribed += live_trades_channel_Subscribed;
                _live_diff_order_book_channel = _pusher.Subscribe("diff_order_book" + _tradingpair);
                _live_diff_order_book_channel.Subscribed += live_diff_order_book_channel_Subscribed;
            }
        }

        private void live_diff_order_book_channel_Subscribed(object sender)
        {
            _live_diff_order_book_channel.Bind("data", (dynamic data) =>
            {
                LiveOrderBook(data);
            });
                                   }

        private void live_trades_channel_Subscribed(object sender)
        {
            
           _live_trades_channel.Bind("trade", (dynamic data) =>
            {
                LiveTickerMsg(data);
            });
        }
        private void LiveTickerMsg(dynamic data)
        {
            //{ "event":"trade","data":"{\"amount\": 0.77256895999999997, \"buy_order_id\": 432580350, \"sell_order_id\": 432577777, \"amount_str\": \"0.77256896\", \"price_str\": \"6132.98\", \"timestamp\": \"1509415625\", \"price\": 6132.9799999999996, \"type\": 0, \"id\": 24689742}","channel":"live_trades"}
            CommonLab.Trade t = new Trade();
            //CommonLab.Ticker t = new Ticker();
            t.TradeID = data.id;
            t.Price = data.price;
            t.Amount = data.amount;

            t.Type = (CommonLab.TradeType)data.type;
            t.SellOrderID = data.sell_order_id;
            t.BuyOrderID = data.buy_order_id;
            t.ExchangeTimeStamp = data.timestamp;
            t.LocalServerTimeStamp= CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);


            _tradinginfo.t.UpdateTickerBuyTrade(t);
           
            TradeInfoEvent(_tradinginfo,TradeEventType.TRADE);
        }
        private void LiveOrderBook(dynamic data)
        {

           
            

            dynamic jasks = data.asks;
            _tradinginfo.d.Asks = new List<MarketOrder>();
            for (int i = 0; i < jasks.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price = Convert.ToDouble(jasks[i][0]);
                m.Amount = Convert.ToDouble(jasks[i][1]);
                _tradinginfo.d.AddNewAsk(m);
            }

            dynamic jbids =data.bids;
            _tradinginfo.d.Bids = new List<MarketOrder>();
            for (int i = 0; i < jbids.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price = Convert.ToDouble(jbids[i][0]);
                m.Amount = Convert.ToDouble(jbids[i][1]);
                _tradinginfo.d.AddNewBid(m);
            }
            _tradinginfo.d.ExchangeTimeStamp = Convert.ToDouble(data.timestamp);
            _tradinginfo.d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);
         
            TradeInfoEvent(_tradinginfo,TradeEventType.ORDERS);
            //return d;
        }

        public void Close()
        {
            if (_pusher != null)
            {
                _pusher.Disconnect();
            }
        }
    }
}
