using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using KFCC.ExchangeInterface;
namespace KFCC.EBitstamp
{
    public class BitstampExchange : IExchanges
    {
        private string _secret;
        private string _key;
        private string _uid;
        private string _username;
        static private BitstampExchange _instance=null;
        public string Name { get { return "Bitstamp"; } }
        public string ExchangeUrl {  get { return "www.bitstamp.net"; } }
        public string Remark { get { return "bitstamp exchange remark"; } }
        public string Secret { get { return _secret; } set { _secret = value; } }
        public string Key { get { return _key; } set { _key = value; } }
        public string UID { get { return _uid; } set { _uid = value; } }
        public string UserName { get { return _username; } set { _username = value; } }

        public Dictionary<string, int> SubscribeTradingPairs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
        public BitstampExchange Create(string key,string secret,string uid,string username)
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
    }
}
