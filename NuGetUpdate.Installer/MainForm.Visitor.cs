using System;
using System.Collections.Generic;
using System.Text;
using NuGetUpdate.Installer.Pages;
using NuGetUpdate.Installer.ScriptEngine;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer
{
    partial class MainForm
    {
        private class Visitor : ScriptExecutionVisitor
        {
            private readonly MainForm _form;

            public override ScriptRunner Runner
            {
                get { return _form._runner; }
            }

            public Visitor(MainForm form)
            {
                _form = form;
            }

            protected override void RaiseProgressChanged(string message, double? progress)
            {
                _form.RaiseProgressChanged(message, progress);
            }

            protected override bool RequestApplicationClose()
            {
                return _form.RequestApplicationClose();
            }

            public override void ControlCheckBox(ControlCheckBox action)
            {
                _form.AddControlToPage(action);
            }

            public override void ControlLabel(ControlLabel action)
            {
                _form.AddControlToPage(action);
            }

            public override void ControlLink(ControlLink action)
            {
                _form.AddControlToPage(action);
            }

            public override void MessageBox(MessageBox action)
            {
                _form.Invoke(new Action(() =>
                {
                    var result = System.Windows.Forms.MessageBox.Show(
                        _form,
                        Runner.ParseTemplate(action.Text),
                        UILabels.NuGetSetup,
                        Enum<System.Windows.Forms.MessageBoxButtons>.Parse(action.Buttons.ToString()),
                        Enum<System.Windows.Forms.MessageBoxIcon>.Parse(action.Icon.ToString())
                    );

                    if (action.Result != null)
                        Runner.Variables.AddOrSet(action.Result, result);
                }));
            }

            public override void PageInstallDestinationFolder(PageInstallDestinationFolder action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<InstallDestinationFolderPage>(Runner, action, continuation);
                }
            }

            public override void PageInstallFinish(PageInstallFinish action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<FinishPage>(Runner, continuation);
                }

                base.PageInstallFinish(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageInstallLicense(PageInstallLicense action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<InstallLicensePage>(Runner, action, continuation);
                }
            }

            public override void PageInstallProgress(PageInstallProgress action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<ProgressPage>(Runner, continuation);
                }

                base.PageInstallProgress(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageInstallStartMenu(PageInstallStartMenu action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<InstallStartMenuPage>(Runner, action, continuation);
                }
            }

            public override void PageInstallWelcome(PageInstallWelcome action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<WelcomePage>(Runner, action, continuation);
                }
            }

            public override void PageUninstallFinish(PageUninstallFinish action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<FinishPage>(Runner, continuation);
                }

                base.PageUninstallFinish(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageUninstallProgress(PageUninstallProgress action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<ProgressPage>(Runner, continuation);
                }

                base.PageUninstallProgress(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageUninstallWelcome(PageUninstallWelcome action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<WelcomePage>(Runner, action, continuation);
                }
            }

            public override void PageUpdateFinish(PageUpdateFinish action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<FinishPage>(Runner, continuation);
                }

                base.PageUpdateFinish(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageUpdateProgress(PageUpdateProgress action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<ProgressPage>(Runner, continuation);
                }

                base.PageUpdateProgress(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageUpdateWelcome(PageUpdateWelcome action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<WelcomePage>(Runner, action, continuation);
                }
            }
        }
    }
}
