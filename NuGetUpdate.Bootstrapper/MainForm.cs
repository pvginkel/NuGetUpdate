using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Bootstrapper
{
    public partial class MainForm : Shared.Form
    {
        private PackageDownloader _downloader;

        public MainForm()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
            Text = String.Format(Text, Program.Arguments.Title);

            Disposed += MainForm_Disposed;
        }

        void MainForm_Disposed(object sender, EventArgs e)
        {
            if (_downloader != null)
            {
                _downloader.Dispose();
                _downloader = null;
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            NativeMethods.SetForegroundWindow(Handle);

            _downloader = new PackageDownloader(Program.Arguments.Site, Program.Arguments.Package);

            _downloader.DownloadCompleted += (s, ea) => DownloadComplete(ea.DownloadFolder);
            _downloader.DownloadFailed += (s, ea) => ShowException(ea.Exception);

            _downloader.Start();

            _timer.Start();
        }

        private void DownloadComplete(string downloadFolder)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => DownloadComplete(downloadFolder)));
                return;
            }

            // Get rid of the main form. We're starting the real update
            // process now and don't want the main form anymore.

            Dispose();

            try
            {
                string nguPath = Path.Combine(
                    Path.Combine(
                        downloadFolder,
                        Constants.ToolsFolder
                    ),
                    Constants.NuGetUpdateFileName
                );

                NativeMethods.AllowSetForegroundWindow(NativeMethods.ASFW_ANY);

                using (var process = Process.Start(new ProcessStartInfo
                {
                    FileName = nguPath,
                    Arguments = String.Format(
                        "-i -p {0} -t {1} -s {2} -- {3}",
                        Escaping.ShellEncode(Program.Arguments.Package),
                        Escaping.ShellEncode(Program.Arguments.Title),
                        Escaping.ShellEncode(Program.Arguments.Site),
                        Escaping.ShellEncode(Program.ExtraArguments)
                    ),
                    UseShellExecute = false,
                    WorkingDirectory = downloadFolder
                }))
                {
                    process.WaitForExit();
                }

                SEH.SinkExceptions(() => Directory.Delete(downloadFolder, true));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    UILabels.StartSetupFailure + Environment.NewLine +
                    Environment.NewLine +
                    ex.Message,
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ShowException(Exception exception)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowException(exception)));
                return;
            }

            MessageBox.Show(
                this,
                UILabels.DownloadFailure + Environment.NewLine +
                Environment.NewLine +
                exception.Message,
                Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            Dispose();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _progressLabel.Text = _downloader.Status;

            double progress = _downloader.Progress;

            if (double.IsNaN(progress))
            {
                _progressBar.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                if (_progressBar.Style != ProgressBarStyle.Continuous)
                    _progressBar.Style = ProgressBarStyle.Continuous;

                _progressBar.Value = (int)(_progressBar.Maximum * progress);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;

            var result = MessageBox.Show(
                this,
                String.Format(UILabels.AreYouSureQuit, Program.Arguments.Title),
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
                _downloader.Dispose();
            else
                e.Cancel = false;
        }
    }
}
