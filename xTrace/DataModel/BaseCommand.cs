using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.DataModel
{
    public abstract class BaseCommand
    {
        public DataModel.XPCommandType CommandType { get; set; }
        public String XPName { get; set; }

        public abstract void ParserFromString(String txtSource);

        public int GetIntFromString(String s)
        {
            int iRtn = -1;
            if (!int.TryParse(s, out iRtn))
                iRtn = 1;

            return iRtn;
        }

        public float GetFloatFromString(String s)
        {
            float fRtn = -1;
            if (!float.TryParse(s, out fRtn))
                fRtn = -1;
            return fRtn;
        }

        public bool GetBooleanFromString(String s)
        {
            bool bRtn = false;

            if (!bool.TryParse(s, out bRtn))
                bRtn = false;

            return bRtn;
        }

    }

}
