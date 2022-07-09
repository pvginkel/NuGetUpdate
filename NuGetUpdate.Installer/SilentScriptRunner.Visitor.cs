using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            private readonly TextWriter _logger;

            public override ScriptRunner Runner { get; }

            public Visitor(ScriptEnvironment environment, ScriptRunnerMode mode, string scriptFileName, TextWriter logger)
            {
                _logger = logger;

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
                _logger.WriteLine(message);

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