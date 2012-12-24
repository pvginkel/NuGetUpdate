using System;
using System.Collections.Generic;
using System.Text;
using NuGetUpdate.Installer.ScriptEngine;

namespace NuGetUpdate.Installer.Pages
{
    public interface IWaitablePage
    {
        void WaitForClose(IScriptContinuation continuation);
    }
}
