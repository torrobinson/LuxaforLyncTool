using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxaforLyncTool_Client
{
    public class Settings
    {
        public double Brightness { get; set; } = 1.0;
        public int ConnectionFailureRetryMilliseconds { get; set; } = 10000;
    }
}
