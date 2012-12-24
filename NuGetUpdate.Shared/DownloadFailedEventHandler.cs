using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Shared
{
    public class DownloadFailedEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public DownloadFailedEventArgs(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            Exception = exception;
        }
    }

    public delegate void DownloadFailedEventHandler(object sender, DownloadFailedEventArgs e);
}
