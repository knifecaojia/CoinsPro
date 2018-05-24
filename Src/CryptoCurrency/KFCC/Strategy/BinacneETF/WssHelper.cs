using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Threading;
namespace BinacneETF
{
    public class WssHelper
    {
        private WebSocket wsticker;
        private WebSocket wstrade;
        const string spotwssurl = "wss://stream.binance.com:9443";
        public DateTime LastCommTimeStamp { get; set; }
        public WssHelper()
        {

        }
        public void StartListen()
        {
            wsticker = new WebSocket(spotwssurl + "/ws/!ticker@arr");
            if (Config.Proxy != null)
            {
                wsticker.SetProxy("http://"+Config.Proxy.IP+":"+ Config.Proxy.Port, "", "");
            }
            wsticker.OnMessage += (sender, e) =>
            {
                if (e.IsText)
                {
                    LastCommTimeStamp = DateTime.Now;

                    
                    JArray JaTickers = JArray.Parse(e.Data);
                    try
                    {
                        for (int i = 0; i < JaTickers.Count; i++)
                        {
                            JObject ticker = JObject.Parse(JaTickers[i].ToString());
                            var tvar=Config.Exchange.Symbols.Where(baticker => baticker.Symbol == ticker["s"].ToString()
                            );
                            if (tvar != null)
                            {
                                BATicker t = tvar.ToArray()[0].Ticker;
                                t.High = Convert.ToDouble(ticker["h"].ToString());
                                t.Low = Convert.ToDouble(ticker["l"].ToString());
                                t.Last = Convert.ToDouble(ticker["c"].ToString());
                                t.Sell = Convert.ToDouble(ticker["a"].ToString());
                                t.Buy = Convert.ToDouble(ticker["b"].ToString());
                                t.SellQuantity = Convert.ToDouble(ticker["A"].ToString());
                                t.BuyQuantity = Convert.ToDouble(ticker["B"].ToString());
                                t.Volume = Convert.ToDouble(ticker["q"].ToString());
                                t.VolumeBase = Convert.ToDouble(ticker["v"].ToString());
                                t.PriceChange = Convert.ToDouble(ticker["p"].ToString());
                                t.PriceChangePct = Convert.ToDouble(ticker["P"].ToString());
                                t.Open = Convert.ToDouble(ticker["o"].ToString());
                                t.ExchangeTimeStamp = Convert.ToDouble(ticker["E"].ToString())/1000;
                                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                                if (tvar.ToArray()[0].HisTicker == null)
                                    tvar.ToArray()[0].HisTicker = t.Clone();
                            }
                        }

                        Config.RaiseUpdateTickerEvent();
                        //UpdateTicker(tradingpair, t);

                       
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("ticker err:" + err.Message + e.Data);
                    }



                }
            };
            wsticker.Connect();
        }
        public void StartListenTrade(List<string> Symbols)
        {
            if (Symbols.Count == 0)
                return;
            string CombinedStr = "/stream?streams=";
            for (int i = 0; i < Symbols.Count; i++)
            {
                CombinedStr += Symbols[i] + "@trade";
                if (i < Symbols.Count - 1)
                {
                    CombinedStr += "/";
                }
            }
            wstrade = new WebSocket(spotwssurl + CombinedStr);
            if (Config.Proxy != null)
            {
                wstrade.SetProxy("http://" + Config.Proxy.IP + ":" + Config.Proxy.Port, "", "");
            }
            wstrade.OnMessage += (sender, e) =>
            {
                if (e.IsText)
                {
                    LastCommTimeStamp = DateTime.Now;

                    JObject Jobject = JObject.Parse(e.Data);
                    string streamName = Jobject["stream"].ToString();
                    if (streamName.IndexOf("trade") < 0)
                        return;
                    string rawData = Jobject["data"].ToString();
                    string symbol = streamName.Split('@')[0];
                    
                    try
                    {
                       
                        JObject trade = JObject.Parse(rawData);
                        CommonLab.Trade t = new CommonLab.Trade();
                        
                        t.Amount = Convert.ToDouble(trade["q"].ToString());
                        t.Price = Convert.ToDouble(trade["p"].ToString());
                        t.TradeID = trade["t"].ToString();
                        t.BuyOrderID = trade["b"].ToString();
                        t.SellOrderID = trade["a"].ToString();
                        if (Convert.ToBoolean(trade["m"].ToString()))
                            t.Type = CommonLab.TradeType.Buy;
                        else
                            t.Type = CommonLab.TradeType.Sell;
                        t.ExchangeTimeStamp = Convert.ToDouble(trade["T"].ToString())/1000;
                        t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                        Config.RaiseUpdateTradeEvent(symbol,t);
                        //UpdateTicker(tradingpair, t);


                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("trade err:" + err.Message + e.Data);
                    }



                }
            };
            wstrade.Connect();
        }
        public void Stop()
        {
            if (wsticker.IsAlive)
            {
                wsticker.Close();
            }
            if (wstrade.IsAlive)
            {
                wstrade.Close();
            }
        }
    }
}
