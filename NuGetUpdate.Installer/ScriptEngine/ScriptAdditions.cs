using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.ScriptEngine
{
    abstract partial class ContainerType : IScriptAction
    {
        public abstract void Visit(IScriptVisitor visitor);
    }

    partial class Assign : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.Assign(this);
        }
	}

    partial class Call : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.Call(this);
        }
	}

    partial class ControlCheckBox
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.ControlCheckBox(this);
        }
	}

    partial class ControlLabel : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.ControlLabel(this);
        }
	}

    partial class ControlLink
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.ControlLink(this);
        }
	}

    partial class CreateDirectory : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.CreateDirectory(this);
        }
	}

    partial class CreateShortcut : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.CreateShortcut(this);
        }
	}

    partial class ExecShell : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.ExecShell(this);
        }
	}

    partial class ExecWait : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.ExecWait(this);
        }
	}

    partial class Function
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.Function(this);
        }
	}

    partial class If : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.If(this);
        }
	}

    partial class IfElse
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.IfElse(this);
        }
	}

    partial class IfThen
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.IfThen(this);
        }
	}

    partial class InstallPackage : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.InstallPackage(this);
        }
	}

    partial class Message : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.Message(this);
        }
	}

    partial class MessageBox : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.MessageBox(this);
        }
	}

    partial class PageInstallDestinationFolder : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.PageInstallDestinationFolder(this);
        }
	}

    partial class PageInstallFinish
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.PageInstallFinish(this);
        }
	}

    partial class PageInstallLicense : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.PageInstallLicense(this);
        }
	}

    partial class PageInstallProgress
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.PageInstallProgress(this);
        }
	}

    partial class PageInstallStartMenu : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.PageInstallStartMenu(this);
        }
	}

    partial class PageInstallWelcome : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.PageInstallWelcome(this);
        }
	}

    partial class PageUninstallFinish
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.PageUninstallFinish(this);
        }
	}

    partial class PageUninstallProgress
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.PageUninstallProgress(this);
        }
	}

    partial class PageUninstallWelcome : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.PageUninstallWelcome(this);
        }
	}

    partial class PageUpdateFinish
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.PageUpdateFinish(this);
        }
	}

    partial class PageUpdateProgress
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.PageUpdateProgress(this);
        }
	}

    partial class PageUpdateWelcome : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.PageUpdateWelcome(this);
        }
	}

    partial class ScriptInstall
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.ScriptInstall(this);
        }
	}

    partial class ScriptSetup
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.ScriptSetup(this);
        }
	}

    partial class ScriptUninstall
	{
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.ScriptUninstall(this);
        }
    }

    partial class ScriptUpdate
    {
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.ScriptUpdate(this);
        }
    }

    partial class ScriptSilentUpdate
    {
        public override void Visit(IScriptVisitor visitor)
        {
            visitor.ScriptSilentUpdate(this);
        }
    }

    partial class UninstallPackage : IScriptAction
	{
        public void Visit(IScriptVisitor visitor)
        {
            visitor.UninstallPackage(this);
        }
	}

    public interface IScriptVisitor
    {
		void Assign(Assign action);
		void Call(Call action);
		void ControlCheckBox(ControlCheckBox action);
		void ControlLabel(ControlLabel action);
		void ControlLink(ControlLink action);
		void CreateDirectory(CreateDirectory action);
		void CreateShortcut(CreateShortcut action);
		void ExecShell(ExecShell action);
		void ExecWait(ExecWait action);
		void Function(Function action);
		void If(If action);
		void IfElse(IfElse action);
		void IfThen(IfThen action);
		void InstallPackage(InstallPackage action);
		void Message(Message action);
		void MessageBox(MessageBox action);
		void PageInstallDestinationFolder(PageInstallDestinationFolder action);
		void PageInstallFinish(PageInstallFinish action);
		void PageInstallLicense(PageInstallLicense action);
		void PageInstallProgress(PageInstallProgress action);
		void PageInstallStartMenu(PageInstallStartMenu action);
		void PageInstallWelcome(PageInstallWelcome action);
		void PageUninstallFinish(PageUninstallFinish action);
		void PageUninstallProgress(PageUninstallProgress action);
		void PageUninstallWelcome(PageUninstallWelcome action);
		void PageUpdateFinish(PageUpdateFinish action);
		void PageUpdateProgress(PageUpdateProgress action);
		void PageUpdateWelcome(PageUpdateWelcome action);
		void ScriptInstall(ScriptInstall action);
		void ScriptSetup(ScriptSetup action);
		void ScriptUninstall(ScriptUninstall action);
		void ScriptUpdate(ScriptUpdate action);
		void ScriptSilentUpdate(ScriptSilentUpdate action);
		void UninstallPackage(UninstallPackage action);
    }

	public abstract class AbstractScriptVisitor : IScriptVisitor
	{
        public virtual void Assign(Assign action)
        {
        }

        public virtual void Call(Call action)
        {
        }

        public virtual void ControlCheckBox(ControlCheckBox action)
        {
            VisitChildren(action);
        }

        public virtual void ControlLabel(ControlLabel action)
        {
        }

        public virtual void ControlLink(ControlLink action)
        {
            VisitChildren(action);
        }

        public virtual void CreateDirectory(CreateDirectory action)
        {
        }

        public virtual void CreateShortcut(CreateShortcut action)
        {
        }

        public virtual void ExecShell(ExecShell action)
        {
        }

        public virtual void ExecWait(ExecWait action)
        {
        }

        public virtual void Function(Function action)
        {
            VisitChildren(action);
        }

        public virtual void If(If action)
        {
            VisitChildren(action.Then);

            if (action.Else != null)
                VisitChildren(action.Else);
        }

        public virtual void IfElse(IfElse action)
        {
            VisitChildren(action);
        }

        public virtual void IfThen(IfThen action)
        {
            VisitChildren(action);
        }

        public virtual void InstallPackage(InstallPackage action)
        {
        }

        public virtual void Message(Message action)
        {
        }

        public virtual void MessageBox(MessageBox action)
        {
        }

        public virtual void PageInstallDestinationFolder(PageInstallDestinationFolder action)
        {
        }

        public virtual void PageInstallFinish(PageInstallFinish action)
        {
            VisitChildren(action);
        }

        public virtual void PageInstallLicense(PageInstallLicense action)
        {
        }

        public virtual void PageInstallProgress(PageInstallProgress action)
        {
            VisitChildren(action);
        }

        public virtual void PageInstallStartMenu(PageInstallStartMenu action)
        {
        }

        public virtual void PageInstallWelcome(PageInstallWelcome action)
        {
        }

        public virtual void PageUninstallFinish(PageUninstallFinish action)
        {
            VisitChildren(action);
        }

        public virtual void PageUninstallProgress(PageUninstallProgress action)
        {
            VisitChildren(action);
        }

        public virtual void PageUninstallWelcome(PageUninstallWelcome action)
        {
        }

        public virtual void PageUpdateFinish(PageUpdateFinish action)
        {
            VisitChildren(action);
        }

        public virtual void PageUpdateProgress(PageUpdateProgress action)
        {
            VisitChildren(action);
        }

        public virtual void PageUpdateWelcome(PageUpdateWelcome action)
        {
        }

        public virtual void ScriptInstall(ScriptInstall action)
        {
            VisitChildren(action);
        }

        public virtual void ScriptSetup(ScriptSetup action)
        {
            VisitChildren(action);
        }

        public virtual void ScriptUninstall(ScriptUninstall action)
        {
            VisitChildren(action);
        }

        public virtual void ScriptUpdate(ScriptUpdate action)
        {
            VisitChildren(action);
        }

        public virtual void ScriptSilentUpdate(ScriptSilentUpdate action)
        {
            VisitChildren(action);
        }

        public virtual void UninstallPackage(UninstallPackage action)
        {
        }

        private void VisitChildren(ContainerType action)
        {
            if (action.Items == null)
                return;

            foreach (IScriptAction child in action.Items)
            {
                child.Visit(this);
            }
        }
	}
}
