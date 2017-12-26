using CommonLab;
using KFCC.ExchangeInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
        public Fee eFee { get; set; }
        public Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface> SubscribedTradingPairs { get { return _subscribedtradingpairs; } }

        public Proxy proxy { get { return _proxy; } set { _proxy = value; } }

        //public bool SportWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public bool SportThirdPartWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private Account _account;
        public Account Account { get { return _account; } set { _account = value; } }
        public event ExchangeEventWarper.TickerEventHander TickerEvent;
        public event ExchangeEventWarper.DepthEventHander DepthEvent;
        public event ExchangeEventWarper.TradeEventHander TradeEvent;
        public event ExchangeEventWarper.SubscribedEventHander SubscribedEvent;

        public HuobiExchange()
        {
           
        }
        public HuobiExchange(string key, string secret, string uid, string username)
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
                    _subscribedtradingpairs.Add(tradingpairs, new RESTHelper(tp, t, d));
                    //_subscribedtradingpairs.Add(tradingpairs, new PusherHelper(tradingpairs, t, d));
                    //_subscribedtradingpairs[tradingpairs].TradeInfoEvent += OkCoinExchange_TradeInfoEvent;
                }
            }
            if (SubscribedEvent != null)
                SubscribedEvent(this, st, tp);
            return true;
        }

        private void HuobiExchange_TradeInfoEvent(TradingInfo ti, TradeEventType tt)
        {
            if (TickerEvent != null && tt == TradeEventType.TRADE)
            {
                TickerEvent(this, ti.t, (CommonLab.EventTypes)ti.type, ti.tp);
            }
            if (DepthEvent != null && tt == TradeEventType.ORDERS)
            {
                DepthEvent(this, ti.d, (CommonLab.EventTypes)ti.type, ti.tp);
            }
        }

        public Ticker GetTicker(string tradingpair, out string rawresponse)
        {
            DateTime st = DateTime.Now;
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
                t.ExchangeTimeStamp = Convert.ToDouble(obj["ts"].ToString());
                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                t.Delay = (DateTime.Now - st).TotalMilliseconds;
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
            DateTime st = DateTime.Now;
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
            d.ExchangeTimeStamp = Convert.ToDouble(obj["tick"]["ts"]);// Convert.ToDouble(obj["timestamp"].ToString());
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
            var action = $"/v1/order/orders/place";
            var method = "POST";
            string typestr = "";

            switch (type)
            {
                case OrderType.ORDER_TYPE_BUY:
                    typestr = "buy-limit";
                   
                    break;
                case OrderType.ORDER_TYPE_SELL:
                    typestr = "sell-limit";
                  
                    break;
                case OrderType.ORDER_TYPE_MARKETBUY:
                    typestr = "buy-market";
                    break;
                case OrderType.ORDER_TYPE_MARKETSELL:
                    typestr = "sell-market";
                    break;
                default:
                    break;
            }
            var data = new Dictionary<string, object>()
            {
                {"AccessKeyId", _key},
                {"SignatureMethod", SignatureMethod},
                {"SignatureVersion", SignatureVersion},
                {"Timestamp", GetDateTime()},
                //{"account-id", _account.ID },
                //{ "amount", amount},
                //{"symbol", tradingpair },
               // {"type", typestr },
            };
            //if (typestr.IndexOf("limit") > 0)
                //data.Add("price", price);
            var sign =CommonLab.TokenGen.CreateSign_Huobi(Domain,method, action, _secret, data);
            data["Signature"] = sign;
            var url = $"{ApiUrl}{action}?{CommonLab.TokenGen.ConvertQueryString_Huobi(data, true)}";
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            var postData = new Dictionary<string, object>()
            {
                {"account-id", _account.ID},
                {"amount", amount},
                
                {"symbol", tradingpair},
                {"type", typestr}
            };
            RestRequest rr = new RestRequest(Method.POST);
            //rr.AddHeader("ContentType ", "application/json");
            
           
            if (typestr.IndexOf("limit") > 0)
                postData.Add("price", price.ToString("F2"));
            rr.RequestFormat = DataFormat.Json;
            rr.AddJsonBody(JsonConvert.SerializeObject(postData));
            var response = new RestClient(url).Execute(rr);


            string rawresponse = RequestDataSync(url, method, postData, null);

            try
            {
                JObject obj = JObject.Parse(rawresponse);
                
                return Convert.ToInt32(obj["data"]);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        /// <summary>
        /// 生成新的订单但是并不执行
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tradingpair"></param>
        /// <param name="price"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int NewOrder(OrderType type, string tradingpair, double price, double amount)
        {
            CheckSet();
            var action = $"/v1/order/orders";
            var method = "POST";

            var data = new Dictionary<string, object>()
            {
                {"AccessKeyId", _key},
                {"SignatureMethod", SignatureMethod},
                {"SignatureVersion", SignatureVersion},
                {"Timestamp", GetDateTime()},
            };

            var sign = CommonLab.TokenGen.CreateSign_Huobi(Domain, method, action, _secret, data);
            data["Signature"] = sign;
            var url = $"{ApiUrl}{action}?{CommonLab.TokenGen.ConvertQueryString_Huobi(data, true)}";
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }

            RestRequest rr = new RestRequest(Method.POST);
            //rr.AddParameter("account-id", _account.ID);
            //rr.AddParameter("amount", amount);
            //rr.AddParameter("symbol", tradingpair);

            string typestr = "";

            switch (type)
            {
                case OrderType.ORDER_TYPE_BUY:
                    typestr = "buy-limit";
                    //rr.AddParameter("price", price);
                    break;
                case OrderType.ORDER_TYPE_SELL:
                    typestr = "sell-limit";
                    //rr.AddParameter("price", price);
                    break;
                case OrderType.ORDER_TYPE_MARKETBUY:
                    typestr = "buy-market";
                    break;
                case OrderType.ORDER_TYPE_MARKETSELL:
                    typestr = "sell-market";
                    break;
                default:
                    break;
            }

            //rr.AddParameter("type", typestr);
            
            var response = new RestClient(url).Execute(rr);


            string rawresponse = response.Content;

            try
            {
                JObject obj = JObject.Parse(rawresponse);

                return Convert.ToInt32(obj["data"]);
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
            if (t.FromSymbol.ToLower() == "bch")
                return "bcc" + t.ToSymbol.ToLower();
            if (t.ToSymbol.ToLower() == "bch")
                return t.FromSymbol.ToLower()+ "bcc" ;
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


        public string GetOrders(string tradingpair)
        {

            CheckSet();

            string rawresponse = "";

            var action = $"/v1/order/orders";
            var method = "GET";
            var data = new Dictionary<string, object>()
            {
                {"AccessKeyId", _key},
                {"SignatureMethod", SignatureMethod},
                {"SignatureVersion", SignatureVersion},
                {"Timestamp",GetDateTime()},
                {"states", "submitted"},
                {"symbol", tradingpair},
                {"types","buy-limit,sell-limit"},
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
            
            return rawresponse;
        }
               
            
        

        public Order GetOrderStatus(string OrderID, string tradingpair, out string rawresponse)
        {
            CheckSet();
            Order order = null;
            var action = $"/v1/order/orders/{OrderID}";
            var method = "GET";
            var data = new Dictionary<string, object>()
            {
                {"AccessKeyId", _key},
                {"SignatureMethod", SignatureMethod},
                {"SignatureVersion", SignatureVersion},
                {"Timestamp",GetDateTime()},
                {"order-id", OrderID}
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
                if (obj["status"].ToString() == "error")
                {
                    throw (new Exception("error:" + rawresponse));
                }
                JObject orders = JObject.Parse(obj["data"].ToString());

                order = new Order();
                order.Id = orders["id"].ToString();
                order.Amount = Convert.ToDouble(orders["amount"].ToString());
                order.DealAmount = Convert.ToDouble(orders["field-amount"].ToString());
                order.Price = Convert.ToDouble(orders["price"].ToString());
                order.AvgPrice = Convert.ToDouble(orders["field-cash-amount"].ToString()) / Convert.ToDouble(orders["field-amount"].ToString());
                order.Type = GetOrderTypeFromString(orders["type"].ToString());
                order.Status = GetOrderStatus(orders["state"].ToString());
                order.TradingPair = orders["symbol"].ToString();


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

            var action = $"/v1/order/orders/{OrderID}/submitcancel";
            var method = "POST";
            var data = new Dictionary<string, object>()
            {
                {"AccessKeyId", _key},
                {"SignatureMethod", SignatureMethod},
                {"SignatureVersion", SignatureVersion},
                {"Timestamp",GetDateTime()},
                {"order-id", OrderID}
            };
            var sign = CommonLab.TokenGen.CreateSign_Huobi(Domain, method, action, _secret, data);
            data["Signature"] = sign;
            var url = $"{ApiUrl}{action}?{CommonLab.TokenGen.ConvertQueryString_Huobi(data, true)}";

            rawresponse = RequestDataSync(url, method, null, null);

            
            try
            {
                JObject obj = JObject.Parse(rawresponse);
                if (obj["status"].ToString() == "error")
                {
                    throw (new Exception("error:" + rawresponse));
                }
                if (obj["status"].ToString() == "ok")
                {
                    if (obj["data"].ToString() == OrderID)
                    {
                        return true;

                    }
                }


            }
            catch (Exception e)
            {
                Exception err = new Exception("订单取消解析json失败" + e.Message);
                throw err;
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
            if (str.ToLower() == "buy-limit")
                return OrderType.ORDER_TYPE_BUY;
            if (str.ToLower() == "sell-limit")
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
            if (str.ToLower() == "canceled")
                return OrderStatus.ORDER_STATE_CANCELED;
            if (str.ToLower() == "submitted")
                return OrderStatus.ORDER_STATE_PENDING;
            if (str.ToLower() == "partial-filled")
                return OrderStatus.ORDER_STATE_PARTITAL;
            if (str.ToLower() == "filled ")
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
        private string RequestDataSync(string url, string method, Dictionary<string, object> param, WebHeaderCollection headers)
        {
            string resp = string.Empty;
           
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Method = method;
            if (_proxy != null)
            {
                request.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            if (headers != null)
            {
                foreach (var key in headers.AllKeys)
                {
                    request.Headers.Add(key, headers[key]);
                }
            }
            try
            {
                if (method == "POST" && param != null)
                {
                    byte[] bs = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(param));
                    request.ContentType = "application/json";
                    request.ContentLength = bs.Length;
                    using (var reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }
                }
                //如果是Get 请求参数附加在URL之后
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (resp == null)
                        throw new Exception("Response is null");
                    resp = GetResponseBody(response);
                    //httpCode = (int)response.StatusCode;
                }
            }
            catch (WebException ex)
            {
                using (HttpWebResponse response = ex.Response as HttpWebResponse)
                {
                    resp = GetResponseBody(response);
                    //httpCode = (int)response.StatusCode;
                }
            }
            return resp;
        }
        private string GetResponseBody(HttpWebResponse response)
        {
            var readStream = new Func<Stream, string>((stream) =>
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            });

            using (var responseStream = response.GetResponseStream())
            {
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    using (GZipStream stream = new GZipStream(responseStream, CompressionMode.Decompress))
                    {
                        return readStream(stream);
                    }
                }
                if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    using (DeflateStream stream = new DeflateStream(responseStream, CompressionMode.Decompress))
                    {
                        return readStream(stream);
                    }
                }
                return readStream(responseStream);
            }
        }

       

        private string ConvertQueryString(Dictionary<string, object> data, bool urlencode = false)
        {
            var stringbuilder = new StringBuilder();
            foreach (var item in data)
            {
                stringbuilder.AppendFormat("{0}={1}&", item.Key, urlencode ? Uri.EscapeDataString(item.Value.ToString()) : item.Value.ToString());
            }
            stringbuilder.Remove(stringbuilder.Length - 1, 1);
            return stringbuilder.ToString();
        }

        public List<Order> GetHisOrders(string tradingpair)
        {
            throw new NotImplementedException();
        }
    }
}
