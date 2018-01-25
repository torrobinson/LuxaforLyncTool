using System;
using System.Drawing;
using System.Globalization;
using LuxaforLyncTool_Light.Color;
using LuxaforLyncTool_Light.Device;
using LuxaforLyncTool_Light.Helpers;
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

        // Out brightness setting
        public double Brightness = 1.0;

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

            // Send the initial Enable command
            SendBytes(10, new byte[] {69});
        }

        /// <summary>1
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
        private void SendSimpleColorByte(byte singleByte)
        {;
            if (this.Status == ConnectionStatus.Disconnected) return;
            var command = new CommandMessage(Device.InputReportSize, 10, new byte[]{ singleByte });
            Device.SendMessage(command);
        }

        private void SendBytes(byte commandByte, byte[] byteArray)
        {
            if (this.Status == ConnectionStatus.Disconnected) return;
            var command = new CommandMessage(Device.InputReportSize, commandByte, byteArray);
            Device.SendMessage(command);
        }
        #endregion

        #region Color Commands
        public void SendSimpleColor(char simpleColorCode)
        {
            SendSimpleColorByte((byte)simpleColorCode);
        }

        public void SendPulseColor(System.Drawing.Color color, byte speed, byte pulseCount, byte leds = 0xFF)
        {
            byte[] colorBytes = ColorHelpers.ColorToBytes(color, this.Brightness);
            // TODO: actually flash and return to previous color
            SendBytes(LightCommands.Strobe, new byte[]
            {
                leds,
                colorBytes[0],
                colorBytes[1],
                colorBytes[2],
                speed,
                new Byte(),
                pulseCount // repeats
            });
        }

        public void SendComplexColor(System.Drawing.Color color, byte led = 0xFF)
        {
            byte[] colorBytes = ColorHelpers.ColorToBytes(color, this.Brightness);
            // TODO: actually flash and return to previous color
            SendBytes(LightCommands.ComplexColor, new byte[]
            {
                led,
                colorBytes[0],
                colorBytes[1],
                colorBytes[2],
            });
        }

        #endregion

        #endregion
    }
}
