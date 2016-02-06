using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.DataModel
{
    public class XCommand_ATT:BaseCommand
    {

        public XCommand_ATT(String s)
        {
            ParserFromString(s);
        }

        public float Heading { get; set; }
        public float Pitch { get;set;}
        public float Roll { get; set; }



        public override void ParserFromString(string txtSource)
        {
            CommandType = XPCommandType.TYPE_XATT;
            List<String> strList = txtSource.Split(",".ToCharArray()).ToList();
            XPName = strList[0].Replace("XATT", "");
            Heading = GetFloatFromString(strList[1]);
            Pitch = GetFloatFromString(strList[2]);
            Roll = GetFloatFromString(strList[3]);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(CommandType.ToString());
            sb.AppendLine("Name - " + XPName);
            sb.AppendLine("Heading:" + Heading.ToString());
            sb.AppendLine("Pitch:" + Pitch.ToString());
            sb.AppendLine("Roll:" + Roll.ToString());

            string sRtn = string.Empty;
            sRtn = sb.ToString();
            return sRtn;
        }
    }
}
