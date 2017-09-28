using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKCoinAPI
{
    public class Depth
    {
        public DepthItem[] Asks;
        public DepthItem[] Bids;
    }

    public class DepthItem
    {
        public decimal Price;
        public decimal Volume;
    }
}
