using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLab
{
    public class Proxy
    {
        public Proxy(string ip, int port)
        {
            this.IP = ip;
            this.Port = port;
        }
        public string IP;
        public int Port;
    }
}
