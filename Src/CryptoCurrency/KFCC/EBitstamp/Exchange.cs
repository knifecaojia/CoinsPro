using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLab;
using KFCC.ExchangeInterface;
namespace KFCC.EBitstamp
{
    public class Exchange : IExchanges
    {
        private string _secret;
        private string _key;
        private string _uid;
        private string _username;
        public string Name { get { return "Bitstamp"; } }
        public string ExchangeUrl {  get { return "www.bitstamp.net"; } }
        public string Remark { get { return "bitstamp exchange remark"; } }
        public string Secret { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Key { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool SportWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool SportThirdPartWSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event ExchangeEventWarper.TickerEventHander TickerEvent;
        public event ExchangeEventWarper.DepthEventHander DepthEvent;
    }
}
