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
    public partial class InstallLicensePage : PageControl
    {
        private readonly ScriptRunner _runner;
        private readonly IScriptContinuation _continuation;

        public override IButtonControl AcceptButton
        {
            get { return _acceptButton; }
        }

        public override IButtonControl CancelButton
        {
            get { return _cancelButton; }
        }

        public InstallLicensePage(ScriptRunner runner, PageInstallLicense action, IScriptContinuation continuation)
        {
            if (runner == null)
                throw new ArgumentNullException("runner");
            if (action == null)
                throw new ArgumentNullException("action");
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            _runner = runner;
            _continuation = continuation;

            InitializeComponent();

            _license.Text = TextUtil.FixNewlines(runner.ParseTemplate(action.Value));
            _header.SubText = String.Format(_header.SubText, runner.Environment.Config.SetupTitle);
            _agree.Text = String.Format(_agree.Text, runner.Environment.Config.SetupTitle);

            _license.Select(0, 0);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _continuation.Resume();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            FindForm().Dispose();
        }
    }
}
