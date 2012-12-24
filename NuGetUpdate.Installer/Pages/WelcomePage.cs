using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NuGetUpdate.Installer.ScriptEngine;

namespace NuGetUpdate.Installer.Pages
{
    public partial class WelcomePage : PageControl
    {
        private readonly IScriptContinuation _continuation;

        public override IButtonControl AcceptButton
        {
            get { return _acceptButton; }
        }

        public override IButtonControl CancelButton
        {
            get { return _cancelButton; }
        }

        public WelcomePage(ScriptRunner runner, IScriptAction action, IScriptContinuation continuation)
        {
            if (runner == null)
                throw new ArgumentNullException("runner");
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            _continuation = continuation;

            InitializeComponent();

            switch (runner.Mode)
            {
                case ScriptRunnerMode.Install:
                    _headerLabel.Text = String.Format(UILabels.InstallWelcome, runner.Environment.Config.SetupTitle);
                    _wizardLabel.Text = String.Format(UILabels.InstallWelcomeSubText, runner.Environment.Config.SetupTitle);
                    break;

                case ScriptRunnerMode.Update:
                    _headerLabel.Text = String.Format(UILabels.UpdateWelcome, runner.Environment.Config.SetupTitle);
                    _wizardLabel.Text = String.Format(UILabels.UpdateWelcomeSubText, runner.Environment.Config.SetupTitle);
                    break;

                case ScriptRunnerMode.Uninstall:
                    _headerLabel.Text = String.Format(UILabels.UninstallWelcome, runner.Environment.Config.SetupTitle);
                    _wizardLabel.Text = String.Format(UILabels.UninstallWelcomeSubText, runner.Environment.Config.SetupTitle);
                    break;
            }

            bool isLast = false;

            if (action is PageInstallWelcome)
                isLast = ((PageInstallWelcome)action).IsLast;

            PageUtil.UpdateAcceptButton(_acceptButton, isLast);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _continuation.Resume();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            FindForm().Close();
        }
    }
}
