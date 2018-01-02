using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace KFCC.EZBExchange
{
    class WssHelper : KFCC.ExchangeInterface.SubscribeInterface
    {
        //wss://real.okex.com:10441/websocket
        const string spotwssurl = "wss://api.zb.com:9999/websocket";
        const string futhurewssurl = "";
        private string _tradingpair;
        private string _appkey;
        private string Cache = "";

    
        private TradingInfo _tradinginfo;
        public event ExchangeEventWarper.TradeInfoEventHander TradeInfoEvent;
        

        private WebSocket ws ;
        public TradingInfo TradeInfo
        {
            get { return _tradinginfo; }
        }

        public Thread CheckTread { get;set; }
        public TradePair Tp { get; set; }
        public DateTime LastCommTimeStamp { get; set; }

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
                Thread.Sleep(10000);
            }

        }
        public WssHelper(CommonLab.TradePair tp, Ticker t, Depth d)
        {
            
            _tradingpair = tp.FromSymbol.ToLower() + tp.ToSymbol.ToLower();
            _tradinginfo = new TradingInfo(SubscribeTypes.WSS, _tradingpair,tp);
            _tradinginfo.t = t;
            _tradinginfo.d = d;
            Tp = tp;
            CheckTread = new Thread(CheckState);
            CheckTread.IsBackground = true;
            ws = new WebSocket(spotwssurl);
            ws.OnOpen += (sender, e) =>
            {
                LastCommTimeStamp = DateTime.Now;
                ws.Send("{'event':'addChannel','channel':'" + _tradingpair + "_ticker'}");
                ws.Send("{'event':'addChannel','channel':'" + _tradingpair + "_depth'}");
                ws.Send("{'event':'addChannel','channel':'" + _tradingpair + "_trades'}");
                if (!CheckTread.IsAlive)
                CheckTread.Start();
            };

            ws.OnMessage += (sender, e) =>
            {
                LastCommTimeStamp = DateTime.Now;
                if (e.IsText)
                {
                    JObject obj = null;
                  
                    try
                    {
                        obj = JObject.Parse(e.Data);
                    }
                    catch
                    {
                        Console.WriteLine(e.Data);


                        return;
                    }




                    if (obj.Property("ping") != null)
                    {
                        long pong = Convert.ToInt64(obj["ping"].ToString());
                        ws.Send("{\"pong\": " + pong + "}");
                    }
                    if (obj.Property("status") != null)
                    {
                        if (obj["status"].ToString() == "error")
                        {
                            Exception err = new Exception(obj["err-msg"].ToString());
                            throw err;
                        }
                    }
                    if (obj.Property("channel") != null)
                    {
                        string ch = obj["channel"].ToString();
                        if (ch.IndexOf("ticker") > 0) //kline数据
                        {

                            try
                            {
                                JObject ticker = JObject.Parse(obj["ticker"].ToString());
                                t.High = Convert.ToDouble(ticker["high"].ToString());
                                t.Low = Convert.ToDouble(ticker["low"].ToString());
                                t.Last = Convert.ToDouble(ticker["last"].ToString());
                                t.Sell = Convert.ToDouble(ticker["sell"].ToString());
                                t.Buy = Convert.ToDouble(ticker["buy"].ToString());
                                t.Volume = Convert.ToDouble(ticker["vol"].ToString());
                                t.Open = 0;// Convert.ToDouble(ticker["open"].ToString()); ;// Convert.ToDouble(ticker["open"].ToString());
                                t.ExchangeTimeStamp = Convert.ToDouble(obj["date"].ToString()) / 1000;
                                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                                t.Delay = t.LocalServerTimeStamp - t.ExchangeTimeStamp;
                                //UpdateTicker(tradingpair, t);
                                _tradinginfo.t = t;

                                TradeInfoEvent(_tradinginfo, TradeEventType.TICKER);
                            }
                            catch (Exception err)
                            {
                                throw err;
                            }
                        }
                        if (ch.IndexOf("depth") > 0) //深度数据
                        {
                            JArray jasks = JArray.Parse(obj["asks"].ToString());
                            _tradinginfo.d.Asks = new List<MarketOrder>();
                            for (int i = 0; i < jasks.Count; i++)
                            {
                                MarketOrder m = new MarketOrder();
                                m.Price = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[0]);
                                m.Amount = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[1]);
                                _tradinginfo.d.AddNewAsk(m);
                            }

                            JArray jbids = JArray.Parse(obj["bids"].ToString());
                            _tradinginfo.d.Bids = new List<MarketOrder>();
                            for (int i = 0; i < jbids.Count; i++)
                            {
                                MarketOrder m = new MarketOrder();
                                m.Price = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[0]);
                                m.Amount = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[1]);
                                _tradinginfo.d.AddNewBid(m);
                            }
                            _tradinginfo.d.ExchangeTimeStamp = 0;// Convert.ToDouble(obj["tick"]["ts"]) ;// Convert.ToDouble(obj["timestamp"].ToString());
                            _tradinginfo.d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                            _tradinginfo.d.Delay = 0;// _tradinginfo.d.LocalServerTimeStamp - _tradinginfo.d.ExchangeTimeStamp;
                            if ((_tradinginfo.t.Buy != _tradinginfo.d.Bids[0].Price) || (_tradinginfo.t.Sell != _tradinginfo.d.Asks[0].Price))
                            {
                                TradeInfoEvent(_tradinginfo, TradeEventType.TICKER);
                            }
                            _tradinginfo.t.UpdateTickerBuyDepth(_tradinginfo.d);
                            TradeInfoEvent(_tradinginfo, TradeEventType.ORDERS);
                        }
                        if (ch.IndexOf("trades") > 0) //交易数据
                        {
                            JObject trade = JObject.Parse(obj["data"][0].ToString());

                            _tradinginfo.trade.TradeID = trade["tid"].ToString();
                            _tradinginfo.trade.Price = Convert.ToDouble(trade["price"].ToString());
                            _tradinginfo.trade.Amount = Convert.ToDouble(trade["amount"].ToString());
                            _tradinginfo.trade.ExchangeTimeStamp = Convert.ToDouble(trade["date"].ToString());
                            _tradinginfo.trade.Type = Trade.GetType(trade["type"].ToString());
                            _tradinginfo.trade.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                            //UpdateTicker(tradingpair, t);



                            TradeInfoEvent(_tradinginfo, TradeEventType.TRADE);

                        }
                        if (ch.IndexOf("detail") > 0) //市场数据
                        {
                        }
                    }



                }
            };
            ws.OnError += (sender, e) =>
            {
                Thread.Sleep(10000);
                CommonLab.Log log = new Log("/log/huobi_wss_err.log");
                log.WriteLine(e.Message);
                log.WriteLine(e.Exception.StackTrace);
                ws.Connect();
            };
            ws.Connect();
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
