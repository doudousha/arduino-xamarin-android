using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Net.Wifi;
using Android.Widget;
using Android.OS;
using Android.Util;
using Java.Lang;
using Exception = System.Exception;

namespace UdpSendExample1
{
    [Activity(Label = "UdpSendExample1", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private string _ip = "192.168.0.102";
        private int _port = 4210;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var open = FindViewById<Button>(Resource.Id.button1);
            var close = FindViewById<Button>(Resource.Id.button2);
            var seekBar = FindViewById<SeekBar>(Resource.Id.seekBar1);

            open.Click += Open_Click;
            close.Click += Close_Click;

            seekBar.ProgressChanged += SeekBar_ProgressChanged;
        }

        private void SeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            SendData(_ip, _port, e.Progress.ToString());
        }

        private void Close_Click(object sender, System.EventArgs e)
        {
            SendData(_ip, _port, "0");
        }

        private void Open_Click(object sender, System.EventArgs e)
        {
            SendData(_ip, _port, "254");
        }

        private void SendData(string ip, int port, string sendData)
        {
            IPEndPoint ep1 = new IPEndPoint(IPAddress.Any, 1234);

            UdpClient sendClient = new UdpClient();
            sendClient.ExclusiveAddressUse = false;
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint ep2 = new IPEndPoint(IPAddress.Parse(ip), port);
            sendClient.Client.Bind(ep1);
            byte[] senDatas = Encoding.ASCII.GetBytes(sendData);
            sendClient.Send(senDatas, senDatas.Length, ep2);


            //var dgram = sendClient.Receive(ref ep1);
            //string receiveContent = Encoding.ASCII.GetString(dgram);

        }
    }
}

