using CommonLab;
using Newtonsoft.Json.Linq;
using PusherClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCC.EBitstamp
{
    public class PusherHelper
    {
        private string _tradingpair;
        private string _appkey;
        private Channel _live_trades_channel;
        private Channel _live_diff_order_book_channel;
        private Pusher _pusher;
        public PusherHelper(string tradingpair,string appkey= "de504dc5763aeef9ff52")
        {
            _tradingpair = tradingpair;
            _pusher.ConnectionStateChanged += _pusher_ConnectionStateChanged;
            _pusher.Error += _pusher_Error;
            _pusher.Connect();
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
                _live_trades_channel = _pusher.Subscribe("live_trades_" + _tradingpair);
                _live_trades_channel.Subscribed += live_trades_channel_Subscribed;
                _live_diff_order_book_channel = _pusher.Subscribe("diff_order_book_" + _tradingpair);
                _live_diff_order_book_channel.Subscribed += live_diff_order_book_channel_Subscribed;
            }
        }

        private void live_diff_order_book_channel_Subscribed(object sender)
        {
            _live_diff_order_book_channel.Bind("data", (dynamic data) =>
            {
                LiveOrderBook(data.message);
            });
        }

        private void live_trades_channel_Subscribed(object sender)
        {
            
           _live_trades_channel.Bind("trade", (dynamic data) =>
            {
                LiveTickerMsg(data.message);
            });
        }
        private void LiveTickerMsg(string msg)
        {

        }
        private void LiveOrderBook(string msg)
        {
           
            CommonLab.Depth d = new Depth();
            d.Asks = new List<MarketOrder>();
            d.Bids = new List<MarketOrder>();
            JObject obj = JObject.Parse(msg);
            JArray jasks = JArray.Parse(obj["asks"].ToString());
            for (int i = 0; i < jasks.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[0]);
                m.Amount = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[1]);
                d.Asks.Add(m);
            }

            JArray jbids = JArray.Parse(obj["bids"].ToString());
            for (int i = 0; i < jbids.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[0]);
                m.Amount = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[1]);
                d.Bids.Add(m);
            }
            d.ExchangeTimeStamp = Convert.ToDouble(obj["timestamp"].ToString());
            d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);
            //_lastdepth = d;
            //return d;
        }
    }
}
