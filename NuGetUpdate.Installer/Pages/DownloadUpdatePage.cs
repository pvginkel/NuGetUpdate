using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.Pages
{
    public partial class DownloadUpdatePage : PageControl
    {
        private PackageDownloader _downloader;

        private bool _initialized;

        public DownloadUpdatePage()
        {
            InitializeComponent();

            using (var metadata = Metadata.Open(Program.Arguments.Package))
            {
                _downloader = new PackageDownloader(metadata.NuGetSite, Program.Arguments.Package);
            }

            _downloader.DownloadCompleted += (s, ea) => DownloadComplete(ea.DownloadFolder);
            _downloader.DownloadFailed += (s, ea) => ShowException(ea.Exception);

            Disposed += UpdateDownloadPage_Disposed;
        }

        private void UpdateDownloadPage_ParentChanged(object sender, EventArgs e)
        {
            if (_initialized)
                return;

            _initialized = true;

            _downloader.Start();

            _timer.Start();
        }

        void UpdateDownloadPage_Disposed(object sender, EventArgs e)
        {
            if (_downloader != null)
            {
                _downloader.Dispose();
                _downloader = null;
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            FindForm().Close();
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

            FindForm().Dispose();

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
                        "-u -p {0} -- {1}",
                        Escaping.ShellEncode(Program.Arguments.Package),
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

            // If the download failed and downloader set the package code,
            // this means we were able to correctly download the update, but
            // it wasn't a valid NuGet Update package. Because of this, we
            // set attempted version here to signal we shouldn't retry this
            // update.

            if (_downloader.PackageCode != null)
            {
                using (var metadata = Metadata.Create(Program.Arguments.Package))
                {
                    metadata.AttemptedVersion = _downloader.PackageCode;
                }
            }

            MessageBox.Show(
                this,
                UILabels.DownloadFailure + Environment.NewLine +
                Environment.NewLine +
                exception.Message,
                FindForm().Text,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            FindForm().Dispose();
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
    }
}
