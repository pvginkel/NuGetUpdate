using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuGetUpdate.Installer.InstallLogging;
using NuGetUpdate.Installer.Pages;
using NuGetUpdate.Shared;

namespace NuGetUpdate.Installer.ScriptEngine
{
    public abstract class ScriptExecutionVisitor : ScriptRunnerVisitor
    {
        public List<InstallLogEntry> InstallLog { get; private set; }

        protected ScriptExecutionVisitor()
        {
            InstallLog = new List<InstallLogEntry>();
        }

        private void RaiseProgressChanged(string message)
        {
            RaiseProgressChanged(message, null);
        }

        protected abstract void RaiseProgressChanged(string message, double? progress);

        public override void ExecShell(ExecShell action)
        {
            string fileName = Runner.InvokeExpression<string>(action.FileName);

            RaiseProgressChanged(
                String.Format(UILabels.ExecutingProcess, fileName, fileName)
            );

            string verb = null;
            string arguments = null;

            if (action.Verb != null)
                verb = Runner.InvokeExpression<string>(action.Verb);
            if (action.Arguments != null)
                arguments = Runner.InvokeExpression<string>(action.Arguments);

            using (Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                Verb = verb,
                WorkingDirectory = GetWorkingDirectory(),
                WindowStyle = Enum<System.Diagnostics.ProcessWindowStyle>.Parse(action.WindowStyle.ToString())
            }))
            {
                // We do not wait for this process.
            }
        }

        private string GetWorkingDirectory()
        {
            string targetPath = Runner.Variables.GetOptional<string>(
                Constants.ScriptVariables.TargetPath
            );

            if (
                String.IsNullOrEmpty(targetPath) ||
                !Directory.Exists(targetPath)
            )
                targetPath = Path.GetTempPath();

            return targetPath;
        }

        public override void ExecWait(ExecWait action)
        {
            string fileName = Runner.InvokeExpression<string>(action.FileName);

            string arguments = null;

            if (action.Arguments != null)
                arguments = Runner.InvokeExpression<string>(action.Arguments);

            string progressFileName = fileName;

            if (!String.IsNullOrEmpty(arguments))
                progressFileName += " " + arguments;

            RaiseProgressChanged(
                String.Format(UILabels.ExecutingProcess, progressFileName)
            );

            using (var process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WindowStyle = Enum<System.Diagnostics.ProcessWindowStyle>.Parse(action.WindowStyle.ToString()),
                WorkingDirectory = GetWorkingDirectory()
            }))
            {
                process.WaitForExit();
            }
        }

        public override void Message(Message action)
        {
            RaiseProgressChanged(
                Runner.ParseTemplate(action.Value)
            );
        }

        public override void PageInstallProgress(PageInstallProgress action)
        {
            base.PageInstallProgress(action);

            CommitInstallation();
        }

        public void CommitInstallation()
        {
            // We commit the installation at this point. Things may still
            // happen, but they won't be recorded in the metadata.

            using (var metadata = Metadata.Create(Runner.Environment.Config.PackageCode))
            {
                metadata.InstalledVersion = Runner.Environment.Config.PackageVersion;
                metadata.InstallLog = new InstallLog
                {
                    Items = InstallLog.ToArray()
                };
                if (!String.IsNullOrEmpty(Program.Arguments.Site))
                {
                    metadata.NuGetSite = Program.Arguments.Site;
                    metadata.NuGetSiteUserName = Program.Arguments.SiteUserName;
                    metadata.NuGetSitePassword = Program.Arguments.SitePassword;
                }
                metadata.SetupTitle = Runner.Environment.Config.SetupTitle;
                metadata.InstallPath = Runner.Variables.GetRequired<string>(
                    Constants.ScriptVariables.TargetPath
                );
            }
        }

        public override void PageUninstallProgress(PageUninstallProgress action)
        {
            base.PageUninstallProgress(action);

            // We commit the installation at this point. Things may still
            // happen, but they won't be recorded in the metadata.

            Metadata.Delete(Runner.Environment.Config.PackageCode);
        }

        public override void PageUpdateProgress(PageUpdateProgress action)
        {
            base.PageUpdateProgress(action);

            CommitInstallation();
        }

        public override void CreateDirectory(CreateDirectory action)
        {
            string directory = Runner.InvokeExpression<string>(action.Path);

            RaiseProgressChanged(String.Format(UILabels.CreatingDirectory, directory));

            Directory.CreateDirectory(directory);
        }

        public override void CreateShortcut(CreateShortcut action)
        {
            string shortcutFileName = Runner.InvokeExpression<string>(
                action.ShortcutFileName
            );

            AddToInstallLog(new InstallLogCreateFile
            {
                Path = shortcutFileName
            });

            RaiseProgressChanged(String.Format(UILabels.CreatingShortcut, shortcutFileName));

            var shellLink = new ShellLink
            {
                Target = Runner.InvokeExpression<string>(action.TargetFileName),
            };

            if (action.IconFileName != null)
                shellLink.IconPath = Runner.InvokeExpression<string>(action.IconFileName);

            if (action.IconIndex != null)
                shellLink.IconIndex = Runner.InvokeExpression<int>(action.IconIndex);

            if (action.StartOptions != null)
                shellLink.Arguments = Runner.InvokeExpression<string>(action.StartOptions);

            if (action.Description != null)
                shellLink.Description = Runner.ParseTemplate(action.Description);

            Directory.CreateDirectory(Path.GetDirectoryName(shortcutFileName));

            shellLink.Save(shortcutFileName);
        }

        public override void InstallPackage(InstallPackage action)
        {
            string packageFolder = Runner.Environment.Config.PackageFolder;
            string toolsFolder = Path.Combine(packageFolder, Constants.ToolsFolder);
            string targetPath = Runner.Variables.GetRequired<string>(
                Constants.ScriptVariables.TargetPath
            );

            AddToInstallLog(new InstallLogCreateDirectory
            {
                Path = targetPath,
                Force = false
            });

            if (!action.IntoRoot)
                targetPath = Path.Combine(targetPath, Constants.BinFolder);

            string tempPath = null;

            try
            {
                if (Runner.Mode == ScriptRunnerMode.Update || Runner.Mode == ScriptRunnerMode.SilentUpdate)
                    tempPath = RemoveCurrentTargetPath(targetPath);

                AddToInstallLog(new InstallLogCreateDirectory
                {
                    Path = targetPath,
                    Force = true
                });

                string startMenuPath = Runner.Variables.GetOptional<string>(
                    Constants.ScriptVariables.StartMenuPath
                );

                if (!String.IsNullOrEmpty(startMenuPath))
                {
                    string startMenuBase = NativeMethods.SHGetFolderPath(
                        IntPtr.Zero,
                        NativeMethods.SpecialFolderCSIDL.CSIDL_STARTMENU,
                        IntPtr.Zero,
                        0
                    );

                    startMenuPath = Path.Combine(startMenuBase, startMenuPath);

                    AddToInstallLog(new InstallLogCreateDirectory
                    {
                        Path = startMenuPath,
                        Force = false
                    });
                }

                int fileCount = Directory.GetFiles(toolsFolder, "*", SearchOption.AllDirectories).Length;

                if (fileCount == 0)
                    return;

                int currentFile = 0;

                CopyDirectory(toolsFolder, targetPath, null, action.Overwrite, fileCount, ref currentFile);

                CreateUninstallShortCut(targetPath);
            }
            finally
            {
                if (tempPath != null)
                    SEH.SinkExceptions(() => Directory.Delete(tempPath, true));
            }
        }

        private string RemoveCurrentTargetPath(string targetPath)
        {
            string tempPath;

            for (int i = 0; ; i++)
            {
                tempPath = Path.Combine(
                    Path.GetDirectoryName(targetPath),
                    "tmp~" + i
                );

                if (!Directory.Exists(tempPath))
                    break;
            }

            for (int i = 0; i < UninstallManager.RetryCount; i++)
            {
                if (i == 0)
                {
                    RaiseProgressChanged(
                        String.Format(UILabels.DeletingDirectory, targetPath)
                    );
                }
                else
                {
                    RaiseProgressChanged(
                        String.Format(UILabels.RetryingDeletingDirectory, targetPath)
                    );
                }

                try
                {
                    if (!Directory.Exists(targetPath))
                        return null;

                    Directory.Move(targetPath, tempPath);

                    return tempPath;
                }
                catch
                {
                    Thread.Sleep(UninstallManager.RetryTimeout);
                }
            }

            // If we couldn't move the Bin path automatically, we ask
            // the user what to do. Maybe he/she should close the application.

            while (true)
            {
                if (!RequestApplicationClose())
                    throw new AbortedException();

                RaiseProgressChanged(
                    String.Format(UILabels.RetryingDeletingDirectory, targetPath)
                );

                try
                {
                    Directory.Move(targetPath, tempPath);

                    return tempPath;
                }
                catch
                {
                    // Retry.
                }
            }

            throw new NuGetUpdateException(UILabels.CannotRemoveBinFolder);
        }

        protected abstract bool RequestApplicationClose();

        private void CreateUninstallShortCut(string targetPath)
        {
            bool createShortcuts = Runner.Variables.GetOptional<bool>(
                Constants.ScriptVariables.CreateShortcuts
            );

            if (!createShortcuts)
                return;

            string startMenuPath = Runner.Variables.GetRequired<string>(
                Constants.ScriptVariables.StartMenuPath
            );

            string startMenuBase = NativeMethods.SHGetFolderPath(
                IntPtr.Zero,
                NativeMethods.SpecialFolderCSIDL.CSIDL_STARTMENU,
                IntPtr.Zero,
                0
            );

            startMenuPath = Path.Combine(startMenuBase, startMenuPath);

            AddToInstallLog(new InstallLogCreateDirectory
            {
                Path = startMenuPath,
                Force = false
            });

            Directory.CreateDirectory(startMenuPath);

            string shortcutFileName = Path.Combine(
                startMenuPath,
                UILabels.Uninstall + ".lnk"
            );

            AddToInstallLog(new InstallLogCreateFile
            {
                Path = shortcutFileName
            });

            RaiseProgressChanged(String.Format(UILabels.CreatingShortcut, shortcutFileName));

            var shellLink = new ShellLink
            {
                Target = Path.Combine(targetPath, Constants.NuGetUpdateFileName),
                IconIndex = 1,
                IconPath = Path.Combine(targetPath, Constants.NuGetUpdateFileName),
                Arguments = String.Format("-r -p {0}", Escaping.ShellEncode(Runner.Environment.Config.PackageCode))
            };

            shellLink.SetPropertyValue(PropertyStoreProperty.AppUserModel_StartPinOption, 1 /* APPUSERMODEL_STARTPINOPTION_NOPINONINSTALL */);
            shellLink.SetPropertyValue(PropertyStoreProperty.AppUserModel_ExcludeFromShowInNewInstall, true);

            shellLink.Save(shortcutFileName);
        }

        public override void UninstallPackage(UninstallPackage action)
        {
            var manager = new UninstallManager(InstallLog);

            manager.ProgressChanged += (s, e) => RaiseProgressChanged(e.Message);

            manager.Execute();
        }

        private void CopyDirectory(string source, string destination, string subDirectory, bool overwrite, int fileCount, ref int currentFile)
        {
            string fullSource = source;
            string fullDestination = destination;

            if (subDirectory != null)
            {
                fullSource = Path.Combine(fullSource, subDirectory);
                fullDestination = Path.Combine(fullDestination, subDirectory);
            }

            Directory.CreateDirectory(fullDestination);

            foreach (string fileName in Directory.GetFiles(fullSource))
            {
                string shortFileName = Path.GetFileName(fileName);
                string fullFileName = shortFileName;

                if (subDirectory != null)
                    fullFileName = Path.Combine(subDirectory, fullFileName);

                RaiseProgressChanged(
                    String.Format(UILabels.CopyingFile, fullFileName),
                    (double)currentFile / fileCount
                );

                string destinationFileName = Path.Combine(fullDestination, shortFileName);

                if (overwrite || !File.Exists(destinationFileName))
                    File.Copy(fileName, destinationFileName, overwrite);

                currentFile++;
            }

            foreach (string directory in Directory.GetDirectories(fullSource))
            {
                string fullDirectory = Path.GetFileName(directory);

                if (subDirectory != null)
                    fullDirectory = Path.Combine(subDirectory, fullDirectory);

                CopyDirectory(source, destination, fullDirectory, overwrite, fileCount, ref currentFile);
            }
        }

        private void AddToInstallLog(InstallLogEntry entry)
        {
            if (!InstallLog.Contains(entry))
                InstallLog.Add(entry);
        }
    }
}
