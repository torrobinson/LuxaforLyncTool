using System;
using System.Drawing;
using System.Globalization;
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
        public static UsbHidDevice Device;

        // Out brightness setting
        public double Brightness = 1.0;

        /// <summary>
        /// Constructor
        /// </summary>
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

        #region Communication Commands
        private void SendBytes(byte commandByte, byte[] byteArray)
        {
            // Cancel if not connected
            if (this.Status == ConnectionStatus.Disconnected) return;

            // Otherwise, send a new command and command buffer
            var command = new CommandMessage(Device.InputReportSize, commandByte, byteArray);
            Device.SendMessage(command);
        }
        #endregion

        #region Color Commands
        /// <summary>
        /// Send a pulse request to the light. The light returns the color it started at when the pulse is complete
        /// </summary>
        /// <param name="color">The color the light should pulse</param>
        /// <param name="speed">How fast the light should pulse</param>
        /// <param name="pulseCount">How many times the light should pulse</param>
        /// <param name="leds">Which LEDs should pulse</param>
        public void SendPulseColor(System.Drawing.Color color, Speed speed, byte pulseCount, LED leds = LED.All)
        {
            byte[] colorBytes = ColorHelpers.ColorToBytes(color, this.Brightness);
            // TODO: actually flash and return to previous color
            SendBytes((byte)LightCommand.Strobe, new byte[]
            {
                (byte)leds,
                colorBytes[0],
                colorBytes[1],
                colorBytes[2],
                (byte)speed,
                new Byte(),
                pulseCount 
            });
        }

        /// <summary>
        /// Send a request to permanently change into a specified color
        /// </summary>
        /// <param name="color">The color the light should become</param>
        /// <param name="led">The LEDs that should change color</param>
        public void SendColor(System.Drawing.Color color, LED led = LED.All)
        {
            byte[] colorBytes = ColorHelpers.ColorToBytes(color, this.Brightness);
            // TODO: actually flash and return to previous color
            SendBytes((byte)LightCommand.ComplexColor, new byte[]
            {
                (byte)led,
                colorBytes[0],
                colorBytes[1],
                colorBytes[2],
            });
        }

        #endregion

        #endregion
    }
}
