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
        public override string ToString()
        {
            string str = "";
            foreach (var item in Balances)
            {

                str += item.Key + "---B:" + item.Value.balance.ToString() + " A:" + item.Value.available.ToString() + " F:" + item.Value.reserved.ToString();
                str += System.Environment.NewLine;
            }
            return str;
        }
        public Account Clone()
        {
            Account a = new Account();
            a.ID = ID;
            foreach (KeyValuePair<string, Balance> item in Balances)
            {
                Balance b = new Balance();
                b.available = item.Value.available;
                b.balance = item.Value.balance;
                b.borrow = item.Value.borrow;
                b.reserved = item.Value.reserved;
                a.Balances.Add(item.Key, b);
            }
            foreach (KeyValuePair<string, Fee> item in Fees)
            {
                Fee f = new Fee();
                f.MakerFee = item.Value.MakerFee;
                f.TakerFee = item.Value.TakerFee;
                a.Fees.Add(item.Key, f);
            }
            return a;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="flag">true 返回非零账户</param>
        /// <returns></returns>
        public string ToString(bool flag)
        {
            if (!flag)
                return   ToString();
            string str = "";
            foreach (var item in Balances)
            {
                if (item.Value.balance != 0)
                {
                    str += item.Key + "---B:" + item.Value.balance.ToString() + " A:" + item.Value.available.ToString() + " F:" + item.Value.reserved.ToString();
                    str += System.Environment.NewLine;
                }
               
            }
            return str;
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
    public class SubAccount
    {
        public string ID;
        public string Type;
        public string State;
        
    }
}
