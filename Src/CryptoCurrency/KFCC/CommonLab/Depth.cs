using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class Depth
    {

        //Info	:交易所返回的原始结构(只支持商品期货CTP)
        /// <summary>
        /// 卖单数组, MarketOrder数组, 按价格从低向高排序
        /// </summary>
        public List<MarketOrder> Asks;//	:
        /// <summary>
        /// 买单数组, MarketOrder数组, 按价格从高向低排序
        /// </summary>
        public List<MarketOrder> Bids;//	:

        public double ExchangeTimeStamp;//时间戳 交易所返回的
        public double LocalServerTimeStamp;//本地时间戳
        public Depth()
        {
            Asks = new List<MarketOrder>();
            Bids = new List<MarketOrder>();
        }
        public void AddNewBid(MarketOrder m)
        {
            Bids.Add(m);
            Bids.Sort(delegate (MarketOrder x, MarketOrder y)
            {
                return y.Price.CompareTo(x.Price);
            });
            CutLast();
        }
        public void AddNewAsk(MarketOrder m)
        {
            Asks.Add(m);
            Asks.Sort(delegate (MarketOrder x, MarketOrder y)
            {
                return x.Price.CompareTo(y.Price);
            });
            CutLast();
        }
        private void CutLast()
        {
            if (Asks.Count > 100)
            {
                Asks.RemoveRange(100, Asks.Count - 100);
            }
            if (Bids.Count > 100)
            {
                Bids.RemoveRange(100, Bids.Count - 100);
            }
        }
        public string ToString(int count=3)
        {
            string bids = "bids:";
            if (count > (Math.Min(Asks.Count, Bids.Count)))
            {
                count = Math.Min(Asks.Count, Bids.Count);
            }
            for (int i = 0; i < count; i++)
            {
                bids += Bids[i].Amount + "@" + Bids[i].Price;
            }
            string asks = "asks:";
            for (int i = 0; i < count; i++)
            {
                asks += Asks[i].Amount + "@" + Asks[i].Price;
            }
            return bids + "----" + asks;
        }
    }
}
