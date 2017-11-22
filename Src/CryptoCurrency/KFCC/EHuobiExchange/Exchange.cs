﻿using CommonLab;
using KFCC.ExchangeInterface;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KFCC.EHuobiExchange
{
    public class HuobiExchange : IExchanges
    {
        private static string _secret;
        private static string _key;
        private static string _uid;
        private static string _username;
        private Proxy _proxy = null;
        private const string SignatureMethod = "HmacSHA256";
        private const int SignatureVersion = 2;
        static private Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface> _subscribedtradingpairs = null;
        static private string ApiUrl = @"https://api.huobi.pro";
        static private string Domain = "api.huobi.pro";
        public string Name { get { return "Huobi"; } }
        public string ExchangeUrl { get { return "huobi.pro"; } }
        public string Remark { get { return "Huobi pro exchange remark"; } }
        public string Secret { get { return _secret; } set { _secret = value; } }
        public string Key { get { return _key; } set { _key = value; } }
        public string UID { get { return _uid; } set { _uid = value; } }
        public string UserName { get { return _username; } set { _username = value; } }

        public Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface> SubscribedTradingPairs { get { return _subscribedtradingpairs; } }

        public Proxy proxy { get { return _proxy; } set { _proxy = value; } }

        //public bool SportWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public bool SportThirdPartWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private Account _account;
        public Account Account { get { return _account; } set { _account = value; } }
        public event ExchangeEventWarper.TickerEventHander TickerEvent;
        public event ExchangeEventWarper.DepthEventHander DepthEvent;
        public HuobiExchange(string key, string secret, string uid, string username)
        {
            _key = key;
            _secret = secret;
            _uid = uid;
            _username = username;
        }
        public bool Subscribe(CommonLab.TradePair tp, SubscribeTypes st)
        {
            //throw new NotImplementedException();
            //订阅 
            string tradingpairs = GetLocalTradingPairString(tp, st);
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
                    _subscribedtradingpairs.Add(tradingpairs, new WssHelper(tp, t, d));
                    _subscribedtradingpairs[tradingpairs].TradeInfoEvent += HuobiExchange_TradeInfoEvent;
                }
            }
            else if (st == SubscribeTypes.RESTAPI)
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
                    //_subscribedtradingpairs.Add(tradingpairs, new PusherHelper(tradingpairs, t, d));
                    //_subscribedtradingpairs[tradingpairs].TradeInfoEvent += OkCoinExchange_TradeInfoEvent;
                }
            }

            return true;
        }

        private void HuobiExchange_TradeInfoEvent(TradingInfo ti, TradeEventType tt)
        {
            if (TickerEvent != null && tt == TradeEventType.TRADE)
            {
                TickerEvent(this, ti.t, (CommonLab.EventTypes)ti.type, ti.tradingpair);
            }
            if (DepthEvent != null && tt == TradeEventType.ORDERS)
            {
                DepthEvent(this, ti.d, (CommonLab.EventTypes)ti.type, ti.tradingpair);
            }
        }

        public Ticker GetTicker(string tradingpair, out string rawresponse)
        {
            //throw new NotImplementedException();
            string url = ApiUrl+ "/market/detail/merged?symbol="+tradingpair;
            rawresponse = CommonLab.Utility.GetHttpContent(url, "GET", "", _proxy);
            CommonLab.Ticker t = new Ticker();
            JObject obj = JObject.Parse(rawresponse);
            try
            {
                JObject ticker = JObject.Parse(obj["tick"].ToString());
                t.High = Convert.ToDouble(ticker["high"].ToString());
                t.Low = Convert.ToDouble(ticker["low"].ToString());
                t.Last = Convert.ToDouble(ticker["close"].ToString());
                t.Sell = Convert.ToDouble(JArray.Parse(ticker["ask"].ToString())[0]);
                t.Buy = Convert.ToDouble(JArray.Parse(ticker["bid"].ToString())[0]);
                t.Volume = Convert.ToDouble(ticker["vol"].ToString());
                t.Open = Convert.ToDouble(ticker["open"].ToString()); ;// Convert.ToDouble(ticker["open"].ToString());
                t.ExchangeTimeStamp = Convert.ToDouble(obj["ts"].ToString())/1000;
                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);
                UpdateTicker(tradingpair, t);
            }
            catch(Exception e)
            {
                throw e;
            }
            return t;
        }

        public Depth GetDepth(string tradingpair, out string rawresponse)
        {
            string url = ApiUrl+ "/market/depth?symbol="+tradingpair+"&type=step0";
            rawresponse = CommonLab.Utility.GetHttpContent(url, "GET", "", _proxy);
            CommonLab.Depth d = new Depth();
            d.Asks = new List<MarketOrder>();
            d.Bids = new List<MarketOrder>();
            JObject obj = JObject.Parse(rawresponse);
            JArray jasks = JArray.Parse(obj["tick"]["asks"].ToString());
            for (int i = 0; i < jasks.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[0]);
                m.Amount = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[1]);
                d.Asks.Add(m);
            }

            JArray jbids = JArray.Parse(obj["tick"]["bids"].ToString());
            for (int i = 0; i < jbids.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[0]);
                m.Amount = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[1]);
                d.Bids.Add(m);
            }
            d.ExchangeTimeStamp = Convert.ToDouble(obj["tick"]["ts"])/1000;// Convert.ToDouble(obj["timestamp"].ToString());
            d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStamp(DateTime.Now);
            UpdateDepth(tradingpair, d);
            return d;
        }
        /// <summary>
        /// 更新深度数据
        /// </summary>
        /// <param name="d"></param>
        protected void UpdateDepth(string tradingpair, Depth d)
        {
            if (SubscribedTradingPairs.ContainsKey(tradingpair))
            {
                //((PusherHelper)SubscribedTradingPairs[tradingpair]).UpdateDepth(d);
            }
        }
        /// <summary>
        /// 更新深度数据
        /// </summary>
        /// <param name="t"></param>
        protected void UpdateTicker(string tradingpair, Ticker t)
        {
            if (SubscribedTradingPairs.ContainsKey(tradingpair))
            {
                //((PusherHelper)SubscribedTradingPairs[tradingpair]).UpdateTicker(t);
            }
        }
        protected int Trade(OrderType type, string tradingpair, double price, double amount)
        {
            CheckSet();
            string url = GetPublicApiURL("", "trade.do");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            RestRequest rr = new RestRequest(Method.POST);
            rr.AddParameter("api_key", _key);
            rr.AddParameter("symbol", tradingpair);
            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("api_key", _key);
            paras.Add("symbol", tradingpair);
            switch (type)
            {
                case OrderType.ORDER_TYPE_BUY:
                    paras.Add("type", "buy");
                    paras.Add("price", price.ToString());
                    rr.AddParameter("type", "buy");
                    rr.AddParameter("price", price.ToString());
                    break;
                case OrderType.ORDER_TYPE_SELL:
                    paras.Add("type", "sell");
                    paras.Add("price", price.ToString());
                    rr.AddParameter("type", "sell");
                    rr.AddParameter("price", price.ToString());
                    break;
                case OrderType.ORDER_TYPE_MARKETBUY:
                    paras.Add("type", "buy_market");
                    rr.AddParameter("type", "buy_market");
                    break;
                case OrderType.ORDER_TYPE_MARKETSELL:
                    paras.Add("type", "sell_market");
                    rr.AddParameter("type", "sell_market");
                    break;

            }
            paras.Add("amount", amount.ToString());
            rr.AddParameter("amount", amount.ToString());
            String sign = "";//MD5Util.buildMysignV1(paras, _secret);
            rr.AddParameter("sign", sign);

            var response = new RestClient(url).Execute(rr);


            string rawresponse = response.Content;

            try
            {
                JObject obj = JObject.Parse(rawresponse);
                if (!Convert.ToBoolean(obj["result"]))
                {
                    throw (new Exception("error:" + rawresponse));
                }
                return Convert.ToInt32(obj["order_id"]);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tradepair"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public string GetPublicApiURL(string tradepair, string method)
        {
            //https://www.okex.com/api/v1/ticker.do?symbol=ltc_btc
            return ApiUrl + method + tradepair;
        }

        public string GetLocalTradingPairString(TradePair t, SubscribeTypes st = CommonLab.SubscribeTypes.RESTAPI)
        {
            //if (st == SubscribeTypes.WSS)
            //{
            //    if (t.FromSymbol.ToLower() == "btc" && t.ToSymbol.ToLower() == "usd")
            //        return "";
            //    return "_" + t.FromSymbol.ToLower() + t.ToSymbol.ToLower();
            //}
            //else if (st == SubscribeTypes.RESTAPI)
            //{
            //    return t.FromSymbol.ToLower() + "_" + t.ToSymbol.ToLower();
            //}
            return t.FromSymbol.ToLower() + t.ToSymbol.ToLower();
        }
        private string GetDateTime() => DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

        public Account GetAccount(out string rawresponse)
        {
            CheckSet();

            rawresponse = "";
            if (_account == null)
            {
                #region
                var action = "/v1/account/accounts";
                var method = "GET";
                var data = new Dictionary<string, object>()
            {
                {"AccessKeyId", _key},
                {"SignatureMethod", SignatureMethod},
                {"SignatureVersion", SignatureVersion},
                {"Timestamp",GetDateTime()},
            };
                var sign = CommonLab.TokenGen.CreateSign_Huobi(Domain, method, action, _secret, data);
                data["Signature"] = sign;
                var url = $"{ApiUrl}{action}?{CommonLab.TokenGen.ConvertQueryString_Huobi(data, true)}";

                #endregion


                RestClient rc = new RestClient(url);
                if (_proxy != null)
                {
                    rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
                }
                RestRequest rr = new RestRequest(Method.GET);

                var response = new RestClient(url).Execute(rr);
                rawresponse = response.Content;
                try
                {
                    _account = new Account();
                    JObject obj = JObject.Parse(rawresponse);
                    _account.ID = obj["data"][0]["id"].ToString();
                }
                catch
                {

                }


            }

            if(_account != null)
            {
                var action = $"/v1/account/accounts/" + _account.ID + "/balance";
                var method = "GET";
                var data = new Dictionary<string, object>()
            {
                           {"AccessKeyId", _key},
                {"SignatureMethod", SignatureMethod},
                {"SignatureVersion", SignatureVersion},
                {"Timestamp",GetDateTime()},

                {"account-id", _account.ID}
            };
                var sign = CommonLab.TokenGen.CreateSign_Huobi(Domain, method, action, _secret, data);
                data["Signature"] = sign;

                var url = $"{ApiUrl}{action}?{CommonLab.TokenGen.ConvertQueryString_Huobi(data, true)}";
                RestClient rc = new RestClient(url);
                if (_proxy != null)
                {
                    rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
                }
                RestRequest rr = new RestRequest(Method.GET);

                var response = new RestClient(url).Execute(rr);
                rawresponse = response.Content;
                try
                {
                    JObject obj = JObject.Parse(rawresponse);
                    JArray jlist = JArray.Parse(obj["data"]["list"].ToString());
                    for (int i = 0; i < jlist.Count; i++)
                    {
                       
                        string Name = jlist[i]["currency"].ToString();
                        string type = jlist[i]["type"].ToString();
                        double balance = Convert.ToDouble(jlist[i]["balance"].ToString());
                        if (_account.Balances.ContainsKey(Name))
                        {

                            _account.Balances[Name].borrow =0;
                            if(type== "trade")
                            _account.Balances[Name].available = balance;
                            if(type== "frozen")
                            _account.Balances[Name].reserved = balance;
                            _account.Balances[Name].balance = _account.Balances[Name].available + _account.Balances[Name].reserved;
                        }
                        else
                        {
                            Balance b = new Balance();
                            b.borrow = 0;
                            if (type == "trade")
                                b.available = balance;
                            if (type == "frozen")
                                b.reserved = balance;
                            b.balance = b.available + b.reserved;

                         

                            _account.Balances.Add(Name, b);
                        }
                    }
                }
                catch
                {

                }
            }
            return _account;
        }

        public Order GetOrderStatus(string OrderID, string tradingpair, out string rawresponse)
        {
            CheckSet();
            Order order = null;
            string url = GetPublicApiURL("", "order_info.do");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            RestRequest rr = new RestRequest(Method.POST);
            rr.AddParameter("api_key", _key);
            rr.AddParameter("symbol", tradingpair);
            rr.AddParameter("order_id", OrderID);

            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("api_key", _key);
            paras.Add("symbol", tradingpair);
            paras.Add("order_id", OrderID);
            String sign = "";//MD5Util.buildMysignV1(paras, _secret);
            rr.AddParameter("sign", sign);
            var response = new RestClient(url).Execute(rr);


            rawresponse = response.Content;
            try
            {
                JObject obj = JObject.Parse(rawresponse);
                if (!Convert.ToBoolean(obj["result"]))
                {
                    throw (new Exception("error:" + rawresponse));
                }
                JArray orders = JArray.Parse(obj["orders"].ToString());
                if (orders.Count > 0)
                {
                    order = new Order();
                    order.Id = orders[0]["order_id"].ToString();
                    order.Amount = Convert.ToDouble(orders[0]["amount"].ToString());
                    order.DealAmount = Convert.ToDouble(orders[0]["deal_amount"].ToString());
                    order.Price = Convert.ToDouble(orders[0]["price"].ToString());
                    order.Type = GetOrderTypeFromString(orders[0]["type"].ToString());
                    order.Status = GetOrderStatus(orders[0]["status"].ToString());
                    order.TradingPair = orders[0]["symbol"].ToString();
                }

            }
            catch (Exception e)
            {
                Exception err = new Exception("订单获取解析json失败" + e.Message);
                throw err;
            }
            return order;

        }

        public bool CancelOrder(string OrderID, string tradingpair, out string rawresponse)
        {
            CheckSet();

            string url = GetPublicApiURL("", "cancel_order.do");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            RestRequest rr = new RestRequest(Method.POST);
            rr.AddParameter("api_key", _key);
            rr.AddParameter("symbol", tradingpair);
            rr.AddParameter("order_id", OrderID);

            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("api_key", _key);
            paras.Add("symbol", tradingpair);
            paras.Add("order_id", OrderID);
            String sign = "";//MD5Util.buildMysignV1(paras, _secret);
            rr.AddParameter("sign", sign);
            var response = new RestClient(url).Execute(rr);


            rawresponse = response.Content;
            try
            {
                JObject obj = JObject.Parse(rawresponse);
                if (Convert.ToBoolean(obj["result"]))
                {
                    return true;
                }


            }
            catch (Exception e)
            {
                //这里应该抛出错误
            }
            return false;
        }

        public bool CancelAllOrders()
        {
            throw new NotImplementedException();
        }

        public int Buy(string Symbol, double Price, double Amount)
        {
            if (Price > 0)
            {
                return Trade(OrderType.ORDER_TYPE_BUY, Symbol, Price, Amount);
            }
            else if (Price == 0)
            {
                return Trade(OrderType.ORDER_TYPE_MARKETBUY, Symbol, Price, Amount);
            }
            return 0;
        }

        public int Sell(string Symbol, double Price, double Amount)
        {
            if (Price > 0)
            {
                return Trade(OrderType.ORDER_TYPE_SELL, Symbol, Price, Amount);
            }
            else if (Price == 0)
            {
                return Trade(OrderType.ORDER_TYPE_MARKETSELL, Symbol, Price, Amount);
            }
            return 0;
        }

        public void CheckSet()
        {
            if (string.IsNullOrEmpty(_key) || string.IsNullOrEmpty(_secret))
            {
                throw new Exception("交易所设置有问题！");
            }
        }
        static public OrderType GetOrderTypeFromString(string str)
        {
            if (str.ToLower() == "buy")
                return OrderType.ORDER_TYPE_BUY;
            if (str.ToLower() == "sell")
                return OrderType.ORDER_TYPE_SELL;
            if (str.ToLower() == "buy_market")
                return OrderType.ORDER_TYPE_MARKETBUY;
            if (str.ToLower() == "sell_market")
                return OrderType.ORDER_TYPE_MARKETSELL;
            return OrderType.ORDER_TYPE_UNKOWN;
        }
        static public OrderStatus GetOrderStatus(string str)
        {
            //status:-1:已撤销  0:未成交  1:部分成交  2:完全成交 4:撤单处理中
            if (str.ToLower() == "-1")
                return OrderStatus.ORDER_STATE_CANCELED;
            if (str.ToLower() == "0")
                return OrderStatus.ORDER_STATE_PENDING;
            if (str.ToLower() == "1")
                return OrderStatus.ORDER_STATE_PARTITAL;
            if (str.ToLower() == "2")
                return OrderStatus.ORDER_STATE_SUCCESS;
            if (str.ToLower() == "4")
                return OrderStatus.ORDER_STATE_CANCELING;
            return OrderStatus.ORDER_STATE_UNKOWN;
        }
        private Exception JsonError(JObject obj)
        {
            if (obj.Property("error_code") == null || obj.Property("error_code").ToString() == "")
            {
                return new Exception("\"error_code\" not found in raw json string!");
            }
            else
                return new Exception("");// SpotErrcode2Msg.Prase(obj["error_code"].ToString()));
        }

        public void DisSubcribe(TradePair tp, SubscribeTypes st)
        {
            throw new NotImplementedException();
        }
    }
}
