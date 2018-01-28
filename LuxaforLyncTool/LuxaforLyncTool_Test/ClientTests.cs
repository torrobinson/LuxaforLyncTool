using System;
using LuxaforLyncTool_Client;
using Xunit;

namespace LuxaforLyncTool_Test
{
    public class ClientTests
    {
        [Fact] 
        public void DefaultBrightness_IsValid()
        {
            Settings settings = new Settings();
            Assert.True(settings.Brightness > 0.0 && settings.Brightness <= 1.0);
        }
    }
}
