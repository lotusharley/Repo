using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace xTrace.Control
{
    public sealed class xTraceGPSEFISReceiver
    {
        private static xTraceGPSEFISReceiver _Instance;
        private string IPAddr;
        private string Port;
        private Windows.Networking.Sockets.DatagramSocket udpSocket;
        private bool isRun = false;
        private bool isReceive = false;
        Windows.Storage.Streams.IOutputStream outstm;
        private int waitSeconds;
        public IGPSEFISReceived iGPSEFISReceived;
        private DataModel.XCommand_GPS CurrentGPS;
        private DataModel.XCommand_ATT CurrentATT;
        private DataModel.XCurrent_Status CurrentStat;

        public event EventHandler<DataModel.XCurrent_Status> onXPlaneStatReceived;

        private xTraceGPSEFISReceiver()
        {
            IPAddr = ConfigUtils.GetInstance().LoadConfig().IPADDR;
            Port = ConfigUtils.GetInstance().LoadConfig().PORT;
            CurrentStat = new DataModel.XCurrent_Status();

            waitSeconds = 3;

        }

        public static xTraceGPSEFISReceiver GetInstance()
        {
            if (_Instance == null)
                _Instance = new xTraceGPSEFISReceiver();
            return _Instance;
        }

        public async void StartReceiveAsync()
        {
            isRun = true;
            while(isRun)
            {
                if(!isReceive)
                {
                    System.Diagnostics.Debug.WriteLine("Start Receive");
                    udpSocket = new Windows.Networking.Sockets.DatagramSocket();
                    udpSocket.MessageReceived += UdpSocket_MessageReceived;

                    Windows.Networking.HostName hostName = null;
                    IReadOnlyList<Windows.Networking.HostName> networkinfo = Windows.Networking.Connectivity.NetworkInformation.GetHostNames();
                    foreach (Windows.Networking.HostName h in networkinfo)
                    {
                        if (h.IPInformation != null)
                        {
                            Windows.Networking.Connectivity.IPInformation ipinfo = h.IPInformation;
                            if (h.RawName == IPAddr)
                            {
                                hostName = h;
                                break;
                            }
                        }
                    }
                    if (hostName != null)
                        await udpSocket.BindEndpointAsync(hostName, Port);
                    else
                        await udpSocket.BindServiceNameAsync(Port);
                    outstm = await udpSocket.GetOutputStreamAsync(new Windows.Networking.HostName("255.255.255.255"), "49002");
                    await outstm.FlushAsync();
                    Windows.Storage.Streams.DataWriter dw = new Windows.Storage.Streams.DataWriter(outstm);
                    dw.WriteString("Start Receive");
                    await dw.StoreAsync();
                    isReceive = true;
                }
                else
                {
                    if(CurrentStat.GPSStatus !=null & CurrentStat.ATTStatus != null)
                    {
                        if (onXPlaneStatReceived != null)
                            onXPlaneStatReceived.Invoke(this, CurrentStat);
                    }
                    System.Diagnostics.Debug.WriteLine("Try To Sleep");
                    isReceive = false;
                    await udpSocket.CancelIOAsync();
                    udpSocket.MessageReceived -= UdpSocket_MessageReceived;
                    udpSocket.Dispose();
                    udpSocket = null;
                }
                await Task.Delay(waitSeconds*1000);
            }
        }

        public async void StopReceive()
        {
            try
            {
                isRun = false;
                if(isReceive)
                {
                    await udpSocket.CancelIOAsync();
                    udpSocket.MessageReceived -= UdpSocket_MessageReceived;
                    isReceive = false;
                }
                    
            }
            finally
            {
                if(udpSocket != null)
                {
                    udpSocket.Dispose();
                    udpSocket = null;
                }
                
                _Instance = null;
            }
        }

        private void UdpSocket_MessageReceived(Windows.Networking.Sockets.DatagramSocket sender, Windows.Networking.Sockets.DatagramSocketMessageReceivedEventArgs args)
        {
            
            try
            {
                
                Windows.Storage.Streams.DataReader dr = args.GetDataReader();
                dr.InputStreamOptions = Windows.Storage.Streams.InputStreamOptions.Partial;
                uint uLength = dr.UnconsumedBufferLength;
                string sCmd = dr.ReadString(uLength);
                DataModel.BaseCommand XCmd = DataModel.XCommandFactory.GetInstance().CreateCommand(sCmd);

                if (XCmd.CommandType == DataModel.XPCommandType.TYPE_XGPS)
                {
                    CurrentStat.GPSStatus = (DataModel.XCommand_GPS)XCmd;
                }
                    

                if (XCmd.CommandType == DataModel.XPCommandType.TYPE_XATT)
                {
                    CurrentStat.ATTStatus = (DataModel.XCommand_ATT)XCmd;
                }
                    

                System.Diagnostics.Debug.WriteLine("Received Message:" + sCmd);
            }
            catch(Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.ToString());
            }
            
        }
    }
}
