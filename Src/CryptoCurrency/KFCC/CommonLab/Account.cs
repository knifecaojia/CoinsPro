using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class Account
    {
        Dictionary<string, Balance> Balances;
        Dictionary<string, Fee> Fees;
        string ID;

    }
    public class Balance
    {
        public double balance;
        public double reserved;
        public double available;
    }
    public class Fee
    {
        public double TakerFee;
        public double MakerFee;
    }
}
