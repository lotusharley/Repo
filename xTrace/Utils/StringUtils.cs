using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.Utils
{
    public class StringUtils
    {
        public static string CutString(string start,string end,string source)
        {
            string sRtn = string.Empty;
            int iStart = -1;
            int iEnd = -1;
            iStart = source.IndexOf(start);
            if (iStart == -1)
                return string.Empty;
            iEnd = source.IndexOf(end, iStart +1);
            if (iEnd == -1)
                return string.Empty;
            sRtn = source.Substring(iStart, iEnd - iStart);
            return sRtn;
        }

        public static List<String> CutStrings(string start,string end,string source)
        {
            List<string> rtn = new List<string>();
            int iStart = -1;
            int iEnd = -1;
            int iStop = -1;
            iStop = source.LastIndexOf(start);
            if (iStop == -1)
                return rtn;
            while(iStart <= iStop)
            {
                iStart = source.IndexOf(start, iEnd + 1);
                iEnd = source.IndexOf(end, iStart + 1);
                string s = source.Substring(iStart, iEnd - iStart);
                rtn.Add(s);
                iStart = iStart + 1;
            }
            return rtn;
        }

        public static bool HasNumber(string source)
        {
            bool bRtn = false;

            foreach(char c in source.ToCharArray())
            {
                if (c > 48 && c < 58)
                    return true;
            }

            return bRtn;
        }
    }
}
