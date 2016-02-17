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

// “内容对话框”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上进行了说明

namespace xTrace.Views
{
    public sealed partial class Dlg_StartTrace : ContentDialog
    {
        public Dlg_StartTrace()
        {
            this.InitializeComponent();

            string IPAddr, Port;
            IPAddr = Control.ConfigUtils.GetInstance().LoadConfig().IPADDR;
            Port = Control.ConfigUtils.GetInstance().LoadConfig().PORT;

            txt_Content.Text = "IP Address:" + IPAddr + "\r\n" + "Port:" + Port + "\r\nWhen Received Any Data this dialog will automatic close.";
            
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }
    }
}
