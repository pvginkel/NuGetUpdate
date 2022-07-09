using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGetUpdate.Installer.ScriptEngine;

namespace NuGetUpdate.Installer
{
    public partial class SilentScriptRunner
    {
        private class Visitor : ScriptExecutionVisitor
        {
            public override ScriptRunner Runner { get; }

            public Visitor(ScriptEnvironment environment, ScriptRunnerMode mode, string scriptFileName)
            {
                Runner = new ScriptRunner(environment, this, mode, scriptFileName);
            }

            public override void ControlCheckBox(ControlCheckBox action) => throw new NotSupportedException();
            public override void ControlLabel(ControlLabel action) => throw new NotSupportedException();
            public override void ControlLink(ControlLink action) => throw new NotSupportedException();
            public override void MessageBox(MessageBox action) => throw new NotSupportedException();
            public override void PageInstallDestinationFolder(PageInstallDestinationFolder action) => throw new NotSupportedException();
            public override void PageInstallLicense(PageInstallLicense action) => throw new NotSupportedException();
            public override void PageInstallStartMenu(PageInstallStartMenu action) => throw new NotSupportedException();
            public override void PageInstallWelcome(PageInstallWelcome action) => throw new NotSupportedException();
            public override void PageUninstallWelcome(PageUninstallWelcome action) => throw new NotSupportedException();
            public override void PageUpdateWelcome(PageUpdateWelcome action) => throw new NotSupportedException();

            protected override void RaiseProgressChanged(string message, double? progress)
            {
                if (progress.HasValue)
                    Trace.WriteLine($"Progress '{message}' {(int)(progress * 100)}%");
                else
                    Trace.WriteLine($"Progress '{message}'");
            }

            protected override bool RequestApplicationClose()
            {
                return true;
            }
        }
    }
}