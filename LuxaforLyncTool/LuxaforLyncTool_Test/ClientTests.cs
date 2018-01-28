using System;
using LuxaforLyncTool_Client;
using NUnit.Framework;

namespace LuxaforLyncTool_Test
{
    [TestFixture]
    public class ClientTests
    {
        [TestCase] 
        public void DefaultBrightness_IsValid()
        {
            Settings settings = new Settings();
            Assert.IsTrue(settings.Brightness > 0.0 && settings.Brightness <= 1.0);
        }
    }
}
