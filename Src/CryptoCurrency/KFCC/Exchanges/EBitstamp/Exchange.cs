using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace KFCC.EBitstamp
{
    public class BitstampExchange : KFCC.ExchangeInterface.IExchanges
    {
        private static string _secret;
        private static string _key;
        private static string _uid;
        private static string _username;
        private Proxy _proxy = null;
      
        static private string ApiUrl = @"https://www.bitstamp.net/api/v2/";
        //static private BitstampExchange _instance=null;

       

        static private Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface> _subscribedtradingpairs = null;
 

        public string Name { get { return "Bitstamp"; } }
        public string ExchangeUrl {  get { return "www.bitstamp.net"; } }
        public string Remark { get { return "bitstamp exchange remark"; } }
        public string Secret { get { return _secret; } set { _secret = value; } }
        public string Key { get { return _key; } set { _key = value; } }
        public string UID { get { return _uid; } set { _uid = value; } }
        public string UserName { get { return _username; } set { _username = value; } }
     
        public Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface> SubscribedTradingPairs { get { return _subscribedtradingpairs; } }

        public Proxy proxy { get { return _proxy; }set { _proxy = value; } }
        private Account _account;
        public Account Account {get{ return _account; }set { _account = value; } }

        public Fee eFee { get; set; }

        //public bool SportWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public bool SportThirdPartWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event ExchangeEventWarper.TickerEventHander TickerEvent;
        public event ExchangeEventWarper.DepthEventHander DepthEvent;
        public event ExchangeEventWarper.TradeEventHander TradeEvent;
        public event ExchangeEventWarper.SubscribedEventHander SubscribedEvent;

        public BitstampExchange()
        {

        }
        
        public BitstampExchange(string key, string secret, string uid, string username)
        {
            _key = key;
            _secret = secret;
            _uid = uid;
            _username = username;
        }
        public void SetupExchage(string key, string secret, string uid, string username)
        {
            _key = key;
            _secret = secret;
            _uid = uid;
            _username = username;
        }
        public void SetupFee(string maker, string taker)
        {
            eFee = new Fee();
            eFee.MakerFee = Convert.ToDouble(maker) / 100;
            eFee.TakerFee = Convert.ToDouble(taker) / 100;
        }
        //public static BitstampExchange Create(string key,string secret,string uid,string username)
        //{
        //    if (_instance == null)
        //    {
        //        _instance = new BitstampExchange(key, secret, uid, username);
        //    }
        //    else
        //    {
        //        if (_key == key && _secret == secret && _uid == uid && _username == username)
        //        {
        //            return _instance;
        //        }
        //        else
        //        {

        //        }
        //    }
        //    return _instance; 

        //}

        public bool Subscribe(CommonLab.TradePair tp, SubscribeTypes st)
        {
            //throw new NotImplementedException();
            //订阅 
            string tradingpairs = GetLocalTradingPairString(tp,st);
            if (st == SubscribeTypes.WSS)
            {
                if (_subscribedtradingpairs == null)
                {
                    _subscribedtradingpairs = new Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface>();
                    
                }
                if (_subscribedtradingpairs.ContainsKey(tradingpairs))
                {
                    //有这个订阅
                }
                else
                {
                    string raw;
                    Ticker t = GetTicker(GetLocalTradingPairString(tp), out raw);
                    Depth d = GetDepth(GetLocalTradingPairString(tp), out raw);
                    _subscribedtradingpairs.Add(tradingpairs, new PusherHelper(tradingpairs,t,d,tp));
                    _subscribedtradingpairs[tradingpairs].TradeInfoEvent += BitstampExchange_TradeInfoEvent;
                }
            }
            if (SubscribedEvent != null)
                SubscribedEvent(this, st, tp);
            return true;
        }

        private void BitstampExchange_TradeInfoEvent(TradingInfo ti,TradeEventType tt)
        {
            if (TickerEvent != null&&tt==TradeEventType.TRADE)
            {
                TickerEvent(this, ti.t, (CommonLab.EventTypes)ti.type, ti.tp);
            }
            if (DepthEvent != null&&tt==TradeEventType.ORDERS)
            {
                DepthEvent(this, ti.d, (CommonLab.EventTypes)ti.type, ti.tp);
            }
        }

        public Ticker GetTicker(string tradingpair,  out string rawresponse)
        {
            DateTime st = DateTime.Now;
            //throw new NotImplementedException();
            string url = GetPublicApiURL(tradingpair,"ticker");
            rawresponse = CommonLab.Utility.GetHttpContent(url,"GET","",_proxy);
            CommonLab.Ticker t = new Ticker();
            JObject obj = JObject.Parse(rawresponse);
            t.High = Convert.ToDouble(obj["high"].ToString());
            t.Low = Convert.ToDouble(obj["low"].ToString());
            t.Last = Convert.ToDouble(obj["last"].ToString());
            t.Sell = Convert.ToDouble(obj["ask"].ToString());
            t.Buy = Convert.ToDouble(obj["bid"].ToString());
            t.Volume = Convert.ToDouble(obj["volume"].ToString());
            t.Open = Convert.ToDouble(obj["open"].ToString());
            t.ExchangeTimeStamp= Convert.ToDouble(obj["timestamp"].ToString())*1000;
            t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
            t.Delay = (DateTime.Now - st).TotalMilliseconds;
            UpdateTicker(tradingpair, t);
            return t;
        }

        public Depth GetDepth(string tradingpair, out string rawresponse)
        {
            DateTime st = DateTime.Now;
            string url = GetPublicApiURL(tradingpair, "order_book");
            rawresponse = CommonLab.Utility.GetHttpContent( url, "GET", "", _proxy);
            CommonLab.Depth d = new Depth();
            d.Asks = new List<MarketOrder>();
            d.Bids = new List<MarketOrder>();
            JObject obj = JObject.Parse(rawresponse);
            JArray jasks = JArray.Parse(obj["asks"].ToString());
            for (int i = 0; i < jasks.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price =Convert.ToDouble(JArray.Parse(jasks[i].ToString())[0]);
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
            d.ExchangeTimeStamp = Convert.ToDouble(obj["timestamp"].ToString())*1000;
            d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
            d.Delay = (DateTime.Now - st).TotalMilliseconds;
            UpdateDepth(tradingpair, d);
            return d;
        }
        public Trade[] GetTrades(string tradepair, out string rawresponse, string since = "0")
        {
            rawresponse = "";
            return null;
        }
        /// <summary>
        /// 更新深度数据
        /// </summary>
        /// <param name="d"></param>
        protected void UpdateDepth(string tradingpair, Depth d)
        {
            if (SubscribedTradingPairs != null)
                if (SubscribedTradingPairs.ContainsKey(tradingpair))
                {
                    ((PusherHelper)SubscribedTradingPairs[tradingpair]).UpdateDepth(d);
                }
        }
        /// <summary>
        /// 更新深度数据
        /// </summary>
        /// <param name="t"></param>
        protected void UpdateTicker(string tradingpair, Ticker t)
        {
            if (SubscribedTradingPairs != null)
                if (SubscribedTradingPairs.ContainsKey(tradingpair))
                {
                    ((PusherHelper)SubscribedTradingPairs[tradingpair]).UpdateTicker(t);
                }
        }
        public string GetPublicApiURL(string tradingpair, string method)
        {
            return ApiUrl + method + "/" + tradingpair;
        }

        public string GetLocalTradingPairString(TradePair t,SubscribeTypes st=CommonLab.SubscribeTypes.RESTAPI)
        {
            if (st == SubscribeTypes.WSS)
            {
                if (t.FromSymbol.ToLower() == "btc" && t.ToSymbol.ToLower() == "usd")
                    return "";
                return t.FromSymbol.ToLower() + t.ToSymbol.ToLower();
            }
            else if (st == SubscribeTypes.RESTAPI)
            {
                return t.FromSymbol.ToLower() + t.ToSymbol.ToLower();
            }
            return t.FromSymbol.ToLower() + t.ToSymbol.ToLower();
        }

        public Account GetAccount(out string rawresponse)
        {
            CheckSet();
            Account account = new Account();
            string url = GetPublicApiURL("", "balance");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            var response = new RestClient(url).Execute(GetAuthenticatedRequest(Method.POST));


            rawresponse = response.Content;
            return account;
        }

        public Order GetOrderStatus(string OrderID, string tradingpair, out string rawresponse)
        {
            throw new NotImplementedException();
        }

        public bool CancelOrder(string OrderID, string tradingpair, out string rawresponse)
        {
            throw new NotImplementedException();
        }

        public bool CancelAllOrders(string tradingpair = "")
        {
            throw new NotImplementedException();
        }

        public string Buy(string Symbol, double Price, double Amount)
        {
            throw new NotImplementedException();
        }

        public string Sell(string Symbol, double Price, double Amount)
        {
            throw new NotImplementedException();
        }

        public void CheckSet()
        {
            if (string.IsNullOrEmpty(_key) || string.IsNullOrEmpty(_secret))
            {
                throw new Exception("交易所设置有问题！");
            }
        }
        private RestRequest GetAuthenticatedRequest(Method method)
        {
            var request = new RestRequest(method);
            string nonce = TimerHelper.GetTimeStampNonce();
            var msg = string.Format("{0}{1}{2}", nonce, _uid, _key);
            var sigsignature = TokenGen.CreateToken(msg, _secret, Encoding.ASCII);        
            request.AddParameter("key",_key);
            request.AddParameter("nonce", nonce);
            request.AddParameter("signature", sigsignature);
            return request;
        }

        public void DisSubcribe(TradePair tp, SubscribeTypes st)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetHisOrders(string tradingpair)
        {
            throw new NotImplementedException();
        }
    }
}
