using System;
using System.Collections.Generic;
using System.Text;

namespace LuxaforLyncTool_Light.Device
{
    public enum LightCommand
    {
        Single = 0x00,
        ComplexColor = 0x01,
        Fade = 0x02,
        Strobe = 0x03,
        Wave = 0x04
    }
}
