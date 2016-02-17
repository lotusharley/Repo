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

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace xTrace
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DataModel.XCurrent_Status xStatus = new DataModel.XCurrent_Status();
        bool isTrace = false;

        Windows.UI.ViewManagement.ApplicationViewTitleBar titleBar;
        Windows.ApplicationModel.Core.CoreApplicationViewTitleBar coreTitleBar;
        Windows.UI.Color colorTitleBg = Windows.UI.ColorHelper.FromArgb(0, 31, 31, 31);
        Windows.UI.Color colorTitleFg = Windows.UI.Colors.White;
        Windows.UI.Color colorTitleButtonBg = Windows.UI.ColorHelper.FromArgb(0, 54, 54, 54);
        Control.xTraceGPSEFISReceiver xReceiver;
        Windows.UI.Xaml.Controls.Button btn_xPlane = null;
        private static Views.Dlg_StartTrace dlgTrace;
        private static MainPage _instance;
        public static MainPage GetInstance()
        {
            return _instance;
        }

        public MainPage()
        {
            this.InitializeComponent();
            
            titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            CustomerTitleBar();
            _instance = this;
        }

        public void CustomerTitleBar()
        {
            coreTitleBar.ExtendViewIntoTitleBar = true;

            Window.Current.SetTitleBar(GridTitleBar);
            
            titleBar.BackgroundColor = colorTitleBg;
            titleBar.ForegroundColor = colorTitleFg;
            titleBar.ButtonBackgroundColor = colorTitleButtonBg;

            titleBar.ButtonForegroundColor = colorTitleFg;
        }

        private void splitViewToggle_Click(object sender, RoutedEventArgs e)
        {
            vw_Menu.Visibility = Visibility.Visible;
            vw_Menu.IsPaneOpen = !vw_Menu.IsPaneOpen;
            if (!vw_Menu.IsPaneOpen)
            {
                list_Menu.SelectedItem = null;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            StackPanel item = (StackPanel)e.ClickedItem;
            String sDataContext = "";

            if (item.DataContext == null)
                sDataContext = "";
            else
                sDataContext = item.DataContext.ToString();

            switch(sDataContext)
            {
                case "MENU":
                    splitViewToggle_Click(sender, null);
                    break;
                case "MAP":
                    vw_Menu.IsPaneOpen = false;
                    break;
                case "ROUTE":
                    vw_Menu.IsPaneOpen = false;
                    frm_Main.Navigate(typeof(Views.Frm_Route),this);
                    vw_Menu.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                    break;
                case "CONTROLLER":
                    vw_Menu.IsPaneOpen = false;
                    break;
                case "SETTING":
                    frm_Main.Navigate(typeof(Frm_Setting));
                    vw_Menu.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                    break;
                case "TRACKING":
                    StartTrace(item);
                    break;
                default:
                    splitViewToggle_Click(sender, null);
                    break;
            }

        }

        private async void StartTrace(StackPanel srcObject)
        {
            ListViewItem item = (ListViewItem)srcObject.Parent;
            vw_Menu.IsPaneOpen = false;
            isTrace = !isTrace;
            if(isTrace)
            {
                item.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                xReceiver = Control.xTraceGPSEFISReceiver.GetInstance();
                xReceiver.onXPlaneStatReceived += XReceiver_onXPlaneStatReceived;
                Control.xTraceGPSEFISReceiver.GetInstance().StartReceiveAsync();
                dlgTrace = new Views.Dlg_StartTrace();
                await dlgTrace.ShowAsync();
                
            }
            else
            {
                item.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
                Control.xTraceGPSEFISReceiver.GetInstance().StopReceive();
                themapitmes.ItemsSource = null;
                
            }

            list_Menu.SelectedItem = null;
        }

        private void XReceiver_onXPlaneStatReceived(object sender, DataModel.XCurrent_Status e)
        {
            if (dlgTrace != null)
            {
                dlgTrace.Hide();
                dlgTrace = null;
            }
            xStatus = e;
            BindToMap();
        }

        private async void BindToMap()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if(btn_xPlane == null)
                {
                }
                
                List<DataModel.XCurrent_Status> list = new List<DataModel.XCurrent_Status>();
                if (xStatus.GPSStatus != null && xStatus.ATTStatus != null)
                {
                    list.Add(xStatus);
                    themapitmes.ItemsSource = list;

                    txt_Longtitude.Text = "Longtitude:" + xStatus.GPSStatus.Longitude.ToString();
                    txt_Altitude.Text = "Altitude:" + xStatus.GPSStatus.Altitude.ToString();
                    txt_Latitude.Text = "Latitude:" + xStatus.GPSStatus.Latitude.ToString();
                    txt_HeadingTrueNorth.Text = "Heading:" + xStatus.GPSStatus.HeadingTrueNorth.ToString();
                    txt_GroundSpeed.Text = "GroundSpeed:" + xStatus.GPSStatus.GroundSpeed.ToString();
                }
                return;
            });
        }

        private void cmd_Map3D_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
