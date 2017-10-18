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

    }
}
