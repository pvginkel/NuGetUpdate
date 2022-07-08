using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace NuGetUpdate.Bootstrapper
{
    internal static class Program
    {
        public static Arguments Arguments { get; private set; }
        public static string[] ExtraArguments { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            try
            {
                try
                {
                    ExtraArguments = Arguments.ExtractExtra(ref args);

                    Arguments = new Arguments();
                    Arguments.Parse(args);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        UILabels.NuGetSetup,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    return;
                }

                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    UILabels.UnexpectedFailure + Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    UILabels.NuGetSetup,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
