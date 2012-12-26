using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Installer.InstallLogging;
using NuGetUpdate.Installer.Pages;
using NuGetUpdate.Installer.ScriptEngine;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer
{
    public partial class MainForm : Shared.Form
    {
        private ScriptRunner _runner;
        private readonly Visitor _visitor;

        public event ProgressChangedEventHandler ProgressChanged;

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            var ev = ProgressChanged;
            if (ev != null)
                ev(this, e);
        }

        public MainForm()
        {
            InitializeComponent();

            Disposed += MainForm_Disposed;

            // Load package metadata from the correct location.

            string setupTitle = Program.Arguments.Title;
            string installedVersion = null;
            InstallLogEntry[] installLogEntries = null;
            bool isInstalled;

            string sourcePath = Environment.CurrentDirectory;
            
            using (var metadata = Metadata.Open(Program.Arguments.Package, !Program.Arguments.Install))
            {
                isInstalled = metadata != null;

                if (isInstalled)
                {
                    setupTitle = metadata.SetupTitle;
                    installedVersion = metadata.InstalledVersion;

                    var installLog = metadata.InstallLog;

                    if (installLog != null)
                        installLogEntries = installLog.Items;

                    if (Program.Arguments.Uninstall)
                        sourcePath = metadata.InstallPath;
                }
            }

            Text = String.Format(Text, setupTitle);

            // Shortcut when downloading a new update. This is the bootstrapper
            // implementation integrated into the installer.

            if (Program.Arguments.DownloadUpdate)
            {
                ShowPage<DownloadUpdatePage>();
                return;
            }

            // Resolve the path to the script file name and validate the
            // package if we're installing or updating.

            string scriptFileName;

            if (!Program.Arguments.Uninstall)
            {
                Util.ValidateDownloadFolder(sourcePath);

                scriptFileName = Path.Combine(
                    Path.Combine(
                        sourcePath,
                        Constants.ToolsFolder
                    ),
                    Constants.ScriptFileName
                );
            }
            else
            {
                scriptFileName = Path.Combine(
                    Path.Combine(
                        sourcePath,
                        Constants.BinFolder
                    ),
                    Constants.ScriptFileName
                );
            }

            if (!File.Exists(scriptFileName))
                throw new NuGetUpdateException(UILabels.CannotFindScript);

            // Setup the script.

            ScriptRunnerMode mode;

            if (Program.Arguments.Update || (Program.Arguments.Install && isInstalled))
                mode = ScriptRunnerMode.Update;
            else if (Program.Arguments.Install)
                mode = ScriptRunnerMode.Install;
            else
                mode = ScriptRunnerMode.Uninstall;

            ScriptConfig config;

            if (Program.Arguments.Uninstall)
            {
                config = new ScriptConfig(
                    null,
                    Program.Arguments.Package,
                    setupTitle,
                    null,
                    installedVersion,
                    Escaping.ShellEncode(Program.ExtraArguments)
                );
            }
            else
            {
                config = ScriptLoader.LoadConfigFromNuspec(
                    sourcePath,
                    setupTitle,
                    installedVersion,
                    Escaping.ShellEncode(Program.ExtraArguments)
                );
            }

            var environment = new ScriptEnvironment(this, config);

            _visitor = new Visitor(this);

            if (installLogEntries != null)
                _visitor.InstallLog.AddRange(installLogEntries);

            _runner = new ScriptRunner(environment, _visitor, mode, scriptFileName);

            _runner.Completed += _runner_Completed;
            _runner.UnhandledException += _runner_UnhandledException;

            _runner.Execute();
        }

        void _runner_UnhandledException(object sender, ScriptExceptionEventArgs e)
        {
            // If anything goes wrong at any time during the update, we set
            // attempted version. This isn't set on Cancel, so the user can
            // still retry if he cancels.

            using (var metadata = Metadata.Open(Program.Arguments.Package, false, true))
            {
                if (metadata != null)
                    metadata.AttemptedVersion = _runner.Environment.Config.PackageVersion;
            }

            ShowException(e.Exception);
        }

        public void ShowException(Exception exception)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowException(exception)));
                return;
            }

            System.Windows.Forms.MessageBox.Show(
                this,
                UILabels.UnexpectedFailure + Environment.NewLine +
                Environment.NewLine +
                exception.Message,
                UILabels.NuGetSetup,
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error
            );
        }

        void _runner_Completed(object sender, EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Action(Dispose));
            else
                Dispose();
        }

        void MainForm_Disposed(object sender, EventArgs e)
        {
            if (_runner != null)
            {
                _runner.Dispose();
                _runner = null;
            }
        }

        private void ShowPage<T>(params object[] args)
            where T : PageControl
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowPage<T>(args)));
                return;
            }

            var page = (PageControl)Activator.CreateInstance(typeof(T), args);

            FixFonts(page);

            page.Dock = DockStyle.Fill;

            var currentControls = new ArrayList(Controls);

            Controls.Add(page);

            foreach (Control control in currentControls)
            {
                Controls.Remove(control);

                control.Dispose();
            }

            AcceptButton = page.AcceptButton;
            CancelButton = page.CancelButton;

            if (IsHandleCreated)
                page.SelectNextControl(page, true, true, true, false);
        }

        private void WaitForPageClose(IScriptContinuation continuation)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => WaitForPageClose(continuation)));
                return;
            }

            IWaitablePage waitablePage = null;

            if (Controls.Count > 0)
                waitablePage = Controls[0] as IWaitablePage;

            if (waitablePage == null)
                throw new NuGetUpdateException(UILabels.PageNotWaitable);

            waitablePage.WaitForClose(continuation);
        }

        private void AddControlToPage(IScriptAction action)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddControlToPage(action)));
                return;
            }

            IControlHostPage controlHostPage = null;

            if (Controls.Count > 0)
                controlHostPage = Controls[0] as IControlHostPage;

            if (controlHostPage == null)
                throw new NuGetUpdateException(UILabels.PageNotControlHost);

            controlHostPage.AddControl(action);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            NativeMethods.SetForegroundWindow(Handle);

            if (Controls.Count > 0)
                Controls[0].SelectNextControl(Controls[0], true, true, true, false);
        }

        private void RaiseProgressChanged(string message)
        {
            RaiseProgressChanged(message, null);
        }

        private void RaiseProgressChanged(string message, double? progress)
        {
            OnProgressChanged(new ProgressChangedEventArgs(message, progress));
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;

            var result = System.Windows.Forms.MessageBox.Show(
                this,
                String.Format(UILabels.AreYouSureQuit, _runner.Environment.Config.SetupTitle),
                Text,
                System.Windows.Forms.MessageBoxButtons.YesNo,
                System.Windows.Forms.MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                e.Cancel = true;
        }

        private bool RequestApplicationClose()
        {
            if (InvokeRequired)
                return (bool)Invoke(new Func<bool>(RequestApplicationClose));

            var result = System.Windows.Forms.MessageBox.Show(
                this,
                String.Format(UILabels.ApplicationStillRunning, _runner.Environment.Config.SetupTitle),
                Text,
                System.Windows.Forms.MessageBoxButtons.RetryCancel,
                System.Windows.Forms.MessageBoxIcon.Error
            );

            return result == DialogResult.Retry;
        }
    }
}
