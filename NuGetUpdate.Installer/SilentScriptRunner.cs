using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGetUpdate.Installer.InstallLogging;
using NuGetUpdate.Installer.ScriptEngine;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer
{
    public partial class SilentScriptRunner
    {
        public void Run()
        {
            // Load package metadata from the correct location.

            string setupTitle = Program.Arguments.Title;
            string installedVersion = null;
            InstallLogEntry[] installLogEntries = null;

            string sourcePath = Environment.CurrentDirectory;

            using (var metadata = Metadata.Open(Program.Arguments.Package, !Program.Arguments.Install))
            {
                bool isInstalled = metadata != null;

                if (isInstalled)
                {
                    setupTitle = metadata.SetupTitle;
                    installedVersion = metadata.InstalledVersion;

                    var installLog = metadata.InstallLog;

                    if (installLog != null)
                        installLogEntries = installLog.Items;

                    if (Program.Arguments.Uninstall)
                        sourcePath = metadata.InstallPath;
                }
            }

            // Shortcut when downloading a new update. This is the bootstrapper
            // implementation integrated into the installer.

            if (Program.Arguments.DownloadUpdate)
            {
                Download();
                return;
            }

            Util.ValidateDownloadFolder(sourcePath);

            string scriptFileName = Path.Combine(
                Path.Combine(
                    sourcePath,
                    Constants.ToolsFolder
                ),
                Constants.ScriptFileName
            );

            if (!File.Exists(scriptFileName))
                throw new NuGetUpdateException(UILabels.CannotFindScript);

            // Setup the script.

            var config = ScriptLoader.LoadConfigFromNuspec(
                sourcePath,
                setupTitle,
                installedVersion,
                Escaping.ShellEncode(Program.ExtraArguments)
            );

            var environment = new ScriptEnvironment(config);

            var visitor = new Visitor(environment, ScriptRunnerMode.SilentUpdate, scriptFileName);

            if (installLogEntries != null)
                visitor.InstallLog.AddRange(installLogEntries);

            using (var @event = new ManualResetEvent(false))
            {
                visitor.Runner.UnhandledException += (s, e) => @event.Set();
                visitor.Runner.Completed += (s, e) => @event.Set();

                visitor.Runner.Execute();

                @event.WaitOne();
            }
        }

        private static void Download()
        {
            string downloadFolder = null;

            using (var @event = new ManualResetEvent(false))
            {
                PackageDownloader downloader = null;

                try
                {
                    using (var metadata = Metadata.Open(Program.Arguments.Package))
                    {
                        downloader = new PackageDownloader(metadata.NuGetSite, metadata.NuGetSiteUserName, metadata.NuGetSitePassword, Program.Arguments.Package);
                    }

                    downloader.DownloadCompleted += (s, ea) =>
                    {
                        downloadFolder = ea.DownloadFolder;
                        @event.Set();
                    };

                    downloader.DownloadFailed += (s, ea) => @event.Set();

                    downloader.Start();

                    @event.WaitOne();
                }
                finally
                {
                    downloader?.Dispose();
                }
            }

            if (downloadFolder == null)
                throw new NuGetUpdateException("Failed to download package");

            string nguPath = Path.Combine(
                Path.Combine(
                    downloadFolder,
                    Constants.ToolsFolder
                ),
                Constants.NuGetUpdateFileName
            );

            NativeMethods.AllowSetForegroundWindow(NativeMethods.ASFW_ANY);

            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = nguPath,
                Arguments = String.Format(
                    "-u -l -p {0} -- {1}",
                    Escaping.ShellEncode(Program.Arguments.Package),
                    Escaping.ShellEncode(Program.ExtraArguments)
                ),
                UseShellExecute = false,
                WorkingDirectory = downloadFolder
            }))
            {
                process.WaitForExit();
            }

            SEH.SinkExceptions(() => Directory.Delete(downloadFolder, true));
        }
    }
}
