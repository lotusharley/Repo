using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.Control
{
    public interface IGPSEFISReceived
    {
        event EventHandler<DataModel.XCommand_GPS> GPSDataReceived;
        event EventHandler<DataModel.XCommand_ATT> onEFISDataReceived;
        event EventHandler<DataModel.XCommand_TRAFFIC> onTRAFFICDataReceived;
    }
}
