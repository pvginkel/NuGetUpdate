using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using NuGetUpdate.Shared;
using ArgumentException = System.ArgumentException;

namespace NuGetUpdate.Installer
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

                if (
                    (Arguments.Uninstall || Arguments.DownloadUpdate) &&
                    !Arguments.Redirected
                )
                    Redirect();
                else
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

        private static void Redirect()
        {
            // Copy the executable to the temp directory.

            string target = Path.Combine(
                Path.GetTempPath(),
                Path.GetFileName(typeof(Program).Assembly.Location)
            );

            if (File.Exists(target))
                File.Delete(target);

            File.Copy(typeof(Program).Assembly.Location, target);

            string flag;

            if (Program.Arguments.Uninstall)
                flag = "-r";
            else if (Program.Arguments.DownloadUpdate)
                flag = "-du";
            else
                throw new NuGetUpdateException(UILabels.ShouldNotRestart);

            NativeMethods.AllowSetForegroundWindow(NativeMethods.ASFW_ANY);

            using (Process.Start(new ProcessStartInfo
            {
                FileName = target,
                Arguments = String.Format(
                    "{0} -x -p {1} -- {2}",
                    flag,
                    Escaping.ShellEncode(Program.Arguments.Package),
                    Escaping.ShellEncode(Program.ExtraArguments)
                ),
                UseShellExecute = false,
                WorkingDirectory = Path.GetTempPath()
            }))
            {
                // We do not wait for the process to finish.
            }
        }
    }
}
