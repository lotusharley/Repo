using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.DataModel
{
    public class XCommandFactory
    {
        private static XCommandFactory _Instance;
        private XCommandFactory()
        {
        }

        public static XCommandFactory GetInstance()
        {
            if (_Instance == null)
                _Instance = new XCommandFactory();

            return _Instance;
        }

        public BaseCommand CreateCommand(String s)
        {
            BaseCommand cmd = null;

            if (s.Substring(0, 4) == "XGPS")
                cmd = new XCommand_GPS(s);
            else if (s.Substring(0, 4) == "XATT")
                cmd = new XCommand_ATT(s);
            else
                cmd = new XCommand_TRAFFIC(s);

            return cmd;
        }
    }
}
