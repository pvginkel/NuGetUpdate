using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using NuGetUpdate.Installer.InstallLogging;
using NuGetUpdate.Installer.Pages;
using NuGetUpdate.Installer.ScriptEngine;
using NuGetUpdate.Shared;
using System.IO;

namespace NuGetUpdate.Installer
{
    partial class MainForm
    {
        private class Visitor : ScriptRunnerVisitor
        {
            private readonly MainForm _form;

            public override ScriptRunner Runner
            {
                get { return _form._runner; }
            }

            public List<InstallLogEntry> InstallLog { get; private set; }

            public Visitor(MainForm form)
            {
                _form = form;

                InstallLog = new List<InstallLogEntry>();
            }

            public override void ControlCheckBox(ControlCheckBox action)
            {
                _form.AddControlToPage(action);
            }

            public override void ControlLabel(ControlLabel action)
            {
                _form.AddControlToPage(action);
            }

            public override void ControlLink(ControlLink action)
            {
                _form.AddControlToPage(action);
            }

            public override void ExecShell(ExecShell action)
            {
                string fileName = Runner.InvokeExpression<string>(action.FileName);

                _form.RaiseProgressChanged(
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

                _form.RaiseProgressChanged(
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
                _form.RaiseProgressChanged(
                    Runner.ParseTemplate(action.Value)
                );
            }

            public override void MessageBox(MessageBox action)
            {
                _form.Invoke(new Action(() =>
                {
                    var result = System.Windows.Forms.MessageBox.Show(
                        _form,
                        Runner.ParseTemplate(action.Text),
                        UILabels.NuGetSetup,
                        Enum<System.Windows.Forms.MessageBoxButtons>.Parse(action.Buttons.ToString()),
                        Enum<System.Windows.Forms.MessageBoxIcon>.Parse(action.Icon.ToString())
                    );

                    if (action.Result != null)
                        Runner.Variables.AddOrSet(action.Result, result);
                }));
            }

            public override void PageInstallDestinationFolder(PageInstallDestinationFolder action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<InstallDestinationFolderPage>(Runner, action, continuation);
                }
            }

            public override void PageInstallFinish(PageInstallFinish action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<FinishPage>(Runner, continuation);
                }

                base.PageInstallFinish(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageInstallLicense(PageInstallLicense action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<InstallLicensePage>(Runner, action, continuation);
                }
            }

            public override void PageInstallProgress(PageInstallProgress action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<ProgressPage>(Runner, continuation);
                }

                base.PageInstallProgress(action);

                CommitInstallation();
            }

            private void CommitInstallation()
            {
                // We commit the installation at this point. Things may still
                // happen, but they won't be recorded in the metadata.

                using (var metadata = Metadata.Create(Runner.Environment.Config.PackageCode))
                {
                    metadata.InstalledVersion = Runner.Environment.Config.PackageVersion;
                    metadata.InstallLog = new InstallLog
                    {
                        Items = _form._visitor.InstallLog.ToArray()
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

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageInstallStartMenu(PageInstallStartMenu action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<InstallStartMenuPage>(Runner, action, continuation);
                }
            }

            public override void PageInstallWelcome(PageInstallWelcome action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<WelcomePage>(Runner, action, continuation);
                }
            }

            public override void PageUninstallFinish(PageUninstallFinish action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<FinishPage>(Runner, continuation);
                }

                base.PageUninstallFinish(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageUninstallProgress(PageUninstallProgress action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<ProgressPage>(Runner, continuation);
                }

                base.PageUninstallProgress(action);

                // We commit the installation at this point. Things may still
                // happen, but they won't be recorded in the metadata.

                Metadata.Delete(Runner.Environment.Config.PackageCode);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageUninstallWelcome(PageUninstallWelcome action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<WelcomePage>(Runner, action, continuation);
                }
            }

            public override void PageUpdateFinish(PageUpdateFinish action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<FinishPage>(Runner, continuation);
                }

                base.PageUpdateFinish(action);

                using (var continuation = Runner.GetContinuation())
                {
                    _form.WaitForPageClose(continuation);
                }
            }

            public override void PageUpdateProgress(PageUpdateProgress action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<ProgressPage>(Runner, continuation);
                }

                base.PageUpdateProgress(action);

                CommitInstallation();
            }

            public override void PageUpdateWelcome(PageUpdateWelcome action)
            {
                using (var continuation = Runner.GetContinuation())
                {
                    _form.ShowPage<WelcomePage>(Runner, action, continuation);
                }
            }

            public override void CreateDirectory(CreateDirectory action)
            {
                string directory = Runner.InvokeExpression<string>(action.Path);

                _form.RaiseProgressChanged(String.Format(UILabels.CreatingDirectory, directory));

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

                _form.RaiseProgressChanged(String.Format(UILabels.CreatingShortcut, shortcutFileName));

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
                    if (Runner.Mode == ScriptRunnerMode.Update)
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
                        _form.RaiseProgressChanged(
                            String.Format(UILabels.DeletingDirectory, targetPath)
                        );
                    }
                    else
                    {
                        _form.RaiseProgressChanged(
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
                    if (!_form.RequestApplicationClose())
                        throw new AbortedException();

                    _form.RaiseProgressChanged(
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

                _form.RaiseProgressChanged(String.Format(UILabels.CreatingShortcut, shortcutFileName));

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

                manager.ProgressChanged += (s, e) => _form.RaiseProgressChanged(e.Message);

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

                    _form.RaiseProgressChanged(
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
}
