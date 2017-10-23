using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using KFCC.ExchangeInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
namespace KFCC.EBitstamp
{
    public class BitstampExchange : IExchanges
    {
        private string _secret;
        private string _key;
        private string _uid;
        private string _username;
        static private CommonLab.Ticker _lastticker;
        static private CommonLab.Depth _lastdepth;
      
        static private string ApiUrl = @"https://www.bitstamp.net/api/v2/";
        static private BitstampExchange _instance=null;
        public string Name { get { return "Bitstamp"; } }
        public string ExchangeUrl {  get { return "www.bitstamp.net"; } }
        public string Remark { get { return "bitstamp exchange remark"; } }
        public string Secret { get { return _secret; } set { _secret = value; } }
        public string Key { get { return _key; } set { _key = value; } }
        public string UID { get { return _uid; } set { _uid = value; } }
        public string UserName { get { return _username; } set { _username = value; } }
     
        public Dictionary<string, int> SubscribeTradingPairs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Ticker LastTicker { get { return _lastticker; } set { _lastticker = value; } }
        public Depth LastDepth { get { return _lastdepth; } set { _lastdepth = value; } }


        //public bool SportWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public bool SportThirdPartWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event ExchangeEventWarper.TickerEventHander TickerEvent;
        public event ExchangeEventWarper.DepthEventHander DepthEvent;
        private  BitstampExchange(string key, string secret, string uid, string username)
        {
            _key = key;
            _secret = secret;
            _uid = uid;
            _username = username;
        }
        public static BitstampExchange Create(string key,string secret,string uid,string username)
        {
            if (_instance == null)
            {
                _instance = new BitstampExchange(key,secret,uid,username);
            }
            return _instance; 
            
        }

        public bool Subscribe(string tradingpairs, SubscribeTypes st)
        {
            throw new NotImplementedException();
        }

        public Ticker GetTicker(string tradingpair,  out string rawresponse, CommonLab.Proxy p = null)
        {
            //throw new NotImplementedException();
            string url = GetPublicApiURL(tradingpair,"ticker");
            rawresponse = CommonLab.Utility.GetHttpContent(url,p);
            CommonLab.Ticker t = new Ticker();
            JObject obj = JObject.Parse(rawresponse);
            t.High = Convert.ToDouble(obj["high"].ToString());
            t.Low = Convert.ToDouble(obj["low"].ToString());
            t.Last = Convert.ToDouble(obj["last"].ToString());
            t.Sell = Convert.ToDouble(obj["ask"].ToString());
            t.Buy = Convert.ToDouble(obj["bid"].ToString());
            t.Volume = Convert.ToDouble(obj["high"].ToString());
            t.Open = Convert.ToDouble(obj["open"].ToString());
            t.ExchangeTimeStamp= Convert.ToDouble(obj["timestamp"].ToString());
            t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);
            _lastticker = t;
            return t;
        }

        public Depth GetDepth(string tradingpair, out string rawresponse, CommonLab.Proxy p = null)
        {
            string url = GetPublicApiURL(tradingpair, "order_book");
            rawresponse = CommonLab.Utility.GetHttpContent(url, p);
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
            d.ExchangeTimeStamp = Convert.ToDouble(obj["timestamp"].ToString());
            d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);
            _lastdepth = d;
            return d;
        }

        public string GetPublicApiURL(string tradingpair, string method)
        {
            return ApiUrl + method + "/" + tradingpair;
        }
    }
}
