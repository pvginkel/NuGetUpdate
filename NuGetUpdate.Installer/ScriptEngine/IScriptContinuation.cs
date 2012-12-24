using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public interface IScriptContinuation : IDisposable
    {
        void Resume();
    }
}
