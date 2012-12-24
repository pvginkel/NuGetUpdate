using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using NuGetUpdate.Installer.InstallLogging;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer
{
    public class UninstallManager
    {
        public const int RetryCount = 5;
        public static readonly TimeSpan RetryTimeout = TimeSpan.FromSeconds(1);

        private static readonly Dictionary<Type, Action<UninstallManager, InstallLogEntry>> _handlers = new Dictionary<Type, Action<UninstallManager, InstallLogEntry>>
        {
            { typeof(InstallLogCreateDirectory), (s, entry) => s.ProcessCreateDirectory((InstallLogCreateDirectory)entry) },
            { typeof(InstallLogCreateFile), (s, entry) => s.ProcessCreateFile((InstallLogCreateFile)entry) }
        };

        private readonly InstallLogEntry[] _entries;

        public event ProgressChangedEventHandler ProgressChanged;

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            var ev = ProgressChanged;
            if (ev != null)
                ev(this, e);
        }

        public UninstallManager(IEnumerable<InstallLogEntry> entries)
        {
            if (entries == null)
                throw new ArgumentNullException("entries");

            _entries = new List<InstallLogEntry>(entries).ToArray();
        }

        public void Execute()
        {
            for (int i = _entries.Length - 1; i >= 0; i--)
            {
                _handlers[_entries[i].GetType()](this, _entries[i]);
            }
        }

        private void ProcessCreateDirectory(InstallLogCreateDirectory entry)
        {
            if (!Directory.Exists(entry.Path))
                return;

            for (int i = 0; i < RetryCount; i++)
            {
                if (i == 0)
                {
                    OnProgressChanged(new ProgressChangedEventArgs(
                        String.Format(UILabels.DeletingDirectory, entry.Path)
                    ));
                }
                else
                {
                    OnProgressChanged(new ProgressChangedEventArgs(
                        String.Format(UILabels.RetryingDeletingDirectory, entry.Path)
                    ));
                }

                try
                {
                    if (entry.Force)
                    {
                        Directory.Delete(entry.Path, true);
                    }
                    else
                    {
                        bool empty =
                            Directory.GetFiles(entry.Path).Length == 0 &&
                            Directory.GetDirectories(entry.Path).Length == 0;

                        if (empty)
                            Directory.Delete(entry.Path);
                    }

                    return;
                }
                catch
                {
                    Thread.Sleep(RetryTimeout);
                }
            }
        }

        private void ProcessCreateFile(InstallLogCreateFile entry)
        {
            if (!File.Exists(entry.Path))
                return;

            for (int i = 0; i < RetryCount; i++)
            {
                if (i == 0)
                {
                    OnProgressChanged(new ProgressChangedEventArgs(
                        String.Format(UILabels.DeletingFile, entry.Path)
                    ));
                }
                else
                {
                    OnProgressChanged(new ProgressChangedEventArgs(
                        String.Format(UILabels.RetryingDeleteFile, entry.Path)
                    ));
                }

                try
                {
                    File.Delete(entry.Path);

                    return;
                }
                catch
                {
                    Thread.Sleep(RetryTimeout);
                }
            }
        }
    }
}
