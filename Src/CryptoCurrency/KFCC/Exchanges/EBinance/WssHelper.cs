using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace KFCC.Exchanges.EBinance
{
    class WssHelper : KFCC.ExchangeInterface.SubscribeInterface
    {
        //wss://real.okex.com:10441/websocket
        const string spotwssurl = "wss://stream.binance.com:9443/ws/";
        //const string futhurewssurl = "wss://real.okex.com:10440/websocket/okexapi";
        private string _tradingpair;
        private string _appkey;

    
        private TradingInfo _tradinginfo;
        public event ExchangeEventWarper.TradeInfoEventHander TradeInfoEvent;
        

        private WebSocket wsticker ;
        private WebSocket wsdepth;
        private WebSocket wstrade;
        public TradingInfo TradeInfo
        {
            get { return _tradinginfo; }
        }

        public Thread CheckTread { get ; set; }
        public TradePair Tp { get; set; }
        public DateTime LastCommTimeStamp { get ; set; }

        public WssHelper(CommonLab.TradePair tp, Ticker t, Depth d)
        {
            if (tp.FromSymbol.ToLower() == "bch")
                _tradingpair = "bcc" + tp.ToSymbol.ToLower();
            else if (tp.ToSymbol.ToLower() == "bch")
                _tradingpair = tp.FromSymbol.ToLower() + "bcc";
            else
                _tradingpair = tp.FromSymbol.ToLower() + tp.ToSymbol.ToLower();
           
            _tradinginfo = new TradingInfo(SubscribeTypes.WSS, _tradingpair,tp);
            _tradinginfo.t = t;
            _tradinginfo.d = d;
            Tp = tp;
            CheckTread = new Thread(CheckState);
            CheckTread.IsBackground = true;
            wsticker = new WebSocket(spotwssurl+ _tradingpair+"@ticker");
            wstrade = new WebSocket(spotwssurl + _tradingpair + "@trade");
            wsdepth = new WebSocket(spotwssurl + _tradingpair + "@depth20");
            //wsticker.OnOpen += (sender, e) =>
            //{
            //    ws.Send("{'event':'addChannel','channel':'ok_sub_spot_"+_tradingpair+"_ticker'}");
            //    ws.Send("{'event':'addChannel','channel':'ok_sub_spot_" + _tradingpair + "_depth_20'}");
            //    ws.Send("{'event':'addChannel','channel':'ok_sub_spot_" + _tradingpair + "_deals'}");

            //    if (!CheckTread.IsAlive) CheckTread.Start();
            //};

            wsticker.OnMessage += (sender, e) =>
            {
                if (e.IsText)
                {
                    LastCommTimeStamp = DateTime.Now;

                    JObject ticker = JObject.Parse(e.Data);
                    try
                    {
                        
                        t.High = Convert.ToDouble(ticker["h"].ToString());
                        t.Low = Convert.ToDouble(ticker["l"].ToString());
                        t.Last = Convert.ToDouble(ticker["C"].ToString());
                        t.Sell = Convert.ToDouble(ticker["a"].ToString());
                        t.Buy = Convert.ToDouble(ticker["b"].ToString());
                        t.Volume = Convert.ToDouble(ticker["q"].ToString());
                        t.Open = 0;// Convert.ToDouble(ticker["open"].ToString());
                        t.ExchangeTimeStamp = Convert.ToDouble(ticker["E"].ToString());
                        t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                        //UpdateTicker(tradingpair, t);

                        _tradinginfo.t = t;
                        if (TradeInfoEvent != null)
                            TradeInfoEvent(_tradinginfo, TradeEventType.TICKER);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("ticker err:"+err.Message + e.Data);
                    }



                }
            };

            wstrade.OnMessage += (sender, e) =>
            {
                if (e.IsText)
                {
                    LastCommTimeStamp = DateTime.Now;

                    JObject trade = JObject.Parse(e.Data);
                    
                    try
                    {
                        
                        _tradinginfo.trade.TradeID = trade["t"].ToString();
                        _tradinginfo.trade.Price = Convert.ToDouble(trade["p"].ToString());
                        _tradinginfo.trade.Amount = Convert.ToDouble(trade["q"].ToString());
                        _tradinginfo.trade.ExchangeTimeStamp = Convert.ToDouble(trade["E"].ToString()) ;
                        if (trade["m"].ToString() == "m")
                            _tradinginfo.trade.Type = TradeType.Buy;
                        else
                            _tradinginfo.trade.Type = TradeType.Sell;
                        _tradinginfo.trade.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                        if (TradeInfoEvent != null)
                            TradeInfoEvent(_tradinginfo, TradeEventType.TRADE);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("trade err:" + err.Message + e.Data);
                    }



                }
            };
            wsdepth.OnMessage += (sender, e) =>
            {
                if (e.IsText)
                {
                    LastCommTimeStamp = DateTime.Now;

                    JObject depth = JObject.Parse(e.Data);

                    try
                    {
                        JArray jasks = JArray.Parse(depth["asks"].ToString());
                        _tradinginfo.d.Asks = new List<MarketOrder>();
                        for (int i = 0; i < jasks.Count; i++)
                        {
                            MarketOrder m = new MarketOrder();
                            m.Price = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[0]);
                            m.Amount = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[1]);
                            _tradinginfo.d.AddNewAsk(m);
                        }

                        JArray jbids = JArray.Parse(depth["bids"].ToString());
                        _tradinginfo.d.Bids = new List<MarketOrder>();
                        for (int i = 0; i < jbids.Count; i++)
                        {
                            MarketOrder m = new MarketOrder();
                            m.Price = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[0]);
                            m.Amount = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[1]);
                            _tradinginfo.d.AddNewBid(m);
                        }
                        _tradinginfo.d.ExchangeTimeStamp = 0;// Convert.ToDouble(obj["timestamp"].ToString());
                        _tradinginfo.d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                        if(TradeInfoEvent!=null)
                        TradeInfoEvent(_tradinginfo, TradeEventType.ORDERS);
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("depth err:" + err.Message + e.Data);
                    }



                }
            };
            wsticker.OnError += (sender, e) =>
            {
                Thread.Sleep(10000);
                CommonLab.Log log = new Log("/log/binance_wss_err.log");
                log.WriteLine(e.Message);
                log.WriteLine(e.Exception.StackTrace);
                wsticker.Connect();
            };
            wstrade.OnError += (sender, e) =>
            {
                Thread.Sleep(10000);
                CommonLab.Log log = new Log("/log/binance_wss_err.log");
                log.WriteLine(e.Message);
                log.WriteLine(e.Exception.StackTrace);
                wstrade.Connect();
            };
            wsdepth.OnError += (sender, e) =>
            {
                Thread.Sleep(10000);
                CommonLab.Log log = new Log("/log/binance_wss_err.log");
                log.WriteLine(e.Message);
                log.WriteLine(e.Exception.StackTrace);
                wsdepth.Connect();
            };
            wsticker.Connect();
            wsdepth.Connect();
            wstrade.Connect();
            if (!CheckTread.IsAlive) CheckTread.Start();
        }
        public void CheckState()
        {
            Thread.Sleep(10000);
            while (true)
            {


                if (!wsticker.IsAlive)
                {
                    wsticker.Connect();
                }
                if ((DateTime.Now - LastCommTimeStamp).TotalSeconds > 10)
                {
                    wsticker.Connect();
                    LastCommTimeStamp = DateTime.Now;
                }
                Thread.Sleep(10000);
            }
        
        }
        public void Close()
        {
            if (wsticker != null)
            {
                wsticker.Close();
            }
            if (wsdepth != null)
            {
                wsdepth.Close();
            }
            if (wstrade != null)
            {
                wstrade.Close();
            }
        }
    }
}
