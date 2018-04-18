using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Net.Wifi;
using Android.Widget;
using Android.OS;
using Android.Util;

namespace UdpSendReceiveExample
{
    [Activity(Label = "UdpSendReceiveExample", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private string _ip;
        private int _port = 4210;
        private int _listenPort = 10099;

        // 打开android日志 试图-> 其他窗口->设备日志

        private UdpClient receiveClient;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var open = FindViewById<Button>(Resource.Id.button1);
            var close = FindViewById<Button>(Resource.Id.button2);
            var seekBar = FindViewById<SeekBar>(Resource.Id.seekBar1);
            var textView = FindViewById<TextView>(Resource.Id.textView1);

            open.Click += Open_Click;
            close.Click += Close_Click;
            seekBar.ProgressChanged += SeekBar_ProgressChanged;

            Task.Run(() =>
            {

                Log.Info("myapp", "正在监听");

                while (true)
                {
                    try
                    {
                        receiveClient = new UdpClient();
                        IPEndPoint ep1 = new IPEndPoint(IPAddress.Any, _listenPort);
                        receiveClient.ExclusiveAddressUse = false;
                        receiveClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress,
                            true);
                        receiveClient.Client.Bind(ep1);
                        byte[] buffer = receiveClient.Receive(ref ep1);
                        string content = Encoding.ASCII.GetString(buffer);

                        RunOnUiThread(() =>
                        {
                            _ip = ep1.Address.ToString();
                            Log.Info("收到", ep1.Address.ToString() + ":" + ep1.Port + content);
                            textView.Text = ep1.Address.ToString() + ":" + ep1.Port + content;
                        });
                        receiveClient.Dispose();
                    }
                    catch (Exception ex)
                    {
                        receiveClient.Dispose();
                        Log.Info("错误", ex.Message);
                    }
                }

            });

        }

        private void SeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            SendData(e.Progress.ToString());
        }

        private void Close_Click(object sender, System.EventArgs e)
        {
            SendData("0");
        }

        private Timer timer;

        private void Open_Click(object sender, System.EventArgs e)
        {
            WifiManager manager = (WifiManager)GetSystemService(Service.WifiService);
            int ip = manager.ConnectionInfo.IpAddress;

            string ipaddress = Android.Text.Format.Formatter.FormatIpAddress(ip);
            string ipStr = ipaddress.Remove(ipaddress.LastIndexOf(".") + 1);

            //Task.Run(() =>
            //{
            //    for (int i = 255; i > 0; i--)
            //    {

            //        try
            //        {
            //            //string iptemp = ipStr + i;
            //            string iptemp = "192.168.0.105";
            //            SendData(iptemp, "try");
            //            Log.Info("try", iptemp);
            //        }
            //        catch (Exception ex)
            //        {
            //            Log.Info("tryError", ex.Message);
            //        }
            //        Task.Delay(100);
            //    }
            //});
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var sendClient = new UdpClient();
                    IPEndPoint ep1 = new IPEndPoint(IPAddress.Any, _listenPort);
                    sendClient.ExclusiveAddressUse = false;
                    sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    IPEndPoint ep2 = new IPEndPoint(IPAddress.Broadcast, _port);
                    sendClient.Client.Bind(ep1);
                    byte[] content = Encoding.ASCII.GetBytes("try");
                    sendClient.Send(content, content.Length, ep2);
                    sendClient.Dispose();
                    Log.Info("try", i + "");
                }
                catch (Exception ex)
                {
                    Log.Info("tryError", ex.Message);

                }
                Thread.Sleep(200); // 设置间隔，发送成功路会增加
            }

        }

        private void SendData(string sendData)
        {
            var sendClient = new UdpClient();
            IPEndPoint ep1 = new IPEndPoint(IPAddress.Any, _listenPort);
            sendClient.ExclusiveAddressUse = false;
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint ep2 = new IPEndPoint(IPAddress.Parse(_ip), _port);
            sendClient.Client.Bind(ep1);
            byte[] content = Encoding.ASCII.GetBytes(sendData);
            sendClient.Send(content, content.Length, ep2);
            sendClient.Dispose();

        }


        private void SendData(string ip, string sendData)
        {
            var sendClient = new UdpClient();
            IPEndPoint ep1 = new IPEndPoint(IPAddress.Any, _listenPort);
            sendClient.ExclusiveAddressUse = false;
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint ep2 = new IPEndPoint(IPAddress.Parse(ip), _port);
            sendClient.Client.Bind(ep1);
            byte[] content = Encoding.ASCII.GetBytes(sendData);
            sendClient.Send(content, content.Length, ep2);
            sendClient.Dispose();

        }


        protected override void OnDestroy()
        {
            if (receiveClient != null)
            {
                receiveClient.Dispose();
            }
            base.OnDestroy();
        }
    }

}

