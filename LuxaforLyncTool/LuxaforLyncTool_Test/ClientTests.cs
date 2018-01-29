using System;
using LuxaforLyncTool_Client;
using NUnit.Framework;

namespace LuxaforLyncTool_Test
{
    [TestFixture]
    public class ClientTests
    {
        [Test] 
        public void DefaultBrightness_IsValid()
        {
            Settings settings = new Settings();
            Assert.True(settings.Brightness > 0.0 && settings.Brightness <= 1.0);
        }
    }
}
