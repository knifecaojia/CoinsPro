using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerDOG
{
    class Miner
    {
        public int ID { get; set; }
        public string StatusType { get; set; }
        public string Mtype { get; set; }
        public string IPaddress { get; set; }
   

        public string Elapsed { get; set; }
        public string Gper5s { get; set; }
        public string Gavg { get; set; }
        public string ctep1 { get; set; }
        public string ctep2 { get; set; }
        public string ctep3 { get; set; }

        public string fspd1 { get; set; }
        public string fspd2 { get; set; }
        public string freq { get; set; }
        public string HW { get; set; }
        public string HWP { get; set; }

    }
    public class status
    {
        public string Elapsed { get; set; }
        public string Gper5s { get; set; }
        public string Gavg { get; set; }
        public string ctep1 { get; set; }
        public string ctep2 { get; set; }
        public string ctep3 { get; set; }

        public string fspd1 { get; set; }
        public string fspd2 { get; set; }
    }
}
