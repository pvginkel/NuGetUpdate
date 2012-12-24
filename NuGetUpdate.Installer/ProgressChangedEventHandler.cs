using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Installer
{
    public class ProgressChangedEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public double? Progress { get; private set; }

        public ProgressChangedEventArgs(string message)
            : this(message, null)
        {
        }

        public ProgressChangedEventArgs(string message, double? progress)
        {
            Message = message;
            Progress = progress;
        }
    }

    public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);
}
