using System;
using System.Collections.Generic;
using System.Text;

namespace LuxaforLyncTool_Light.Device
{
    public static class LightCommands
    {
        public static readonly byte Single = 00;
        public static readonly byte ComplexColor = 01;
        public static readonly byte Fade = 02;
        public static readonly byte Strobe = 03;
        public static readonly byte Wave = 04;
    }
}
