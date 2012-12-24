using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public class ScriptExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ScriptExceptionEventArgs(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            Exception = exception;
        }
    }

    public delegate void ScriptExceptionEventHandler(object sender, ScriptExceptionEventArgs e);
}
