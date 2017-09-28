using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace OKCoinAPI
{
    public class OKCoin
    {
        string api_key;
        string api_secret;

        int timeout;            
        int timezone;

        string api_url = "https://www.okcoin.cn/";
        public string API_URL
        {
            get { return api_url; }
            set { api_url = value; }
        }

        string api_ver = "api/v1/";
        public string API_VER
        {
            get { return api_ver; }
            set { api_ver = value; }
        }

        TickerType tickerType = TickerType.LTC;
        public TickerType TickerTypeName
        {
            get { return tickerType; }
            set { tickerType = value; }
        }

        public OKCoin(string api_key, string api_secret, int timeout = 5000, int timezone = 8)
        {
            this.api_key = api_key;
            this.api_secret = api_secret;
            this.timeout = timeout;
            this.timezone = timezone;
        }

        string GET_API_URL(string api)
        {
            return string.Format("{0}{1}{2}?", this.api_url, this.api_ver, api);
        }

        string GetActionUrl(string action)
        {
            string url = GET_API_URL(action);

            switch (tickerType)
            {
                case TickerType.BTC: url = url + "symbol=btc_cny"; break;
                case TickerType.LTC: url = url + "symbol=ltc_cny"; break;
            }

            return url;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public UserInfo GetUserInfo()
        {
            UserInfo result = null;

            try
            {
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                paramList.Add("api_key", this.api_key);

                JObject json = Post(GET_API_URL("userinfo.do"), paramList);


                bool queryResult = json["result"].Value<bool>();

                if (queryResult)
                {
                    JToken token = json["info"]["funds"];

                    result = new UserInfo()
                    {
                        asset_net = token["asset"]["net"].Value<decimal>(),
                        asset_total = token["asset"]["total"].Value<decimal>(),
                        free_cny = token["free"]["cny"].Value<decimal>(),
                        free_btc = token["free"]["btc"].Value<decimal>(),
                        free_ltc = token["free"]["ltc"].Value<decimal>(),
                        freezed_cny = token["freezed"]["cny"].Value<decimal>(),
                        freezed_btc = token["freezed"]["btc"].Value<decimal>(),
                        freezed_ltc = token["freezed"]["ltc"].Value<decimal>()
                    };
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 获取OKCoin行情
        /// </summary>
        /// <returns></returns>
        public Ticker GetTicker()
        {
            Ticker result = null;

            try
            {
                string url = GetActionUrl("ticker.do");

                JObject json = Get(url);

                result = new Ticker()
                {
                    Time = DateTime.Now,
                    High = json["ticker"]["high"].Value<decimal>(),
                    Low = json["ticker"]["low"].Value<decimal>(),
                    Buy = json["ticker"]["buy"].Value<decimal>(),
                    Sell = json["ticker"]["sell"].Value<decimal>(),
                    Last = json["ticker"]["last"].Value<decimal>(),
                    Volume = json["ticker"]["vol"].Value<decimal>()
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 获取OKCoin市场深度
        /// </summary>
        /// <returns></returns>
        public Depth GetDepth()
        {
            Depth depth = null;

            try
            {
                string url = GetActionUrl("depth.do");

                JObject json = Get(url);


                IList<DepthItem> asks = new List<DepthItem>();

                for (int i = 0; i < json["asks"].Count(); i++)
                {
                    DepthItem item = new DepthItem()
                    {
                        Price = json["asks"][i][0].Value<decimal>(),
                        Volume = json["asks"][i][1].Value<decimal>()
                    };

                    asks.Add(item);
                }


                IList<DepthItem> bids = new List<DepthItem>();

                for (int i = 0; i < json["bids"].Count(); i++)
                {
                    DepthItem item = new DepthItem()
                    {
                       Price = json["bids"][i][0].Value<decimal>(),
                       Volume = json["bids"][i][1].Value<decimal>()
                    };
                    
                    bids.Add(item);                   
                }

                depth = new Depth();
                depth.Asks = asks.ToArray();
                depth.Bids = bids.ToArray();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            return depth;
        }

        /// <summary>
        /// 下单交易
        /// </summary>
        /// <param name="tradeParam">参数</param>
        /// <returns>如果交易成功：返回订单ID，否则返回空字符串</returns>
        public string Trade(Trade tradeParam)
        {
            string orderID = string.Empty;

            try
            {
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                paramList.Add("api_key", this.api_key);
                paramList.Add("symbol", (tickerType == TickerType.BTC ? "btc_cny" : "ltc_cny"));
                paramList.Add("type", tradeParam.type);
                paramList.Add("price", tradeParam.price.ToString());
                paramList.Add("amount", tradeParam.amount.ToString());
             
                JObject json = Post(GET_API_URL("trade.do"), paramList);

                bool queryResult = json["result"].Value<bool>();

                if (queryResult)
                {
                    orderID =  json["order_id"].Value<string>();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return orderID;

        }

        /// <summary>
        /// 撤销订单
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <returns>如果取消交易成功：返回订单ID，否则返回空字符串</returns>
        public string CancelOrder(string orderID)
        {
            string cancelOrderID = string.Empty;

            try
            {
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                paramList.Add("api_key", this.api_key);
                paramList.Add("symbol", (tickerType == TickerType.BTC ? "btc_cny" : "ltc_cny"));
                paramList.Add("order_id", orderID);

                JObject json = Post(GET_API_URL("cancel_order.do"), paramList);

                bool queryResult = json["result"].Value<bool>();

                if (queryResult)
                {
                    cancelOrderID = json["order_id"].Value<string>();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return cancelOrderID;

        }

        /// <summary>
        /// 查询订单信息
        /// </summary>
        /// <param name="orderID">订单ID -1:未完成订单，否则查询相应订单号的订单</param>
        /// <returns></returns>
        public List<Order> OrderInfo(string orderID)
        {
            List<Order> orderInfoList = null;

            try
            {
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                paramList.Add("api_key", this.api_key);
                paramList.Add("symbol", (tickerType == TickerType.BTC ? "btc_cny" : "ltc_cny"));
                paramList.Add("order_id", orderID);

                JObject json = Post(GET_API_URL("order_info.do"), paramList);

                bool queryResult = json["result"].Value<bool>();

                if (queryResult)
                {
                    orderInfoList = new List<Order>();

                    JToken token = json["orders"];
                    for (int i = 0; i < token.Count(); i++)
                    {
                        Order item = new Order();
                        UInt64 timeValue = token[i]["create_date"].Value<UInt64>();     
                        item.create_date = UnixTime.FromInt32((int)(timeValue / 1000));

                        item.amount = token[i]["amount"].Value<decimal>();
                        item.avg_price = token[i]["avg_price"].Value<decimal>();
                        item.deal_amount = token[i]["deal_amount"].Value<decimal>();
                        item.order_id = token[i]["order_id"].Value<string>();
                        item.orders_id = token[i]["orders_id"].Value<string>();
                        item.price = token[i]["price"].Value<decimal>();
                        item.status = token[i]["status"].Value<string>();
                        item.symbol = token[i]["symbol"].Value<string>();
                        item.type = token[i]["type"].Value<string>();

                        orderInfoList.Add(item);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return orderInfoList;

        }

        #region Get & Post


        static JObject Get(string url, int timeout = 5000)
        {
            WebClientPlus webClient = new WebClientPlus(timeout);

            string result = webClient.DownloadString(url);

            webClient.Dispose();

            JObject json = JObject.Parse(result);

            return json;
        }


        JObject Post(string url, Dictionary<string, string> paramList)
        {
            string data = "";
           
            Dictionary<string, string> sortParamList = (from entry in paramList orderby entry.Key ascending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (KeyValuePair<string, string> param in sortParamList)
            {
                data += data == "" ? "" : "&";
                data += param.Key + "=" + param.Value;
            }

            string sign = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(data + "&secret_key=" + this.api_secret, "MD5").ToUpper();
            data += "&sign=" + sign;

            WebClientPlus webClient = new WebClientPlus(this.timeout);
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string result = webClient.UploadString(url, data);
            webClient.Dispose();

            if (result[0] == '[') { result = "{\"list\":" + result + "}"; }

            JObject json = JObject.Parse(result);
            if (json.GetValue("code") != null)
            {
                //throw new HuobiException(_json.GetValue("code").Value<string>(), _json.GetValue("msg").Value<string>());
            }

            return json;
        }

        #endregion
    }
}