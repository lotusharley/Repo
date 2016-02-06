using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xTrace.DataModel
{
    public class XNavpoint
    {
        public string PointName { get; set; }
        public string Fruquence { get; set; }
        public string Lantitude { get; set; }
        public string Longtitude { get; set; }
        public float Distance { get; set; }
        public float Heading { get; set; }
        public int PointType { get; set; }


        public XNavpoint()
        {
            PointName = string.Empty;
            Fruquence = string.Empty;
            Lantitude = string.Empty;
            Longtitude = string.Empty;
        }
    }

}
