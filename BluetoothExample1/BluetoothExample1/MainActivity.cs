using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Widget;
using Android.OS;
using Android.Util;
using Java.Util;

namespace BluetoothExample1
{
    [Activity(Label = "BluetoothExample1", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private BluetoothSocket socket;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            var button = FindViewById<Button>(Resource.Id.button1);

            button.Click += Button_Click;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            string name = FindViewById<TextView>(Resource.Id.editText1).Text;
            Task.Run(() =>
            {
                bluetoothHandler(name);
            });
        }

        public void bluetoothHandler(string name)
        {
            if (socket == null && tryGetSocket(name, out socket) == false)
            {
                Log.Info("连接状态", "失败");
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    string msg = "hello";
                    byte[] buffer = Encoding.ASCII.GetBytes(msg);

                    //// Read data from the device
                    //await _socket.InputStream.ReadAsync(buffer, 0, buffer.Length);

                    // Write data to the device
                    socket.OutputStream.Write(buffer, 0, buffer.Length);
                    Log.Info("发送成功", msg);
                }
                catch (Exception e)
                {
                    Log.Info("发送失败", e.Message);
                }

            });

        }

        private static bool tryGetSocket(string name, out BluetoothSocket socket)
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            socket = null;
            if (adapter == null)
            {
                Log.Info("app", "No Bluetooth adapter found.");
                return false;
            }

            if (!adapter.IsEnabled)
            {
                Log.Info("app", "Bluetooth adapter is not enabled.");
                return false;
            }

            BluetoothDevice device = (from bd in adapter.BondedDevices
                where bd.Name == name
                select bd).FirstOrDefault();

            if (device == null)
            {
                Log.Info("app", "Named '" + name + "' device not found.");
                return false;
            }

            try
            {
                // nexus5 蓝牙uudid: 00001132-0000-1000-8000-00805f9b34fb
                socket = device.CreateRfcommSocketToServiceRecord(
                    UUID.FromString("00001132-0000-1000-8000-00805f9b34fb"));
                socket.Connect();
                return true;
            }
            catch (Exception e)
            {
                Log.Info("连接失败", e.Message);
            }
            return false;
        }

        /**
         * 在不确定蓝牙手机uuid 的时候,可以枚举出uuid
         */
        private void tryConnect(string name)
        {

            BluetoothSocket socket = null;
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
            {
                Log.Info("app", "No Bluetooth adapter found.");
                Toast.MakeText(this, "No Bluetooth adapter found.", ToastLength.Short).Show();
                return;
            }

            if (!adapter.IsEnabled)
            {
                Log.Info("app", "Bluetooth adapter is not enabled.");
                Toast.MakeText(this, "Bluetooth adapter is not enabled.", ToastLength.Short).Show();
                return;
            }

            BluetoothDevice device = (from bd in adapter.BondedDevices
                where bd.Name == name
                select bd).FirstOrDefault();

            ParcelUuid[] uuids = null;
            if (device.FetchUuidsWithSdp())
            {
                uuids = device.GetUuids();
            }
            if ((uuids != null) && (uuids.Length > 0))
            {
                foreach (var uuid in uuids)
                {
                    Log.Info("uuid", uuid.Uuid.ToString());
                    try
                    {
                        socket = device.CreateRfcommSocketToServiceRecord(uuid.Uuid);
                        socket.Connect();
                        Log.Info("uuid-success", uuid.Uuid.ToString());
                        RunOnUiThread(() =>
                        {
                            Toast.MakeText(this, "connection success", ToastLength.Short).Show();
                        });
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ex: " + ex.Message);
                    }
                }
            }

            RunOnUiThread(() =>
            {
                Toast.MakeText(this, "链接成功.", ToastLength.Short).Show();
            });
        }

    }
}

