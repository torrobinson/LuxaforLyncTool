using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace LuxaforLyncTool_Light.Helpers
{
    public static class ColorHelpers
    {
        /// <summary>
        /// Converts a hex color code into a Color
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static System.Drawing.Color HexToColor(string hex)
        {
            int argb = Int32.Parse(hex.Replace("#", ""), NumberStyles.HexNumber);
            return System.Drawing.Color.FromArgb(argb);
        }

        /// <summary>
        /// Converts a Color into a hex color string
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorToHex(System.Drawing.Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        /// <summary>
        /// Converts a color into the bytes required for the command buffer, while also applying a brightness value
        /// </summary>
        /// <param name="color"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static byte[] ColorToBytes(System.Drawing.Color color, double brightness = 1.0)
        {
            if (brightness != 1.0)
            {
                color = ChangeColorBrightness(color, brightness);
            }
            return new byte[] {color.R, color.G, color.B};
        }

        /// <summary>
        /// Applies a supplied brightness amount to an existing color
        /// </summary>
        /// <param name="color"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static System.Drawing.Color ChangeColorBrightness(System.Drawing.Color color, double brightness)
        {
            double red = (double)color.R;
            double green = (double)color.G;
            double blue = (double)color.B;

            red *= brightness;
            green *= brightness;
            blue *= brightness;

            return System.Drawing.Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }
    }
}
