using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuxaforLyncTool_Client
{
    static class Program
    {
        private static Process clientProcess;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Rather than actually running a WinForms app,
            using (clientProcess = new Process())
            {
                // Start just run a process which displays an icon in the tray,
                clientProcess.DisplayIcon();
                // ... and listens for Lync events we care about
                clientProcess.Listen();

                // On exit, ensure the process disposes properly
                Application.ApplicationExit += OnProcessExit;
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

                // Run the application
                Application.Run();
            }
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            clientProcess.Dispose();
        }

    }
}
