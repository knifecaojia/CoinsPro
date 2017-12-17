using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class TradePair
    {
        public string FromSymbol { get; set; }

        public string ToSymbol { get; set; }

        public TradePair(string f, string t)
        {
            FromSymbol = f;
            ToSymbol = t;
        }
        public bool Compare(TradePair x)
        {
            if (this.FromSymbol == x.FromSymbol && this.ToSymbol == x.ToSymbol)
                return true;
            return false;
        }
        public override string ToString()
        {
            return FromSymbol + "-" + ToSymbol;
        }
    }
}
