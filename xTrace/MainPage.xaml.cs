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
        private int maplayout = 0;
        private double mapZoomLevel;
        private static Views.Dlg_StartTrace dlgTrace;
        private static MainPage _instance;
        public static MainPage GetInstance()
        {
            return _instance;
        }

        Windows.Devices.Geolocation.Geoposition myposition;

        public MainPage()
        {
            this.InitializeComponent();
            
            titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            CustomerTitleBar();
            _instance = this;
            InitMapData();
            cmd_MyFlight.IsEnabled = false;
            mapZoomLevel = themap.ZoomLevel;
            themap.ZoomLevelChanged += Themap_ZoomLevelChanged;
        }

        private void Themap_ZoomLevelChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, object args)
        {
            mapZoomLevel = themap.ZoomLevel;
            if (mapZoomLevel >= 18)
                cmd_MapZoom.IsEnabled = false;
            else
                cmd_MapZoom.IsEnabled = true;

            if (mapZoomLevel == 1)
                cmd_MapZoomout.IsEnabled = false;
            else
                cmd_MapZoomout.IsEnabled = true;

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

        public async void InitMapData()
        {
            myposition = await Utils.GEOUtils.GetMachineLocation();
            if (myposition == null)
                return;
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    themap.Center = myposition.Coordinate.Point;
                });
            }
            
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
                cmd_MyFlight.IsEnabled = false;
                
            }

            list_Menu.SelectedItem = null;
        }

        private void XReceiver_onXPlaneStatReceived(object sender, DataModel.XCurrent_Status e)
        {
            if (dlgTrace != null)
            {
                dlgTrace.Hide();
                dlgTrace = null;
                cmd_MyFlight.IsEnabled = true;
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

        private async void cmd_Map3D_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (themap.DesiredPitch == 60)
                    themap.DesiredPitch = 0;
                else
                    themap.DesiredPitch = 60;

            });
            
        }

        private async void cmd_MyFlight_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Windows.Devices.Geolocation.Geopoint mypoint = this.xStatus.GPSStatus.GeoPointInfo;
                themap.Center = mypoint;
            });
        }

        private async void cmd_Layout_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.maplayout = this.maplayout + 1;
                if(this.maplayout >2)
                {
                    this.maplayout = 0;
                }

                switch(this.maplayout)
                {
                    case 0:
                        themap.Style = Windows.UI.Xaml.Controls.Maps.MapStyle.Road;
                        break;
                    case 1:
                        themap.Style = Windows.UI.Xaml.Controls.Maps.MapStyle.AerialWithRoads;
                        break;
                    case 2:
                        themap.Style = Windows.UI.Xaml.Controls.Maps.MapStyle.Terrain;
                        break;
                }
            });
        }

        private async void cmd_MapZoom_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (mapZoomLevel < 18)
                {
                    mapZoomLevel = mapZoomLevel + 1;
                    themap.ZoomLevel = mapZoomLevel;
                }
            });
        }

        private async void cmd_MapZoomout_Click(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (mapZoomLevel > 1)
                {
                    mapZoomLevel = mapZoomLevel - 1;
                    themap.ZoomLevel = mapZoomLevel;
                }
            });
        }
    }
}
