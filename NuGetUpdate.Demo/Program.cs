using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace NuGetUpdate.Demo
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

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            Application.Run(new MainForm());
        }
    }
}
