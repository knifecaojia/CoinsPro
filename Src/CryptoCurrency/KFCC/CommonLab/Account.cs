using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class Account
    {
        public Dictionary<string, Balance> Balances;
        public Dictionary<string, Fee> Fees;
        public string ID;
        public Account()
        {
            Balances = new Dictionary<string, Balance>();
            Fees = new Dictionary<string, Fee>();
        }
    }
    public class Balance
    {
        public double balance;
        public double reserved;//等同交易所冻结金额（待求证）
        public double available;//可用
        public double borrow;//借款 okex
    }
    public class Fee
    {
        public double TakerFee;
        public double MakerFee;
    }
}
