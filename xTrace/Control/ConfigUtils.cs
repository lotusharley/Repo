using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.Control
{
    public class ConfigUtils
    {
        private static ConfigUtils _Instance;
        Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        private static DataModel.xTraceConfig CurrentSettings;

        public static ConfigUtils GetInstance()
        {
            if (_Instance == null)
                _Instance = new ConfigUtils();
            return _Instance;
        }

        private ConfigUtils()
        {

        }

        public DataModel.xTraceConfig InitAppsettings()
        {
            DataModel.xTraceConfig xConfig = new DataModel.xTraceConfig();
            System.Reflection.PropertyInfo[] properinfos = System.Reflection.TypeExtensions.GetProperties(typeof(DataModel.xTraceConfig));
            
            xConfig.ISFIRSTRUN = true.ToString();
            xConfig.IPADDR = "";
            xConfig.PORT = "49002";
            SaveConfig(xConfig);
            return xConfig;
        }

        public DataModel.xTraceConfig LoadConfig()
        {
            if (CurrentSettings == null)
                CurrentSettings = new DataModel.xTraceConfig();
            System.Reflection.PropertyInfo[] propertyinfos = System.Reflection.TypeExtensions.GetProperties(typeof(DataModel.xTraceConfig));
            foreach(System.Reflection.PropertyInfo p in propertyinfos)
            {
                if(localSettings.Values[p.Name] == null)
                {
                    InitAppsettings();
                }
                else
                {
                    p.SetValue(CurrentSettings, localSettings.Values[p.Name].ToString());
                }
            }

            return CurrentSettings;
        }

        public void SaveConfig(DataModel.xTraceConfig xConfig)
        {
            localSettings.Values["ISFIRSTRUN"] = xConfig.ISFIRSTRUN;
            localSettings.Values["IPADDR"] = xConfig.IPADDR;
            localSettings.Values["PORT"] = xConfig.PORT;
            CurrentSettings = xConfig;
        }
    }
}
