using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public abstract class ScriptRunnerVisitor : AbstractScriptVisitor
    {
        private readonly Dictionary<string, Function> _functions = new Dictionary<string, Function>(StringComparer.OrdinalIgnoreCase);
        private bool _callingFunction;

        public abstract ScriptRunner Runner { get; }

        public sealed override void Assign(Assign action)
        {
            Runner.Variables.AddOrSet(action.Variable, Runner.InvokeExpression(action.Value));
        }

        public sealed override void If(If action)
        {
            if (Runner.InvokeExpression<bool>(action.Condition))
                action.Then.Visit(this);
            else if (action.Else != null)
                action.Else.Visit(this);
        }

        public sealed override void IfElse(IfElse action)
        {
            base.IfElse(action);
        }

        public sealed override void IfThen(IfThen action)
        {
            base.IfThen(action);
        }

        public sealed override void Call(Call action)
        {
            Function function;

            if (!_functions.TryGetValue(action.Name, out function))
                throw new ScriptException(String.Format(UILabels.FunctionNotFound, action.Name));

            _callingFunction = true;

            function.Visit(this);

            _callingFunction = false;
        }

        public sealed override void Function(Function action)
        {
            if (_callingFunction)
                base.Function(action);
            else
                _functions[action.Name] = action;
        }

        public sealed override void ScriptInstall(ScriptInstall action)
        {
            base.ScriptInstall(action);
        }

        public sealed override void ScriptSetup(ScriptSetup action)
        {
            base.ScriptSetup(action);
        }

        public sealed override void ScriptUninstall(ScriptUninstall action)
        {
            base.ScriptUninstall(action);
        }

        public sealed override void ScriptUpdate(ScriptUpdate action)
        {
            base.ScriptUpdate(action);
        }

        public abstract override void ControlCheckBox(ControlCheckBox action);

        public abstract override void ControlLabel(ControlLabel action);

        public abstract override void ControlLink(ControlLink action);

        public abstract override void ExecShell(ExecShell action);

        public abstract override void ExecWait(ExecWait action);

        public abstract override void Message(Message action);

        public abstract override void MessageBox(MessageBox action);

        public abstract override void PageInstallDestinationFolder(PageInstallDestinationFolder action);

        public abstract override void PageInstallLicense(PageInstallLicense action);

        public abstract override void PageInstallStartMenu(PageInstallStartMenu action);

        public abstract override void PageInstallWelcome(PageInstallWelcome action);

        public abstract override void PageUninstallWelcome(PageUninstallWelcome action);

        public abstract override void PageUpdateWelcome(PageUpdateWelcome action);

        public abstract override void CreateDirectory(CreateDirectory action);

        public abstract override void CreateShortcut(CreateShortcut action);

        public abstract override void InstallPackage(InstallPackage action);

        public abstract override void UninstallPackage(UninstallPackage action);
    }
}
