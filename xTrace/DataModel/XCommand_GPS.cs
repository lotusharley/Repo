using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.DataModel
{
    public class XCommand_GPS:BaseCommand
    {

        public XCommand_GPS(String s)
        {
            ParserFromString(s);
        }

        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public float Altitude { get; set; }
        public float HeadingTrueNorth { get; set; }
        public float GroundSpeed { get; set; }

        public Windows.Devices.Geolocation.Geopoint GeoPointInfo {
            get
            {
                Windows.Devices.Geolocation.Geopoint geopoint = new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition()
                {
                    Longitude = this.Longitude,
                    Latitude = this.Latitude,
                    Altitude = this.Altitude
                });
                return geopoint;
            }
        }

        public override void ParserFromString(string txtSource)
        {
            CommandType = XPCommandType.TYPE_XGPS;
            List<String> strList = txtSource.Split(",".ToCharArray()).ToList();
            XPName = strList[0].Replace("XGPS", "");
            Longitude = GetFloatFromString(strList[1]);
            Latitude = GetFloatFromString(strList[2]);
            Altitude = GetFloatFromString(strList[3]);
            HeadingTrueNorth = GetFloatFromString(strList[4]);
            GroundSpeed = GetFloatFromString(strList[5]);

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(CommandType.ToString());
            sb.AppendLine("Name - " + XPName);
            sb.AppendLine("Longitude:" + Longitude.ToString());
            sb.AppendLine("Latitude:" + Latitude.ToString());
            sb.AppendLine("Altitude:" + Altitude.ToString());
            sb.AppendLine("HeadingTrueNorth:" + HeadingTrueNorth.ToString());
            sb.AppendLine("GroundSpeed:" + GroundSpeed.ToString());

            string sRtn = string.Empty;
            sRtn = sb.ToString();
            return sRtn;
        }

    }
}
