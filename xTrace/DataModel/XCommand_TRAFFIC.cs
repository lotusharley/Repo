using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.DataModel
{
    public class XCommand_TRAFFIC:BaseCommand
    {

        public XCommand_TRAFFIC(String s)
        {
            ParserFromString(s);
        }
        public int ICAOAddr { get; set; }
        public float TRAFFICLatitude { get; set; }
        public float TRAFFICLongitude { get; set; }
        public float GEOMETRICAltitude { get; set; }
        public float TRAFFICVertialSpeed { get; set; }
        public bool Airborne { get; set; }
        public float Heading { get; set; }
        public float VelocityKnots { get; set; }
        public string Callsign { get; set; }
        public override void ParserFromString(string txtSource)
        {
            this.CommandType = XPCommandType.TYPE_XTRAFFIC;
            List<String> strList = txtSource.Split(",".ToCharArray()).ToList();
            XPName = strList[0].Replace("XTRAFFIC", "");
            ICAOAddr = GetIntFromString(strList[1]);
            TRAFFICLatitude = GetFloatFromString(strList[2]);
            TRAFFICLongitude = GetFloatFromString(strList[3]);
            GEOMETRICAltitude = GetFloatFromString(strList[4]);
            TRAFFICVertialSpeed = GetFloatFromString(strList[5]);
            Airborne = GetBooleanFromString(strList[6]);
            Heading = GetFloatFromString(strList[7]);
            VelocityKnots = GetFloatFromString(strList[8]);
            Callsign = strList[9];
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(CommandType.ToString());
            sb.AppendLine("Name - " + XPName);
            sb.AppendLine("ICAO address:" + ICAOAddr.ToString());
            sb.AppendLine("Traffic latitude:" + TRAFFICLatitude.ToString());
            sb.AppendLine("Traffic longitude:" + TRAFFICLongitude.ToString());
            sb.AppendLine("Traffic geometric altitude:" + GEOMETRICAltitude.ToString());
            sb.AppendLine("Traffic vertical speed:" + TRAFFICVertialSpeed.ToString());
            if (Airborne)
                sb.AppendLine("Airborne:YES");
            else
                sb.AppendLine("Airborne:NO");
            sb.AppendLine("Heading:" + Heading.ToString());
            sb.AppendLine("Velocity knots" + VelocityKnots.ToString());
            sb.AppendLine("Callsign:" + Callsign);



            string sRtn = string.Empty;
            sRtn = sb.ToString();
            return sRtn;
        }
    }
}
