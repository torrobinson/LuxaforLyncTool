using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuxaforLyncTool_Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Rather than actually running a WinForms app,
            using (Process clientProcess = new Process())
            {
                // Start just run a process which displays an icon in the tray,
                clientProcess.Display();
                // ... and listens for Lync events we care about
                clientProcess.Listen();

                Application.Run();
            }
        }
    }
}
