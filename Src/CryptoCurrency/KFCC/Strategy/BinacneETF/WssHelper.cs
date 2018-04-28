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
        const string spotwssurl = "wss://stream.binance.com:9443/ws/";
        public DateTime LastCommTimeStamp { get; set; }
        public WssHelper()
        {

        }
        public void StartListen()
        {
            wsticker = new WebSocket(spotwssurl + "!ticker@arr");
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
                                t.Last = Convert.ToDouble(ticker["C"].ToString());
                                t.Sell = Convert.ToDouble(ticker["a"].ToString());
                                t.Buy = Convert.ToDouble(ticker["b"].ToString());
                                t.SellQuantity = Convert.ToDouble(ticker["A"].ToString());
                                t.BuyQuantity = Convert.ToDouble(ticker["B"].ToString());
                                t.Volume = Convert.ToDouble(ticker["q"].ToString());
                                t.VolumeBase = Convert.ToDouble(ticker["v"].ToString());
                                t.PriceChange = Convert.ToDouble(ticker["p"].ToString());
                                t.PriceChangePct = Convert.ToDouble(ticker["P"].ToString());
                                t.Open = Convert.ToDouble(ticker["o"].ToString());
                                t.ExchangeTimeStamp = Convert.ToDouble(ticker["E"].ToString());
                                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
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
        public void Stop()
        {
            if (wsticker.IsAlive)
            {
                wsticker.Close();
            }
        }
    }
}
