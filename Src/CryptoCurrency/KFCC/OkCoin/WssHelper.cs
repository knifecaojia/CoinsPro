using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace KFCC.EOkCoin
{
    class WssHelper : KFCC.ExchangeInterface.SubscribeInterface
    {
        //wss://real.okex.com:10441/websocket
        const string spotwssurl = "wss://real.okex.com:10441/websocket";
        const string futhurewssurl = "wss://real.okex.com:10440/websocket/okexapi";
        private string _tradingpair;
        private string _appkey;

    
        private TradingInfo _tradinginfo;
        public event ExchangeEventWarper.TradeInfoEventHander TradeInfoEvent;
        

        private WebSocket ws ;
        public TradingInfo TradeInfo
        {
            get { return _tradinginfo; }
        }

        public Thread CheckTread { get ; set; }
        public TradePair Tp { get; set; }

        public WssHelper(CommonLab.TradePair tp, Ticker t, Depth d)
        {
            _tradingpair = tp.FromSymbol.ToLower()+"_"+tp.ToSymbol.ToLower();
            _tradinginfo = new TradingInfo(SubscribeTypes.WSS, _tradingpair,tp);
            _tradinginfo.t = t;
            _tradinginfo.d = d;
            Tp = tp;
            CheckTread = new Thread(CheckState);
            CheckTread.IsBackground = true;
            ws = new WebSocket(spotwssurl);
            ws.OnOpen += (sender, e) =>
            {
                ws.Send("{'event':'addChannel','channel':'ok_sub_spot_"+_tradingpair+"_ticker'}");
                ws.Send("{'event':'addChannel','channel':'ok_sub_spot_" + _tradingpair + "_depth'}");
                CheckTread.Start();
            };
            
            ws.OnMessage += (sender, e) => {
                if (e.IsText)
                {
                    JArray jaraw = JArray.Parse(e.Data);
                    if (jaraw.Count > 0)
                    {
                        JObject robj = JObject.Parse(jaraw[0].ToString());
                        string[] strs = robj["channel"].ToString().Split('_');
                        if (strs[strs.Length - 1] == "ticker")
                        {
                            //JObject obj = JObject.Parse(robj["data"].ToString());
                            try
                            {
                                JObject ticker = JObject.Parse(robj["data"].ToString());
                                t.High = Convert.ToDouble(ticker["high"].ToString());
                                t.Low = Convert.ToDouble(ticker["low"].ToString());
                                t.Last = Convert.ToDouble(ticker["last"].ToString());
                                t.Sell = Convert.ToDouble(ticker["sell"].ToString());
                                t.Buy = Convert.ToDouble(ticker["buy"].ToString());
                                t.Volume = Convert.ToDouble(ticker["vol"].ToString());
                                t.Open = 0;// Convert.ToDouble(ticker["open"].ToString());
                                t.ExchangeTimeStamp = Convert.ToDouble(ticker["timestamp"].ToString());
                                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);
                                //UpdateTicker(tradingpair, t);

                                _tradinginfo.t=t;

                                TradeInfoEvent(_tradinginfo, TradeEventType.TRADE);
                            }
                            catch(Exception err)
                            {
                                Console.WriteLine(err.Message+e.Data);
                            }
                        }
                        else if (strs[strs.Length - 1] == "depth")
                        {
                            JObject obj = JObject.Parse(robj["data"].ToString());
                            JArray jasks = JArray.Parse(obj["asks"].ToString());
                            for (int i = 0; i < jasks.Count; i++)
                            {
                                MarketOrder m = new MarketOrder();
                                m.Price = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[0]);
                                m.Amount = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[1]);
                                _tradinginfo.d.AddNewAsk(m);
                            }

                            JArray jbids = JArray.Parse(obj["bids"].ToString());
                            for (int i = 0; i < jbids.Count; i++)
                            {
                                MarketOrder m = new MarketOrder();
                                m.Price = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[0]);
                                m.Amount = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[1]);
                                _tradinginfo.d.AddNewBid(m);
                            }
                            _tradinginfo.d.ExchangeTimeStamp = 0;// Convert.ToDouble(obj["timestamp"].ToString());
                            _tradinginfo.d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);
                            TradeInfoEvent(_tradinginfo, TradeEventType.ORDERS);
                        }
                    }


                }
            };
            ws.Connect();
        }
        public void CheckState()
        {
            while (true)
            {


                if (!ws.IsAlive)
                {
                    ws.Connect();
                }
                Thread.Sleep(10000);
            }
        
        }
        public void Close()
        {
            if (ws != null)
            {
                ws.Close();
            }
        }
    }
}
