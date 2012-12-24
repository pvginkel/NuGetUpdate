using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Installer.ScriptEngine;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.Pages
{
    public partial class ProgressPage : PageControl, IWaitablePage
    {
        private bool _initialized;

        public ProgressPage(ScriptRunner runner, IScriptContinuation continuation)
        {
            if (runner == null)
                throw new ArgumentNullException("runner");

            InitializeComponent();

            switch (runner.Mode)
            {
                case ScriptRunnerMode.Install:
                    _header.Text = UILabels.Installing;
                    _header.SubText = String.Format(UILabels.InstallingSubText, runner.Environment.Config.SetupTitle);
                    break;

                case ScriptRunnerMode.Update:
                    _header.Text = UILabels.Updating;
                    _header.SubText = String.Format(UILabels.UpdatingSubText, runner.Environment.Config.SetupTitle);
                    break;

                case ScriptRunnerMode.Uninstall:
                    _header.Text = UILabels.Uninstalling;
                    _header.SubText = String.Format(UILabels.UninstallingSubText, runner.Environment.Config.SetupTitle);
                    break;
            }

            _progressListBox.Visible = false;

            continuation.Resume();
        }

        private void ProgressPage_ParentChanged(object sender, EventArgs e)
        {
            if (_initialized)
                return;

            _initialized = true;

            var form = (MainForm)FindForm();

            form.CloseButtonEnabled = false;
            form.ProgressChanged += form_ProgressChanged;
        }

        void form_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PerformProgressChanged(e.Message, e.Progress);
        }

        private void PerformProgressChanged(string message, double? progress)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => PerformProgressChanged(message, progress)));
                return;
            }

            _progressLabel.Text = message;
            _progressListBox.Items.Add(message);

            if (progress.HasValue)
                _progressBar.Value = (int)(_progressBar.Maximum * progress.Value);

            int visibleItems = _progressListBox.ClientSize.Height / _progressListBox.ItemHeight;

            _progressListBox.TopIndex = Math.Max(_progressListBox.Items.Count - visibleItems + 1, 0);
        }

        private void _showDetails_Click(object sender, EventArgs e)
        {
            _showDetails.Visible = false;
            _progressListBox.Visible = true;
        }

        public void WaitForClose(IScriptContinuation continuation)
        {
            var form = (MainForm)FindForm();

            form.CloseButtonEnabled = true;
            form.ProgressChanged -= form_ProgressChanged;

            continuation.Resume();
        }
    }
}
