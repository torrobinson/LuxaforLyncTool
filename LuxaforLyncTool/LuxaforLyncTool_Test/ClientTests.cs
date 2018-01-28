using System;
using LuxaforLyncTool_Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LuxaforLyncTool_Test
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void DefaultBrightness_IsValid()
        {
            Settings settings = new Settings();
            Assert.IsTrue(settings.Brightness > 0.0 && settings.Brightness <= 1.0);
        }
    }
}
