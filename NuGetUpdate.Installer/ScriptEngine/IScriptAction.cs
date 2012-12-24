using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public interface IScriptAction
    {
        void Visit(IScriptVisitor visitor);
    }
}
