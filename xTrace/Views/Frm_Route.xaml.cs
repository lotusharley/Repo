using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
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

namespace xTrace.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Frm_Route : Page
    {
        Windows.UI.Popups.MessageDialog msgbox = new Windows.UI.Popups.MessageDialog("");
        System.Collections.ObjectModel.ObservableCollection<DataModel.XNavpoint> navpoints = new System.Collections.ObjectModel.ObservableCollection<DataModel.XNavpoint>();
        List<String> cycles = new List<string>();
        Page rootFrame;
        DataModel.XFlightPlan flightplan;
        public Frm_Route()
        {
            this.InitializeComponent();
            rootFrame = MainPage.GetInstance();
            Init();
        }

        private async void Init()
        {
            cycles = await Utils.RouteBuilder.GetCycleInfo();
            int maxcycles = 0;
            foreach (string s in cycles)
            {
                int i = 0;
                list_Cycles.Items.Add(s);
                if (int.TryParse(s, out i))
                {
                    if (i > maxcycles)
                        maxcycles = i;
                }
            }
            for (int i = 0; i <= list_Cycles.Items.Count - 1; i++)
            {
                if (list_Cycles.Items[i].ToString() == maxcycles.ToString())
                {
                    list_Cycles.SelectedIndex = i;
                    break;
                }
            }
        }

        private async void DrawFlightPlane()
        {
            List<Windows.Devices.Geolocation.BasicGeoposition> geopoints = new List<Windows.Devices.Geolocation.BasicGeoposition>();
            //Windows.Devices.Geolocation.Geopath geopath = new Windows.Devices.Geolocation.Geopath()
            List<Windows.UI.Xaml.Controls.Maps.MapIcon> waypointIcons = new List<Windows.UI.Xaml.Controls.Maps.MapIcon>();
            foreach (DataModel.XNavpoint point in flightplan.Waypoints)
            {
                navpoints.Add(point);
                geopoints.Add(new Windows.Devices.Geolocation.BasicGeoposition() { Longitude = double.Parse(point.Longtitude) / 1000000 - 0.000005, Latitude = double.Parse(point.Lantitude) / 1000000 - 0.000005 });
                System.Diagnostics.Debug.WriteLine(double.Parse(point.Longtitude) / 1000000 + " " + double.Parse(point.Lantitude) / 1000000);

                Windows.UI.Xaml.Controls.Maps.MapIcon icon = new Windows.UI.Xaml.Controls.Maps.MapIcon();
                icon.Image = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/navpoint.png"));
                icon.Location = new Windows.Devices.Geolocation.Geopoint(new Windows.Devices.Geolocation.BasicGeoposition()
                {
                    Latitude = double.Parse(point.Lantitude) / 1000000 - 0.000005,
                    Longitude = double.Parse(point.Longtitude)/1000000 - 0.000005
                });
                icon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                icon.Title = point.PointName;
                waypointIcons.Add(icon);
            }
            Windows.Devices.Geolocation.Geopath geopath = new Windows.Devices.Geolocation.Geopath(geopoints);

            await rootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                
                Windows.UI.Xaml.Controls.Maps.MapControl themap = (Windows.UI.Xaml.Controls.Maps.MapControl)rootFrame.FindName("themap");
                Windows.UI.Xaml.Controls.Maps.MapPolyline line = new Windows.UI.Xaml.Controls.Maps.MapPolyline();
                line.Path = geopath;
                line.StrokeColor = Windows.UI.Colors.Red;
                line.StrokeDashed = true;
                line.StrokeThickness = 3;
                themap.MapElements.Clear();
                themap.MapElements.Add(line);

                foreach(Windows.UI.Xaml.Controls.Maps.MapIcon icon in waypointIcons)
                {
                    themap.MapElements.Add(icon);
                }
            });
        }

        private async void btn_AppendNavpoint_Click(object sender, RoutedEventArgs e)
        {
            #region Init RouteBuilder
            Utils.RouteBuilder routebuilder = new Utils.RouteBuilder(list_Cycles.SelectedItem.ToString());
            #endregion

            #region Find departure/approch airport infomation.
            DataModel.XAirportInfo departureairport = await routebuilder.GetAirportInfo(txt_Departure_ICAO.Text);
            if(txt_Departure_ICAO.Text.Length != 4 || txt_Approch_ICAO.Text.Length != 4)
            {
                msgbox.Content = "Departure ICAO and Approch ICAO code length Must be 4";
                await msgbox.ShowAsync();
                return;
            }

            if (departureairport != null)
            {
                txt_Departure_Description.Text = departureairport.Airportname + "\r\n" +
                    "Longtitude " + departureairport.Longtitude + "\r\n" +
                    "Latitude   " + departureairport.Latitude + "\r\n" +
                    "Altitude   " + departureairport.Altitude;
            }
            else
            {
                this.msgbox.Content = "Can't Find Departure ICAO \"" + txt_Departure_ICAO.Text + "\"";
                await msgbox.ShowAsync();
                return;
            }

            DataModel.XAirportInfo approchairport = await routebuilder.GetAirportInfo(txt_Approch_ICAO.Text);
            if(approchairport != null)
            {
                txt_Approch_Description.Text = approchairport.Airportname + "\r\n" +
                    "Longtitude " + approchairport.Longtitude + "\r\n" +
                    "Latitude   " + approchairport.Latitude + "\r\n" +
                    "Altitude   " + approchairport.Altitude;
            }
            else
            {
                this.msgbox.Content = "Can't Find Approch ICAO \"" + txt_Approch_ICAO.Text + "\"";
                await msgbox.ShowAsync();
                return;
            }
            #endregion

            #region Find Route
            List<String> waypoints = new List<string>();
            foreach(string s in txt_Waypoint_Source.Text.Split(" ".ToCharArray()))
            {
                string st = s.Trim().ToUpper();
                if (st != "DCT" && st != "SID" && st != "STAR" && st.Length>0)
                    waypoints.Add(st);
            }

            flightplan = await routebuilder.CreateFlightPlan(departureairport, approchairport, waypoints);
            navpoints.Clear();

            DrawFlightPlane();

            lst_Waypoints.ItemsSource = navpoints;
            msgbox.Content = "Done";
            await msgbox.ShowAsync();
            #endregion
        }

        private void btn_CancelRoute_Click(object sender, RoutedEventArgs e)
        {
            flightplan = null;
            navpoints.Clear();
            txt_Departure_Description.Text = string.Empty;
            txt_Approch_Description.Text = string.Empty;
            txt_Approch_ICAO.Text = string.Empty;
            txt_Departure_ICAO.Text = string.Empty;
            txt_Waypoint_Source.Text = string.Empty;
        }

        private async void ListItemButtom_Click(object sender, RoutedEventArgs e)
        {
            Button btn_Sender = (Button)sender;
            string cmdArgs = btn_Sender.CommandParameter.ToString();

            if(cmdArgs == flightplan.Approch.Airportcode || cmdArgs == flightplan.Departure.Airportcode)
            {
                msgbox.Content = "Can't Remove Approch/Departure Airport From FlightPlane";
                await msgbox.ShowAsync();
                return;
            }

            foreach(DataModel.XNavpoint p in flightplan.Waypoints)
            {
                if(p.PointName == cmdArgs)
                {
                    flightplan.Waypoints.Remove(p);
                    break;
                }
            }
            DrawFlightPlane();
            lst_Waypoints.ItemsSource = null;
            lst_Waypoints.ItemsSource = flightplan.Waypoints;
        }
    }
}
