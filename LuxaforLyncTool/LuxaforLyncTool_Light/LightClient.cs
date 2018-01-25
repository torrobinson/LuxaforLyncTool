using System;
using System.Drawing;
using System.Globalization;
using LuxaforLyncTool_Light.Device;
using UsbHid;
using UsbHid.USB.Classes.Messaging;

namespace LuxaforLyncTool_Light
{
    public class LightClient
    {
        // Device information
        public const int VendorId = 0x04D8;
        public const int ProductId = 0xF372;
        public ConnectionStatus Status;

        // Our paired device
        public static UsbHidDevice Device;

        public LightClient()
        {
            // Instantiate new device
            Device = new UsbHidDevice(VendorId, ProductId);

            // Bind connection states
            Device.OnConnected += DeviceOnConnected;
            Device.OnDisconnected += DeviceOnDisConnected;

            // Attempt connection
            this.Status = ConnectionStatus.Pending;
            Device.Connect();
        }

        /// <summary>
        /// Executed when the device is diconnected from the host
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceOnDisConnected(object sender, EventArgs e)
        {
            this.Status = ConnectionStatus.Disconnected;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executed when the device is connected to the host
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceOnConnected(object sender, EventArgs e)
        {
            this.Status = ConnectionStatus.Connected;
        }

        #region HID Communication

        #region Sending Commands
        private void SendByte(byte singleByte)
        {;
            SendBytes(new byte[] { singleByte });
        }

        private void SendBytes(byte[] byteArray)
        {
            if (this.Status == ConnectionStatus.Disconnected) return;

            byte CommandDeskTime = 10;
            var command = new CommandMessage(Device.InputReportSize, CommandDeskTime, byteArray);
            Device.SendMessage(command);
        }
        #endregion

        #region Color Commands
        public void SendColor(char simpleColorCode)
        {
            SendByte((byte)simpleColorCode);
        }
        public static void SendColor(string hex)
        {
            int argb = Int32.Parse(hex.Replace("#", ""), NumberStyles.HexNumber);
            throw new NotImplementedException();
        }
        #endregion

        #endregion
    }
}
