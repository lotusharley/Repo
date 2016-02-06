using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace xTrace
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Frm_Setting : Page
    {
        DataModel.xTraceConfig xConfig = Control.ConfigUtils.GetInstance().LoadConfig();
        public Frm_Setting()
        {
            this.InitializeComponent();

            IReadOnlyList<Windows.Networking.HostName> networkinfo =  Windows.Networking.Connectivity.NetworkInformation.GetHostNames();
            foreach(Windows.Networking.HostName hostname in networkinfo)
            {
                if (hostname.IPInformation != null)
                {
                    Windows.Networking.Connectivity.IPInformation ipinfo = hostname.IPInformation;
                    if(ipinfo.PrefixLength == 24)
                    {
                        list_Interface.Items.Add(new TextBlock() { Text = hostname.RawName});
                        if (xConfig.IPADDR == hostname.RawName)
                            list_Interface.SelectedIndex = list_Interface.Items.Count - 1;
                    }
                }
            }
            
            txt_UDPPort.Text = xConfig.PORT;
            
        }

        private void cmd_SaveNetworkSetting_Click(object sender, RoutedEventArgs e)
        {
            xConfig.IPADDR = ((TextBlock)list_Interface.SelectedItem).Text;
            xConfig.PORT = txt_UDPPort.Text;
            Control.ConfigUtils.GetInstance().SaveConfig(xConfig);
            
        }
    }
}
