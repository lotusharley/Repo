using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.Utils
{
    public class GEOUtils
    {
        public static async Task<Windows.Devices.Geolocation.Geoposition> GetMachineLocation()
        {
            Windows.Devices.Geolocation.Geoposition rtn = null;

            try
            {
                Windows.Devices.Geolocation.GeolocationAccessStatus accessStatus = await Windows.Devices.Geolocation.Geolocator.RequestAccessAsync();
                switch(accessStatus)
                {
                    case Windows.Devices.Geolocation.GeolocationAccessStatus.Allowed:
                        Windows.Devices.Geolocation.Geolocator geolocator = new Windows.Devices.Geolocation.Geolocator();
                        rtn = await geolocator.GetGeopositionAsync();
                        break;
                    case Windows.Devices.Geolocation.GeolocationAccessStatus.Denied:
                        rtn = null;
                        break;
                    case Windows.Devices.Geolocation.GeolocationAccessStatus.Unspecified:
                        rtn = null;
                        break;
                }
            }
            finally
            {

            }

            return rtn;
        }
    }
}
