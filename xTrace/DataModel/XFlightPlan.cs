using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.DataModel
{
    public class XFlightPlan
    {
        public XAirportInfo Departure { get; set; }
        public XAirportInfo Approch { get; set; }

        public List<XNavpoint> Waypoints { get; set; }

        public XFlightPlan(XAirportInfo departure, XAirportInfo approch)
        {
            Departure = departure;
            Approch = approch;
            Waypoints = new List<XNavpoint>();
            Waypoints.Add(new XNavpoint()
            {
                PointName = Departure.Airportcode,
                Fruquence = "",
                Longtitude = Departure.Longtitude,
                Lantitude = Departure.Latitude,
                PointType = 0
            });

            Waypoints.Add(new XNavpoint()
            {
                PointName = Approch.Airportcode,
                Fruquence = "",
                Longtitude = Approch.Longtitude,
                Lantitude = Approch.Latitude,
                PointType = 0
            });
        }
    }
}
