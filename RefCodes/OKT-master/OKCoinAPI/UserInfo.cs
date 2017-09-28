using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKCoinAPI
{
    public class UserInfo
    {
        public decimal asset_net;      //净资产
        public decimal asset_total;    //总资产
        public decimal free_cny;       //可用CNY
        public decimal free_btc;
        public decimal free_ltc;
        public decimal freezed_cny;    //冻结CNY
        public decimal freezed_btc;
        public decimal freezed_ltc;

    }
}
