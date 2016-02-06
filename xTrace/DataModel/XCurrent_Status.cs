using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.DataModel
{
    public class XCurrent_Status
    {
        public XCommand_GPS GPSStatus { get; set; }
        public XCommand_ATT ATTStatus { get; set; }
    }
}
