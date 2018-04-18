using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using Android.App;
using Android.Widget;
using Android.OS;
using Java.Lang;

namespace ArduinoHttpExample
{
    [Activity(Label = "ArduinoHttpExample", MainLauncher = true)]
    public class MainActivity : Activity
    {

        private string url = "http://192.168.16.126:80";
        private HttpClient _client = new HttpClient();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.slider);

            //var openBtn = FindViewById<Button>(Resource.Id.btn_open);
            //var closeBtn = FindViewById<Button>(Resource.Id.btn_close);
            //var resultTxt = FindViewById<TextView>(Resource.Id.txt_result);
            var seekBar = FindViewById<SeekBar>(Resource.Id.seekBar1);
            seekBar.ProgressChanged += (sender, e) =>
            {
                SendData("192.168.0.108", 4210, e.Progress.ToString());
            };
            //openBtn.Click += async (sender, ele) =>
            //{
            //    var result = await _client.GetAsync(string.Format("{0}?state={1}",url,"ok"));
            //    resultTxt.Text = result.Content.ReadAsStringAsync().Result;

            //};

            //closeBtn.Click += async (sender, ele) =>
            //{
            //    var result = await _client.GetAsync(string.Format("{0}?state={1}", url, "close"));
            //    resultTxt.Text = result.Content.ReadAsStringAsync().Result;
            //};



            //openBtn.Click +=  (sender, ele) =>
            //{
            //    SendData("192.168.0.107", 4210, "hello",resultTxt);
            //};

         
        }

        //private void sendUdp(string ip,int port ,string content)
        //{
        //    UdpClient fubar = new UdpClient();
        //    IPAddress address = IPAddress.Parse(ip);
        //    fubar.Connect(address, port);
        //    byte[] contents = Encoding.ASCII.GetBytes(content);
        //    fubar.Send(contents, contents.Length);
        //}

        private void SendData(string ip, int port, string sendData,TextView textView)
        {
            IPEndPoint ep1 = new IPEndPoint(IPAddress.Any, 1234);

            UdpClient sendClient = new UdpClient();
            sendClient.ExclusiveAddressUse = false;
            sendClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint ep2 = new IPEndPoint(IPAddress.Parse(ip), port);
            sendClient.Client.Bind(ep1);
            byte[] senDatas = Encoding.ASCII.GetBytes(sendData);
            sendClient.Send(senDatas, senDatas.Length, ep2);


            var dgram = sendClient.Receive(ref ep1);
            string receiveContent = Encoding.ASCII.GetString(dgram);
            textView.Text = receiveContent;
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


            var dgram = sendClient.Receive(ref ep1);
            string receiveContent = Encoding.ASCII.GetString(dgram);
          
        }
    }
   
}

