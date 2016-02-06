using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.Utils
{
    public class RouteBuilder
    {
        private string CycleVersion = string.Empty;
        private string sAirports = string.Empty;
        private string sATS = string.Empty;
        private string sNavaids = string.Empty;
        private string sWaypoints = string.Empty;
        private bool isInit = false;

        public RouteBuilder(string sCycle)
        {
            CycleVersion = sCycle;
        }

        private async Task InitNavData()
        {
            sAirports = await FileUtils.ReadTextNavFile(this.CycleVersion, "Airports.txt");
            sATS = await FileUtils.ReadTextNavFile(this.CycleVersion, "ATS.txt");
            sNavaids = await FileUtils.ReadTextNavFile(this.CycleVersion, "Navaids.txt");
            sWaypoints = await FileUtils.ReadTextNavFile(this.CycleVersion, "Waypoints.txt");
            isInit = true;
        }

        public static async Task<List<String>> GetCycleInfo()
        {
            List<String> rtn = new List<string>();
            rtn = await FileUtils.ReturnCycleInfos();
            return rtn;
        }

        public async Task<DataModel.XAirportInfo> GetAirportInfo(string ICAO)
        {
            if (!isInit)
                await InitNavData();

            DataModel.XAirportInfo oRtn = null;
            string airport = StringUtils.CutString("A|" + ICAO.ToUpper(), "\r\n\r", sAirports);
            if (airport.Length == 0)
                return null;
            foreach (string line in airport.Split("\r\n".ToCharArray()))
            {
                if (line.StartsWith("A|"))
                {
                    oRtn = new DataModel.XAirportInfo();
                    string[] s = line.Split("|".ToCharArray());
                    oRtn.Airportcode = s[1];
                    oRtn.Airportname = s[2];
                    oRtn.Longtitude = s[4];
                    oRtn.Latitude = s[3];
                    oRtn.Altitude = s[5];
                }

                if (line.StartsWith("R|"))
                {

                }

            }

            return oRtn;
        }

        public async Task<DataModel.XFlightPlan> CreateFlightPlan(DataModel.XAirportInfo departure, DataModel.XAirportInfo approch, List<String> points)
        {
            if (!isInit)
                await InitNavData();

            DataModel.XFlightPlan rtn = new DataModel.XFlightPlan(departure, approch);

            List<DataModel.XNavpoint> listpoint = new List<DataModel.XNavpoint>();
            Dictionary<string, DataModel.XNavpoint> dictpoint = new Dictionary<string, DataModel.XNavpoint>();
            for(int i =0;i<=points.Count-1;i++)
            {
                listpoint.Add(new DataModel.XNavpoint()
                {
                    PointName = points[i].ToUpper().Trim(),
                    PointType = TypeofNavpoint(points[i],i),
                });
            }
            rtn.Waypoints.InsertRange(1, listpoint);

            #region Find all waypoint
            for(int i=0;i<=rtn.Waypoints.Count -1;i++)
            {
                if(rtn.Waypoints[i].PointType==0 && !dictpoint.ContainsKey(rtn.Waypoints[i].PointName))
                    dictpoint.Add(rtn.Waypoints[i].PointName, rtn.Waypoints[i]);

                if (rtn.Waypoints[i].PointType == 3 && rtn.Waypoints[i].Longtitude.Length == 0 && !dictpoint.ContainsKey(rtn.Waypoints[i].PointName))
                {
                    DataModel.XNavpoint p = await GetWaypoint(rtn.Waypoints[i].PointName);
                    dictpoint.Add(p.PointName, p);
                }
                    //rtn.Waypoints[i] = await GetWaypoint(rtn.Waypoints[i].PointName);

                if(rtn.Waypoints[i].PointType == 2)
                {
                    List<DataModel.XNavpoint> atsinfo = await GetATSInfo(rtn.Waypoints[i - 1].PointName, rtn.Waypoints[i + 1].PointName, rtn.Waypoints[i].PointName);
                    foreach(DataModel.XNavpoint p in atsinfo)
                    {
                        if(!dictpoint.ContainsKey(p.PointName))
                            dictpoint.Add(p.PointName, p);
                    }
                        
                }
            }
            #endregion
            rtn.Waypoints = dictpoint.Values.ToList();

            return rtn;
        }

        public async Task<DataModel.XNavpoint> GetWaypoint(string waypoint)
        {
            DataModel.XNavpoint rtn = null;
            if (!isInit)
                await InitNavData();
            string waypointinfo = StringUtils.CutString(waypoint.ToUpper(), "\r\n", sWaypoints);

            if (waypointinfo.Length == 0)
                return null;

            string[] sp = waypointinfo.Split("|".ToCharArray());
            rtn = new DataModel.XNavpoint();
            rtn.PointName = waypoint.ToUpper();
            rtn.Longtitude = sp[2];
            rtn.Lantitude = sp[1];
            rtn.Fruquence = "";
            rtn.PointType = 3;

            return rtn;
        }

        public async Task<List<DataModel.XNavpoint>> GetATSInfo(string startpoint,string endpoint,string ATS)
        {
            List<DataModel.XNavpoint> rtn = new List<DataModel.XNavpoint>();
            if (!isInit)
                await InitNavData();
            List<String> atsinfos = StringUtils.CutStrings("A|" + ATS, "\r\n\r", sATS);
            string ATSINFO = string.Empty;
            for(int i=0;i<=atsinfos.Count-1;i++)
            {
                int iStart = 0;
                int iEnd = 0;
                iStart = atsinfos[i].IndexOf(startpoint);
                iEnd = atsinfos[i].IndexOf(endpoint);
                if (iStart>0 && iEnd>0 && iStart<iEnd)
                {
                    ATSINFO = atsinfos[i];
                }
            }
            Dictionary<string, DataModel.XNavpoint> map = new Dictionary<string, DataModel.XNavpoint>();
            ATSINFO = ATSINFO.Substring(ATSINFO.IndexOf("S|" + startpoint + "|"));
            foreach (string line in ATSINFO.Split("\r".ToCharArray()))
            {
                string[] s = line.Split("|".ToCharArray());
                if (!map.ContainsKey(s[1]))
                    map.Add(s[1], new DataModel.XNavpoint() { PointName = s[1], Longtitude = s[3], Lantitude =s[2] });
                if (!map.ContainsKey(s[4]))
                    map.Add(s[4], new DataModel.XNavpoint() { PointName = s[4], Longtitude = s[6], Lantitude = s[5] });
                if (s[4] == endpoint)
                    break;
            }
            rtn = map.Values.ToList();



            return rtn;

        }

        /// <summary>
        /// Get Navpoint Type
        /// </summary>
        /// <returns>
        /// 0 Airport
        /// 1 SID/STAR Point like VM URA
        /// 2 Airway A321 W770
        /// 3 Waypoint LADIX 0392N
        /// </returns>
        private int TypeofNavpoint(string point,int idx)
        {
            int iRtn = -1;
            point = point.ToUpper();
            if (point.Length == 5 && idx%2 == 0)
                return 3;

            if (StringUtils.HasNumber(point))
                return 2;
            else
                return 1;

            return iRtn;
        }
    }
}
