using System;
using System.Collections.Generic;
using System.Text;

namespace NuGetUpdate.Shared
{
    public class DownloadCompletedEventArgs
    {
        public string DownloadFolder { get; private set; }

        public DownloadCompletedEventArgs(string downloadFolder)
        {
            if (downloadFolder == null)
                throw new ArgumentNullException("downloadFolder");

            DownloadFolder = downloadFolder;
        }
    }

    public delegate void DownloadCompletedEventHandler(object sender, DownloadCompletedEventArgs e);
}
