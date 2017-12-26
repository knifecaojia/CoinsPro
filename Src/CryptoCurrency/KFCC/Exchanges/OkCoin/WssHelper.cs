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
        public DateTime LastCommTimeStamp { get ; set; }

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
                ws.Send("{'event':'addChannel','channel':'ok_sub_spot_" + _tradingpair + "_depth_20'}");
                ws.Send("{'event':'addChannel','channel':'ok_sub_spot_" + _tradingpair + "_deals'}");

                //if (!CheckTread.IsAlive) CheckTread.Start();
            };
            
            ws.OnMessage += (sender, e) => {
                if (e.IsText)
                {
                    LastCommTimeStamp = DateTime.Now;
                    JArray jaraw = JArray.Parse(e.Data);
                    if (jaraw.Count > 0)
                    {
                        JObject robj = JObject.Parse(jaraw[0].ToString());
                        string[] strs = robj["channel"].ToString().Split('_');
                        if (strs.Length < 2)
                            return;
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
                                
                                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                                t.Delay = t.LocalServerTimeStamp - t.ExchangeTimeStamp;
                                //UpdateTicker(tradingpair, t);

                                _tradinginfo.t = t;

                                TradeInfoEvent(_tradinginfo, TradeEventType.TICKER);
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine(err.Message + e.Data);
                            }
                        }
                        else if (strs[strs.Length - 2] == "depth")
                        {
                            JObject obj = JObject.Parse(robj["data"].ToString());
                            try
                            {
                                if (obj.Property("asks") != null)
                                {
                                    _tradinginfo.d.Asks = new List<MarketOrder>();
                                    JArray jasks = JArray.Parse(obj["asks"].ToString());

                                    for (int i = 0; i < jasks.Count; i++)
                                    {
                                        MarketOrder m = new MarketOrder();
                                        m.Price = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[0]);
                                        m.Amount = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[1]);
                                        _tradinginfo.d.AddNewAsk(m);
                                    }
                                }
                            }
                            catch
                            { }
                            try
                            {
                                if (obj.Property("bids") != null)
                                {
                                    JArray jbids = JArray.Parse(obj["bids"].ToString());
                                    _tradinginfo.d.Bids = new List<MarketOrder>();
                                    for (int i = 0; i < jbids.Count; i++)
                                    {
                                        MarketOrder m = new MarketOrder();
                                        m.Price = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[0]);
                                        m.Amount = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[1]);
                                        _tradinginfo.d.AddNewBid(m);
                                    }
                                }
                            }
                            catch
                            { }
                            _tradinginfo.d.ExchangeTimeStamp =  Convert.ToDouble(obj["timestamp"].ToString());
                            _tradinginfo.d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                            _tradinginfo.d.Delay = _tradinginfo.d.LocalServerTimeStamp - _tradinginfo.d.ExchangeTimeStamp;

                            _tradinginfo.t.UpdateTickerBuyDepth(_tradinginfo.d);
                            TradeInfoEvent(_tradinginfo, TradeEventType.ORDERS);
                        }
                        else if (strs[strs.Length - 1] == "deals")
                        {
                            try
                            {
                                JArray trade = JArray.Parse((JArray.Parse(robj["data"].ToString()))[0].ToString());
                                _tradinginfo.trade.TradeID = trade[0].ToString();
                                _tradinginfo.trade.Price = Convert.ToDouble(trade[1].ToString());
                                _tradinginfo.trade.Amount = Convert.ToDouble(trade[2].ToString());
                                _tradinginfo.trade.ExchangeTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(Convert.ToDateTime(trade[3].ToString()));
                                _tradinginfo.trade.Type =Trade.GetType(trade[4].ToString());
                                _tradinginfo.trade.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                                //UpdateTicker(tradingpair, t);

                               

                                TradeInfoEvent(_tradinginfo, TradeEventType.TRADE);
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine(err.Message + e.Data);
                            }
                        }
                    }


                }
            };
            ws.OnError += (sender, e) =>
            {
                
                Thread.Sleep(1000);
                CommonLab.Log log = new Log("/log/okcoin_wss_err.log");
                log.WriteLine(e.Message);
                log.WriteLine(e.Exception.StackTrace);
                ws.Close();
                ws.Connect();
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
                if ((DateTime.Now - LastCommTimeStamp).TotalSeconds > 10)
                {
                    ws.Connect();
                    LastCommTimeStamp = DateTime.Now;
                }
                Thread.Sleep(1000);
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
