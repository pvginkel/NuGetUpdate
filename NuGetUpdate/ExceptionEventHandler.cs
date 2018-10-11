using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }

    public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);
}
