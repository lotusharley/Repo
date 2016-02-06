using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.DataModel
{
    public class NetworkInterface
    {
        public string HostName { get; set; }
        public string IPAddr { get; set; }

        public NetworkInterface(string hostName,string ipAddr)
        {
            IPAddr = ipAddr;
            HostName = hostName;
        }
    }
}
