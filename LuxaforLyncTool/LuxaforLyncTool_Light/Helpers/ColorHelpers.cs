using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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

        public static byte[] ColorToBytes(System.Drawing.Color color)
        {
            return new byte[] {color.R, color.G, color.B};
        }
    }
}
