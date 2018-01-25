using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace LuxaforLyncTool_Light.Helpers
{
    public static class ColorHelpers
    {
        public static System.Drawing.Color HexToColor(string hex)
        {
            int argb = Int32.Parse(hex.Replace("#", ""), NumberStyles.HexNumber);
            return System.Drawing.Color.FromArgb(argb);
        }

        public static string ColorToHex(System.Drawing.Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static byte[] ColorToBytes(System.Drawing.Color color, double brightness = 1.0)
        {
            if (brightness != 1.0)
            {
                color = ChangeColorBrightness(color, brightness);
            }
            return new byte[] {color.R, color.G, color.B};
        }

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
