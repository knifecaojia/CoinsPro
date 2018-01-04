using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using KFCC.ExchangeInterface;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace KFCC.Exchanges.EQuoine
{
    public class EQuoineExchange : KFCC.ExchangeInterface.IExchanges
    {
        private static string _secret;
        private static string _key;
        private static string _uid;
        private static string _username;
        private Proxy _proxy = null;
        static private Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface> _subscribedtradingpairs = null;
        static private string ApiUrl = "https://api.quoine.com";
   

        public string Name { get { return "Quoine"; } }
        public string ExchangeUrl { get { return "www.Quoine.com"; } }
        public string Remark { get { return "Quoine exchange remark"; } }
        public string Secret { get { return _secret; } set { _secret = value; } }
        public string Key { get { return _key; } set { _key = value; } }
        public string UID { get { return _uid; } set { _uid = value; } }
        public string UserName { get { return _username; } set { _username = value; } }

        public Dictionary<string, KFCC.ExchangeInterface.SubscribeInterface> SubscribedTradingPairs { get { return _subscribedtradingpairs; } }
        public Fee eFee { get; set; }
        public Proxy proxy { get { return _proxy; } set { _proxy = value; } }
        private Account _account;
        public Account Account { get { return _account; } set { _account = value; } }
        //public bool SportWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public bool SportThirdPartWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event ExchangeEventWarper.TickerEventHander TickerEvent;
        public event ExchangeEventWarper.DepthEventHander DepthEvent;
        public event ExchangeEventWarper.TradeEventHander TradeEvent;
        public event ExchangeEventWarper.SubscribedEventHander SubscribedEvent;

        private RestRequest restrequest;

        public EQuoineExchange(string key, string secret, string uid, string username)
        {
            _key = key;
            _secret = secret;
            _uid = uid;
            _username = username;
            GetProducts();
        }
        public EQuoineExchange()
        {

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
        private Dictionary<string, int> ExchangeProducts = new Dictionary<string, int>();
       
        private void GetProducts()
        {
            string url = ApiUrl + "/products";
            restrequest = new RestRequest(Method.GET);
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            restrequest.AddHeader("X-Quoine-API-Version","2");
            var rr = rc.Execute(restrequest);
            string raw = rr.Content;
            JArray products = JArray.Parse(raw);
            if (products.Count > 0)
                for (int i = 0; i < products.Count; i++)
                {
                    if (ExchangeProducts.ContainsKey(products[i]["currency_pair_code"].ToString()))
                    {
                        ExchangeProducts[products[i]["currency_pair_code"].ToString()] = Convert.ToInt32(products[i]["id"].ToString());
                    }
                    else
                    {
                        ExchangeProducts.Add(products[i]["currency_pair_code"].ToString(), Convert.ToInt32(products[i]["id"].ToString()));
                    }
                }

        }
        
        public bool Subscribe(CommonLab.TradePair tp, SubscribeTypes st)
        {
            //throw new NotImplementedException();
            //订阅 
            string tradingpairs = GetLocalTradingPairString(tp, st);
            if (st == SubscribeTypes.WSS)
            {
                //throw new Exception("This exchange does not support wss subscribing");
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
                    //这里根据循环给出假事件
                    //_subscribedtradingpairs[tradingpairs].TradeInfoEvent += OkCoinExchange_TradeInfoEvent;
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
                    //_subscribedtradingpairs[tradingpairs].TradeInfoEvent += OkCoinExchange_TradeInfoEvent;
                }
            }
            if (SubscribedEvent != null)
                SubscribedEvent(this, st, tp);
            return true;
        }

        private void QuoineExchange_TradeInfoEvent(TradingInfo ti, TradeEventType tt)
        {
            if (TickerEvent != null && tt == TradeEventType.TICKER)
            {
                TickerEvent(this, ti.t, (CommonLab.EventTypes)ti.type, ti.tp);
            }
            if (DepthEvent != null && tt == TradeEventType.ORDERS)
            {
                DepthEvent(this, ti.d, (CommonLab.EventTypes)ti.type, ti.tp);
            }
            if (TradeEvent != null && tt == TradeEventType.TRADE)
            {
                TradeEvent(this, ti.trade, (CommonLab.EventTypes)ti.type, ti.tp);

            }
        }

        public Ticker GetTicker(string tradingpair, out string rawresponse)
        {
            DateTime st = DateTime.Now;
           
            string url = ApiUrl + "/products/" + ExchangeProducts[tradingpair];
            restrequest = new RestRequest(Method.GET);
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            restrequest.AddHeader("X-Quoine-API-Version", "2");
            var rr = rc.Execute(restrequest);
            rawresponse = rr.Content;
            JObject ticker = JObject.Parse(rawresponse);
            Ticker t = new Ticker();
            try
            {
                
                t.High = Convert.ToDouble(ticker["high_market_ask"].ToString());
                t.Low = Convert.ToDouble(ticker["low_market_bid"].ToString());
                t.Last = Convert.ToDouble(ticker["last_traded_price"].ToString());
                t.Sell = Convert.ToDouble(ticker["market_ask"].ToString());
                t.Buy = Convert.ToDouble(ticker["market_bid"].ToString());
                t.Volume = Convert.ToDouble(ticker["volume_24h"].ToString());
                t.Open = Convert.ToDouble(ticker["last_price_24h"].ToString());// Convert.ToDouble(ticker["open"].ToString());
                //t.ExchangeTimeStamp = Convert.ToDouble(ticker[6]);
                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
                t.Delay = (DateTime.Now - st).TotalMilliseconds;
               
                UpdateTicker(tradingpair, t);
            }
            catch(Exception e)
            {

            }
            return t;
        }

        public Depth GetDepth(string tradingpair, out string rawresponse)
        {
            DateTime st = DateTime.Now;

            string url = ApiUrl + "/products/" + ExchangeProducts[tradingpair]+ "/price_levels";
            restrequest = new RestRequest(Method.GET);
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            restrequest.AddHeader("X-Quoine-API-Version", "2");
            var rr = rc.Execute(restrequest);
            rawresponse = rr.Content;
            JObject ticker = JObject.Parse(rawresponse);
            CommonLab.Depth d = new Depth();
            d.Asks = new List<MarketOrder>();
            d.Bids = new List<MarketOrder>();
            JObject obj = JObject.Parse(rawresponse);
            JArray jasks = JArray.Parse(obj["sell_price_levels"].ToString());
            for (int i = 0; i < jasks.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[0]);
                m.Amount = Convert.ToDouble(JArray.Parse(jasks[i].ToString())[1]);
                d.AddNewAsk(m);
            }

            JArray jbids = JArray.Parse(obj["buy_price_levels"].ToString());
            for (int i = 0; i < jbids.Count; i++)
            {
                MarketOrder m = new MarketOrder();
                m.Price = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[0]);
                m.Amount = Convert.ToDouble(JArray.Parse(jbids[i].ToString())[1]);
                d.AddNewBid(m);
            }
            d.ExchangeTimeStamp = 0;// Convert.ToDouble(obj["timestamp"].ToString());
            d.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now);
            d.Delay = (DateTime.Now - st).TotalMilliseconds;
            UpdateDepth(tradingpair, d);
            return d;
        }

        public Trade[] GetTrades(string tradepair, out string rawresponse, string since = "0")
        {
            List<Trade> trades = new List<CommonLab.Trade>();
            string url = GetPublicApiURL("symbol=" + tradepair + "&limit=50", "api/v1/trades");
            if (since != "0")
            {
                url += "&since=" + since;
            }
            rawresponse = CommonLab.Utility.GetHttpContent(url, "GET", "", _proxy);

            JArray obj = JArray.Parse(rawresponse);
            for (int i = 0; i < obj.Count; i++)
            {
                JObject trade = JObject.Parse(obj[i].ToString());
                Trade t = new CommonLab.Trade();
                t.TradeID = trade["id"].ToString();
                t.Amount = Convert.ToDouble(trade["qty"].ToString());
                t.Price = Convert.ToDouble(trade["price"].ToString());
                if (Convert.ToBoolean(trade["isBuyerMaker"].ToString()))
                    t.Type = TradeType.Buy;
                else
                    t.Type = TradeType.Sell;
              
                t.ExchangeTimeStamp = Convert.ToDouble(trade["time"].ToString()) ; //时间戳 交易所返回的
                t.LocalServerTimeStamp = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now); //本地时间戳
                t.BuyOrderID = "";//成交的交易号
                t.SellOrderID = "";//成交的交易号
                trades.Add(t);
            }
            if (trades.Count > 0)
                return trades.ToArray();
            return null;
        }
        /// <summary>
        /// 更新深度数据
        /// </summary>
        /// <param name="d"></param>
        protected void UpdateDepth(string tradingpair, Depth d)
        {
            if (SubscribedTradingPairs == null)
                return;
            if (SubscribedTradingPairs.ContainsKey(tradingpair))
            {
                ((RESTHelper)SubscribedTradingPairs[tradingpair]).UpdateDepth(d);
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
                ((RESTHelper)SubscribedTradingPairs[tradingpair]).TradeInfo.t = t;
            }
        }
        protected string Trade(OrderType type, string tradingpair, double price, double amount)
        {
            CheckSet();
            price = CommonLab.Utility.ToFixed(price, 6);
            string url = GetPublicApiURL("", "api/v3/order");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            RestRequest rr = new RestRequest(Method.POST);
            rr.AddHeader("X-MBX-APIKEY", _key);
            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("symbol", tradingpair);
            rr.AddParameter("symbol", tradingpair);
            switch (type)
            {
                case OrderType.ORDER_TYPE_BUY:
                    paras.Add("side", "BUY");
                    paras.Add("type", "LIMIT");
                    paras.Add("timeInForce", "GTC");
                    paras.Add("quantity", amount.ToString());
                    paras.Add("price", price.ToString());
                    rr.AddParameter("side", "BUY");
                    rr.AddParameter("type", "LIMIT");
                    rr.AddParameter("timeInForce", "GTC");
                    rr.AddParameter("quantity", amount.ToString());
                    rr.AddParameter("price", price.ToString());
                  
                    break;
                case OrderType.ORDER_TYPE_SELL:
                    paras.Add("side", "SELL");
                    paras.Add("type", "LIMIT");
                    paras.Add("timeInForce", "GTC");
                    paras.Add("quantity", amount.ToString());
                    paras.Add("price", price.ToString());
                    rr.AddParameter("side", "SELL");
                    rr.AddParameter("type", "LIMIT");
                    rr.AddParameter("timeInForce", "GTC");
                    rr.AddParameter("quantity", amount.ToString());
                    rr.AddParameter("price", price.ToString());
                    break;
                case OrderType.ORDER_TYPE_MARKETBUY:
                    paras.Add("side", "BUY");
                    paras.Add("type", "MARKET");
                    paras.Add("quantity", amount.ToString());
                    rr.AddParameter("side", "BUY");
                    rr.AddParameter("quantity", amount.ToString());
                    rr.AddParameter("type", "MARKET");
                    break;
                case OrderType.ORDER_TYPE_MARKETSELL:
                    paras.Add("side", "SELL");
                    paras.Add("type", "MARKET");
                    paras.Add("quantity", amount.ToString());
                    rr.AddParameter("side", "SELL");
                    rr.AddParameter("quantity", amount.ToString());
                    rr.AddParameter("type", "MARKET");
                    break;

            }
            string ts = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now).ToString();
            paras.Add("recvWindow", "10000");
            paras.Add("timestamp", ts);
            String sign = CommonLab.TokenGen.CreateSign_Binance(paras, _secret);
            rr.AddParameter("recvWindow", "10000");
            rr.AddParameter("timestamp", ts);
            rr.AddParameter("signature", sign);

            var response =  rc.Execute(rr);


            string rawresponse = response.Content;

            try
            {
                JObject obj = JObject.Parse(rawresponse);
       
                return obj["orderId"].ToString();
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
            if (tradepair.Length > 0)
                return ApiUrl + method + "?" + tradepair;
            else
                return ApiUrl + method;
        }

        public string GetLocalTradingPairString(TradePair t, SubscribeTypes st = CommonLab.SubscribeTypes.RESTAPI)
        {
            if (t.FromSymbol.ToLower() == "bcc")
                return "BCH" + t.ToSymbol.ToUpper();
            if (t.ToSymbol.ToLower() == "bcc")
                return t.FromSymbol.ToUpper() + "BCH";
            return t.FromSymbol.ToUpper() + t.ToSymbol.ToUpper();
        }

        public Account GetAccount(out string rawresponse)
        {
            CheckSet();
            if (_account == null)
                _account = new Account();
            string url = GetPublicApiURL("", "api/v3/account");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            RestRequest rr = new RestRequest(Method.GET);
            rr.AddHeader("X-MBX-APIKEY", _key);
            Dictionary<String, String> paras = new Dictionary<String, String>();
            string ts = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now).ToString();
            paras.Add("timestamp", ts);
            paras.Add("recvWindow", "10000");
            String sign = CommonLab.TokenGen.CreateSign_Binance(paras, _secret);
            rr.AddParameter("timestamp", ts);
            rr.AddParameter("recvWindow", "10000");
            rr.AddParameter("signature", sign);
            
            var response = rc.Execute(rr);


            rawresponse = response.Content;
            //解析
            try
            {
                JObject obj = JObject.Parse(rawresponse);
                if (obj.Property("balances") == null)
                {
                    throw (new Exception("error:" + rawresponse));
                }
               
                JArray balances = JArray.Parse(obj["balances"].ToString());
                foreach (JObject jp in balances)
                {
                    string Name = jp["asset"].ToString();
                    double free = Convert.ToDouble(jp["free"].ToString());
                    double locked = Convert.ToDouble(jp["locked"].ToString());
                    if (_account.Balances.ContainsKey(Name))
                    {
                        _account.Balances[Name].borrow = 0;
                        _account.Balances[Name].available = free;
                        _account.Balances[Name].reserved = locked;
                        _account.Balances[Name].balance = free + locked;
                    }
                    else
                    {
                        Balance b = new Balance();
                        b.borrow = 0;
                        b.available = free;
                        b.reserved = locked;
                        b.balance = b.available + b.reserved;

                        _account.Balances.Add(Name, b);
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return _account;
        }

        public Order GetOrderStatus(string OrderID, string tradingpair, out string rawresponse)
        {
            CheckSet();
            Order order = null;
            string url = GetPublicApiURL("", "api/v3/order");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            RestRequest rr = new RestRequest(Method.GET);
            rr.AddHeader("X-MBX-APIKEY", _key);
            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("symbol", tradingpair);
            rr.AddParameter("symbol", tradingpair);
            paras.Add("orderId", OrderID);
            rr.AddParameter("orderId", OrderID);
            string ts = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now).ToString();
            paras.Add("recvWindow", "10000");
            paras.Add("timestamp", ts);
            String sign = CommonLab.TokenGen.CreateSign_Binance(paras, _secret);
            rr.AddParameter("recvWindow", "10000");
            rr.AddParameter("timestamp", ts);
            rr.AddParameter("signature", sign);
            var response = rc.Execute(rr);
            rawresponse = response.Content;
            try
            {
                JObject orders = JObject.Parse(rawresponse);
               
                if (orders.Count > 0)
                {
                    order = new Order();
                    order.Id = orders["orderId"].ToString();
                    order.Amount = Convert.ToDouble(orders["origQty"].ToString());
                    order.DealAmount = Convert.ToDouble(orders["executedQty"].ToString());
                    order.Price = Convert.ToDouble(orders["price"].ToString());
                    order.Type = GetOrderTypeFromString(orders["type"].ToString(), orders["side"].ToString());
                    order.Status = GetOrderStatus(orders["status"].ToString());
                    order.TradingPair = orders["symbol"].ToString();
                    order.CreatDate = CommonLab.TimerHelper.ConvertStringToDateTime(Convert.ToDouble(orders["time"].ToString()) / 1000);
                }

            }
            catch (Exception e)
            {
                Exception err = new Exception("订单获取解析json失败" + e.Message);

                // throw err;
            }
            return order;

        }
        public List<Order> GetHisOrders(string tradingpair)
        {
            CheckSet();
            string rawresponse;
            Order order = null;
            List<Order> orders_array = new List<Order>();
            string url = GetPublicApiURL("", "order_history.do");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            int page = 1;
            RestRequest rr = new RestRequest(Method.POST);
            rr.AddParameter("api_key", _key);
            rr.AddParameter("symbol", tradingpair);
            rr.AddParameter("status", 1);
            rr.AddParameter("current_page", page);
            rr.AddParameter("page_length", 200);

            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("api_key", _key);
            paras.Add("symbol", tradingpair);
            paras.Add("status", "1");
            paras.Add("current_page", page.ToString());

            paras.Add("page_length", "200");
            String sign = CommonLab.TokenGen.CreateSign_Binance(paras, _secret);
            rr.AddParameter("sign", sign);
            var response = new RestClient(url).Execute(rr);

            int total;
            int totalpage = 0;
            rawresponse = response.Content;
            try
            {
                JObject obj = JObject.Parse(rawresponse);
                //if (obj.Property("error_code") != null)
                //    throw (new Exception(SpotErrcode2Msg.Prase(obj["error_code"].ToString())));
                if (!Convert.ToBoolean(obj["result"]))
                {
                    throw (new Exception("error:" + rawresponse));
                }
                JArray orders = JArray.Parse(obj["orders"].ToString());
                total = Convert.ToInt32(obj["total"].ToString());
                totalpage = total / 200 + 1;
                if (orders.Count > 0)
                {

                    for (int i = 0; i < orders.Count; i++)
                    {


                        order = new Order();
                        order.Id = orders[i]["order_id"].ToString();
                        order.Amount = Convert.ToDouble(orders[i]["amount"].ToString());
                        order.DealAmount = Convert.ToDouble(orders[i]["deal_amount"].ToString());
                        order.Price = Convert.ToDouble(orders[i]["price"].ToString());
                        order.Type = GetOrderTypeFromString(orders[i]["type"].ToString(),"");
                        order.Status = GetOrderStatus(orders[i]["status"].ToString());
                        order.TradingPair = orders[i]["symbol"].ToString();
                        order.CreatDate = CommonLab.TimerHelper.ConvertStringToDateTime(Convert.ToDouble(orders[i]["create_date"].ToString()) / 1000);
                        orders_array.Add(order);
                    }

                }
                for (page = 2; page <= totalpage; page++)
                {
                    rr.Parameters.Clear();
                    paras.Clear();
                    rr.AddParameter("api_key", _key);
                    rr.AddParameter("symbol", tradingpair);
                    rr.AddParameter("status", 1);
                    rr.AddParameter("current_page", page);
                    rr.AddParameter("page_length", 200);


                    paras.Add("api_key", _key);
                    paras.Add("symbol", tradingpair);
                    paras.Add("status", "1");
                    paras.Add("current_page", page.ToString());

                    paras.Add("page_length", "200");
                    sign = CommonLab.TokenGen.CreateSign_Binance(paras, _secret);
                    rr.AddParameter("sign", sign);
                    response = new RestClient(url).Execute(rr);


                    rawresponse = response.Content;
                    obj = JObject.Parse(rawresponse);
                    //if (obj.Property("error_code") != null)
                    //    throw (new Exception(SpotErrcode2Msg.Prase(obj["error_code"].ToString())));
                    if (!Convert.ToBoolean(obj["result"]))
                    {
                        throw (new Exception("error:" + rawresponse));
                    }
                    orders = JArray.Parse(obj["orders"].ToString());
                    if (orders.Count > 0)
                    {

                        for (int i = 0; i < orders.Count; i++)
                        {


                            order = new Order();
                            order.Id = orders[i]["order_id"].ToString();
                            order.Amount = Convert.ToDouble(orders[i]["amount"].ToString());
                            order.DealAmount = Convert.ToDouble(orders[i]["deal_amount"].ToString());
                            order.Price = Convert.ToDouble(orders[i]["price"].ToString());
                            order.Type = GetOrderTypeFromString(orders[i]["type"].ToString(),"");
                            order.Status = GetOrderStatus(orders[i]["status"].ToString());
                            order.TradingPair = orders[i]["symbol"].ToString();
                            order.CreatDate = CommonLab.TimerHelper.ConvertStringToDateTime(Convert.ToDouble(orders[i]["create_date"].ToString()) / 1000);
                            orders_array.Add(order);
                        }

                    }
                }
                return orders_array;

            }
            catch (Exception e)
            {
                Exception err = new Exception("订单获取解析json失败" + e.Message);
                throw err;
            }

        }
        public List<Order> GetOrdersStatus(string tradingpair, out string rawresponse)
        {
            CheckSet();
            Order o = null;
            string url = GetPublicApiURL("", "api/v3/allOrders");
            RestClient rc = new RestClient(url);
            RestRequest rr = new RestRequest(Method.GET);
            rr.AddHeader("X-MBX-APIKEY", _key);
            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("symbol", tradingpair);
            rr.AddParameter("symbol", tradingpair);
           
            string ts = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now).ToString();
            paras.Add("recvWindow", "10000");
            paras.Add("timestamp", ts);
            String sign = CommonLab.TokenGen.CreateSign_Binance(paras, _secret);
            rr.AddParameter("recvWindow", "10000");
            rr.AddParameter("timestamp", ts);
            rr.AddParameter("signature", sign);
            var response = rc.Execute(rr);
            rawresponse = response.Content;
            try
            {
                
                JArray orders = JArray.Parse(rawresponse);
                if (orders.Count > 0)
                {
                    List<Order> orders_array = new List<Order>();
                    for (int i = 0; i < orders.Count; i++)
                    {

                        JObject order = JObject.Parse(orders[i].ToString());


                        o = new Order();
                        o.Id = order["orderId"].ToString();
                        o.Amount = Convert.ToDouble(order["origQty"].ToString());
                        o.DealAmount = Convert.ToDouble(order["executedQty"].ToString());
                        o.Price = Convert.ToDouble(order["price"].ToString());
                        o.Type = GetOrderTypeFromString(order["type"].ToString(), order["side"].ToString());
                        o.Status = GetOrderStatus(order["status"].ToString());
                        o.TradingPair = order["symbol"].ToString();
                        o.CreatDate = CommonLab.TimerHelper.ConvertStringToDateTime(Convert.ToDouble(order["time"].ToString()) / 1000);
                       orders_array.Add(o);
                    }
                    return orders_array;
                }

            }
            catch (Exception e)
            {
                Exception err = new Exception("订单获取解析json失败" + e.Message);
                throw err;
            }
            return null;

        }
        public bool CancelOrder(string OrderID, string tradingpair, out string rawresponse)
        {
            CheckSet();

            string url = GetPublicApiURL("", "api/v3/order");
            RestClient rc = new RestClient(url);
            if (_proxy != null)
            {
                rc.Proxy = new WebProxy(_proxy.IP, Convert.ToInt32(_proxy.Port));
            }
            RestRequest rr = new RestRequest(Method.DELETE);
            rr.AddHeader("X-MBX-APIKEY", _key);
            Dictionary<String, String> paras = new Dictionary<String, String>();
            paras.Add("symbol", tradingpair);
            rr.AddParameter("symbol", tradingpair);
            paras.Add("orderId", OrderID);
            rr.AddParameter("orderId", OrderID);
            string ts = CommonLab.TimerHelper.GetTimeStampMilliSeconds(DateTime.Now).ToString();
            paras.Add("recvWindow", "10000");
            paras.Add("timestamp", ts);
            String sign = CommonLab.TokenGen.CreateSign_Binance(paras, _secret);
            rr.AddParameter("recvWindow", "10000");
            rr.AddParameter("timestamp", ts);
            rr.AddParameter("signature", sign);
            var response = rc.Execute(rr);
            rawresponse = response.Content;
            try
            {
                JObject obj = JObject.Parse(rawresponse);
                if (obj["orderId"].ToString()==OrderID)
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

        public bool CancelAllOrders(string tradingpair = "")
        {
            bool flag = false;
            try
            {
                if (SubscribedTradingPairs != null)
                {
                    foreach (KeyValuePair<string, SubscribeInterface> item in SubscribedTradingPairs)
                    {
                        string raw = "";
                        List<CommonLab.Order> orders = GetOrdersStatus(item.Key, out raw);
                        if (orders != null)
                        {
                            for (int i = 0; i < orders.Count; i++)
                            { CancelOrder(orders[i].Id, item.Key, out raw); }
                            flag = true;
                        }
                    }
                }
                return flag;
            }
            catch
            {
                return flag;
            }

        }

        public string Buy(string Symbol, double Price, double Amount)
        {
            if (Price > 0)
            {
                return Trade(OrderType.ORDER_TYPE_BUY, Symbol, Price, Amount);
            }
            else if (Price == 0)
            {
                return Trade(OrderType.ORDER_TYPE_MARKETBUY, Symbol, Price, Amount);
            }
            return "";
        }

        public string Sell(string Symbol, double Price, double Amount)
        {
            if (Price > 0)
            {
                return Trade(OrderType.ORDER_TYPE_SELL, Symbol, Price, Amount);
            }
            else if (Price == 0)
            {
                return Trade(OrderType.ORDER_TYPE_MARKETSELL, Symbol, Price, Amount);
            }
            return "";
        }

        public void CheckSet()
        {
            if (string.IsNullOrEmpty(_key) || string.IsNullOrEmpty(_secret))
            {
                throw new Exception("交易所设置有问题！");
            }
        }
        static public OrderType GetOrderTypeFromString(string str,string side)
        {
            if (str.ToUpper() == "LIMIT"&&side=="BUY")
                return OrderType.ORDER_TYPE_BUY;
            if (str.ToUpper() == "LIMIT" && side == "SELL")
                return OrderType.ORDER_TYPE_SELL;
            if (str.ToUpper() == "MARKET" && side == "BUY")
                return OrderType.ORDER_TYPE_MARKETBUY;
            if (str.ToUpper() == "MARKET" && side == "SELL")
                return OrderType.ORDER_TYPE_MARKETSELL;
            return OrderType.ORDER_TYPE_UNKOWN;
        }
        static public OrderStatus GetOrderStatus(string str)
        {
            //status:-1:已撤销  0:未成交  1:部分成交  2:完全成交 4:撤单处理中
            if (str.ToUpper() == "NEW")
                return OrderStatus.ORDER_STATE_PENDING;
            if (str.ToUpper() == "PARTIALLY_FILLED")
                return OrderStatus.ORDER_STATE_PARTITAL;
            if (str.ToUpper() == "FILLED")
                return OrderStatus.ORDER_STATE_SUCCESS;
            if (str.ToUpper() == "CANCELED")
                return OrderStatus.ORDER_STATE_CANCELED;
            if (str.ToUpper() == "PENDING_CANCEL ")
                return OrderStatus.ORDER_STATE_CANCELING;
            if (str.ToUpper() == "REJECTED ")
                return OrderStatus.ORDER_STATE_UNKOWN;
            if (str.ToUpper() == "EXPIRED ")
                return OrderStatus.ORDER_STATE_UNKOWN;
            return OrderStatus.ORDER_STATE_UNKOWN;
        }
        private Exception JsonError(JObject obj)
        {
            if (obj.Property("error_code") == null || obj.Property("error_code").ToString() == "")
            {
                return new Exception("\"error_code\" not found in raw json string!");
            }
            //else
            //    return new Exception(SpotErrcode2Msg.Prase(obj["error_code"].ToString()));
            return new Exception("\"error_code\" not found in raw json string!");
        }

        public void DisSubcribe(TradePair tp, SubscribeTypes st)
        {
            throw new NotImplementedException();
        }


    }
}

