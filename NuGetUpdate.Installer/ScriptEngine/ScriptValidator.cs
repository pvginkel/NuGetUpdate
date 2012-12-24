using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.ScriptEngine
{
    internal class ScriptValidator : AbstractScriptVisitor
    {
        private readonly ScriptValidatorMode _mode;

        private static readonly Type[] _generalActions = new[]
        {
            typeof(Assign),
            typeof(ExecShell),
            typeof(ExecWait),
            typeof(If),
            typeof(Message),
            typeof(MessageBox)
        };

        private static readonly Type[] _controlActions = new[]
        {
            typeof(ControlCheckBox),
            typeof(ControlLabel),
            typeof(ControlLink)
        };

        private static readonly Type[] _installActions = new[]
        {
            typeof(PageInstallDestinationFolder),
            typeof(PageInstallFinish),
            typeof(PageInstallLicense),
            typeof(PageInstallProgress),
            typeof(PageInstallStartMenu),
            typeof(PageInstallWelcome)
        };

        private static readonly Type[] _uninstallActions = new[]
        {
            typeof(PageUninstallFinish),
            typeof(PageUninstallProgress),
            typeof(PageUninstallWelcome)
        };

        private static readonly Type[] _updateActions = new[]
        {
            typeof(PageUpdateFinish),
            typeof(PageUpdateProgress),
            typeof(PageUpdateWelcome)
        };

        private bool _inPage;
        private bool _allowControls;
        private bool _inFunction;

        public bool HadProgressPage { get; private set; }

        public ScriptValidator(ScriptValidatorMode mode)
        {
            _mode = mode;
        }

        public override void Assign(Assign action)
        {
            VerifyAction(typeof(Assign));

            base.Assign(action);
        }

        public override void ControlCheckBox(ControlCheckBox action)
        {
            VerifyAction(typeof(ControlCheckBox));

            base.ControlCheckBox(action);
        }

        public override void ControlLabel(ControlLabel action)
        {
            VerifyAction(typeof(ControlLabel));

            base.ControlLabel(action);
        }

        public override void ControlLink(ControlLink action)
        {
            VerifyAction(typeof(ControlLink));

            base.ControlLink(action);
        }

        public override void ExecShell(ExecShell action)
        {
            VerifyAction(typeof(ExecShell));

            base.ExecShell(action);
        }

        public override void ExecWait(ExecWait action)
        {
            VerifyAction(typeof(ExecWait));

            base.ExecWait(action);
        }

        public override void If(If action)
        {
            VerifyAction(typeof(If));

            base.If(action);
        }

        public override void Message(Message action)
        {
            VerifyAction(typeof(Message));

            base.Message(action);
        }

        public override void MessageBox(MessageBox action)
        {
            VerifyAction(typeof(MessageBox));

            base.MessageBox(action);
        }

        public override void PageInstallDestinationFolder(PageInstallDestinationFolder action)
        {
            VerifyAction(typeof(PageInstallDestinationFolder));

            EnterPage(false);

            base.PageInstallDestinationFolder(action);

            ExitPage();
        }

        public override void PageInstallFinish(PageInstallFinish action)
        {
            VerifyAction(typeof(PageInstallFinish));

            EnterPage(true);

            base.PageInstallFinish(action);

            ExitPage();
        }

        public override void PageInstallLicense(PageInstallLicense action)
        {
            VerifyAction(typeof(PageInstallLicense));

            EnterPage(false);

            base.PageInstallLicense(action);

            ExitPage();
        }

        public override void PageInstallProgress(PageInstallProgress action)
        {
            VerifyAction(typeof(PageInstallProgress));

            HadProgressPage = true;

            EnterPage(false);

            base.PageInstallProgress(action);

            ExitPage();
        }

        public override void PageInstallStartMenu(PageInstallStartMenu action)
        {
            VerifyAction(typeof(PageInstallStartMenu));

            EnterPage(false);

            base.PageInstallStartMenu(action);

            ExitPage();
        }

        public override void PageInstallWelcome(PageInstallWelcome action)
        {
            VerifyAction(typeof(PageInstallWelcome));

            EnterPage(false);

            base.PageInstallWelcome(action);

            ExitPage();
        }

        public override void PageUninstallFinish(PageUninstallFinish action)
        {
            VerifyAction(typeof(PageUninstallFinish));

            EnterPage(true);

            base.PageUninstallFinish(action);

            ExitPage();
        }

        public override void PageUninstallProgress(PageUninstallProgress action)
        {
            VerifyAction(typeof(PageUninstallProgress));

            HadProgressPage = true;

            EnterPage(false);

            base.PageUninstallProgress(action);

            ExitPage();
        }

        public override void PageUninstallWelcome(PageUninstallWelcome action)
        {
            VerifyAction(typeof(PageUninstallWelcome));

            EnterPage(false);

            base.PageUninstallWelcome(action);

            ExitPage();
        }

        public override void PageUpdateFinish(PageUpdateFinish action)
        {
            VerifyAction(typeof(PageUpdateFinish));

            EnterPage(true);

            base.PageUpdateFinish(action);

            ExitPage();
        }

        public override void PageUpdateProgress(PageUpdateProgress action)
        {
            VerifyAction(typeof(PageUpdateProgress));

            HadProgressPage = true;

            EnterPage(false);

            base.PageUpdateProgress(action);

            ExitPage();
        }

        public override void PageUpdateWelcome(PageUpdateWelcome action)
        {
            VerifyAction(typeof(PageUpdateWelcome));

            EnterPage(false);

            base.PageUpdateWelcome(action);

            ExitPage();
        }

        public override void Function(Function action)
        {
            if (_inFunction)
                throw new ScriptException(UILabels.FunctionsCannotBeNested);

            _inFunction = true;

            base.Function(action);

            _inFunction = false;
        }

        private void EnterPage(bool allowControls)
        {
            if (_inPage)
                throw new ScriptException(UILabels.PagesCannotBeNested);

            _inPage = true;
            _allowControls = allowControls;
        }

        private void ExitPage()
        {
            _inPage = false;
            _allowControls = false;
        }

        private void VerifyAction(Type type)
        {
            if (HasAction(_generalActions, type))
                return;

            if (HasAction(_controlActions, type))
            {
                if (!_allowControls)
                    throw new ScriptException(UILabels.ControlsNotAllowed);

                return;
            }

            Type[] actions = null;

            switch (_mode)
            {
                case ScriptValidatorMode.Install: actions = _installActions; break;
                case ScriptValidatorMode.Uninstall: actions = _uninstallActions; break;
                case ScriptValidatorMode.Update: actions = _updateActions; break;
            }

            if (actions == null || !HasAction(actions, type))
                throw new ScriptException(String.Format(UILabels.ActionNotAllow, type.Name));
        }

        private bool HasAction(Type[] actions, Type type)
        {
            foreach (var action in actions)
            {
                if (action == type)
                    return true;
            }

            return false;
        }
    }
}
